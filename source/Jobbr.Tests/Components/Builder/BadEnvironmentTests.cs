using System;
using System.Threading;
using Jobbr.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Components.Builder
{
    [TestClass]
    public class BadEnvironmentTests
    {
        [TestMethod]
        public void StartJobbr_WithFaultyStorageProvider_RemainsStarting()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var jobbr = new JobbrServer(new DefaultJobbrConfiguration()
            //{
            //    JobStorageProvider = new NotImplementedJobStorageProvider(),
            //    ArtefactStorageProvider = new FileSystemArtefactsStorageProvider(AppDomain.CurrentDomain.BaseDirectory),
            //    JobRunnerExeResolver = () => @"C:\Windows\System32\cmd.exe"
            //});

            //var isStarted = jobbr.Start(1000);

            //Assert.AreEqual(JobbrState.Starting, jobbr.State);
            //Assert.IsFalse(isStarted);
        }

        [TestMethod]
        public void StartingJobber_GetsRunning_WhenStorageProviderTurnsHealthy()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var faultyJobStorageProvider = new FaultyJobStorageProvider();

            //var jobbr = new JobbrServer(new DefaultJobbrConfiguration()
            //{
            //    JobStorageProvider = faultyJobStorageProvider,
            //    ArtefactStorageProvider = new FileSystemArtefactsStorageProvider(AppDomain.CurrentDomain.BaseDirectory),
            //    JobRunnerExeResolver = () => @"C:\Windows\System32\cmd.exe"
            //});

            //faultyJobStorageProvider.DisableImplementation();
            //jobbr.Start(1000);

            //faultyJobStorageProvider.EnableImplementation();

            //this.WaitForStatusChange(() => jobbr.State, 5000);

            //Assert.AreEqual(JobbrState.Running, jobbr.State);
        }

        private void WaitForStatusChange(Func<JobbrState> state, int timeout)
        {
            var initialState = state();

            for (int i = 0; i < timeout / 500; i++)
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
