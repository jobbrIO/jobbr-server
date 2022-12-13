﻿using System;
using System.Threading;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Jobbr.Server.IntegrationTests.Infrastructure;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration.Startup
{
    [TestClass]
    public class BadEnvironmentTests
    {
        [TestMethod]
        public void StartingJobber_GetsRunning_WhenStorageProviderTurnsHealthy()
        {
            var builder = new JobbrBuilder(new NullLoggerFactory());
            builder.Register<IJobStorageProvider>(typeof(FaultyJobStorageProvider));

            var jobbr = builder.Create();
            var faultyJobStorageProvider = FaultyJobStorageProvider.Instance;

            faultyJobStorageProvider.DisableImplementation();
            jobbr.Start(1000);

            faultyJobStorageProvider.EnableImplementation();

            WaitForStatusChange(() => jobbr.State, 5000);

            Assert.AreEqual(JobbrState.Running, jobbr.State);
        }

        [TestMethod]
        public void StartingJobbr_ComponentFails_IsInErrorState()
        {
            var nullLoggerFactory = new NullLoggerFactory();
            var builder = new JobbrBuilder(nullLoggerFactory);
            builder.RegisterForCollection<IJobbrComponent>(typeof(FaultyComponent));

            var jobbr = builder.Create();

            try
            {
                jobbr.Start();
            }
            catch (Exception)
            {
                // Eat exception if any to check for the state
            }

            Assert.AreEqual(JobbrState.Error, jobbr.State);
            nullLoggerFactory.Dispose();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void StartingJobbr_ComponentFails_ExceptionIsThrown()
        {
            var builder = new JobbrBuilder(new NullLoggerFactory());
            builder.RegisterForCollection<IJobbrComponent>(typeof(FaultyComponent));

            var jobbr = builder.Create();

            jobbr.Start();
        }

        private void WaitForStatusChange(Func<JobbrState> state, int timeout)
        {
            var initialState = state();

            for (var i = 0; i < timeout / 500; i++)
            {
                var newState = state();

                if (newState != initialState)
                {
                    return;
                }

                Thread.Sleep(500);
            }
        }

        public class FaultyComponent : IJobbrComponent
        {
            public void Dispose()
            {
            }

            public void Start()
            {
                throw new Exception();
            }

            public void Stop()
            {
            }
        }
    }
}