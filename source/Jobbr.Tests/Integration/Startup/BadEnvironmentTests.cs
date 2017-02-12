using System;
using System.Threading;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Jobbr.Tests.Infrastructure.StorageProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Startup
{
    [TestClass]
    public class BadEnvironmentTests
    {
        [TestMethod]
        public void StartJobbr_WithFaultyStorageProvider_RemainsStarting()
        {
            var builder = new JobbrBuilder();
            builder.Register<IJobStorageProvider>(typeof(NotImplementedJobStorageProvider));

            var jobbr = builder.Create();

            var isStarted = jobbr.Start(1000);

            Assert.AreEqual(JobbrState.Starting, jobbr.State);
            Assert.IsFalse(isStarted);
        }

        [TestMethod]
        public void StartingJobber_GetsRunning_WhenStorageProviderTurnsHealthy()
        {
            var builder = new JobbrBuilder();
            builder.Register<IJobStorageProvider>(typeof(FaultyJobStorageProvider));

            var jobbr = builder.Create();
            var faultyJobStorageProvider = FaultyJobStorageProvider.Instance;

            faultyJobStorageProvider.DisableImplementation();
            jobbr.Start(1000);

            faultyJobStorageProvider.EnableImplementation();

            this.WaitForStatusChange(() => jobbr.State, 5000);

            Assert.AreEqual(JobbrState.Running, jobbr.State);
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
    }
}