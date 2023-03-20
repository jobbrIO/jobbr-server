using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobRunStates = Jobbr.ComponentModel.Execution.Model.JobRunStates;

namespace Jobbr.Server.IntegrationTests.Integration.Execution
{
    [TestClass]
    public class ProgressChannelTests : JobRunExecutionTestBase
    {
        private readonly JobRun _currentRun;

        public ProgressChannelTests()
        {
            var job = CreateTestJob();
            var trigger = CreateInstantTrigger(job);

            _currentRun = TriggerNewJobRun(trigger);
        }

        [TestMethod]
        public void Infrastructure_TriggeredJobIsReady()
        {
            Assert.IsNotNull(_currentRun);
        }

        [TestMethod]
        public void ProgressUpdate_With50Percent_IsStored()
        {
            var progressService = Services.ProgressChannel;

            progressService.PublishProgressUpdate(_currentRun.Id, 50);

            var jobRunFromDb = Services.JobStorageProvider.GetJobRunsByTriggerId(_currentRun.Job.Id, _currentRun.Trigger.Id).Items.Single();

            Assert.AreEqual(50, jobRunFromDb.Progress);
        }

        [TestMethod]
        public void PublishPid_WithRandomInt_IsStored()
        {
            var progressService = Services.ProgressChannel;

            progressService.PublishPid(_currentRun.Id, 42373, "host01");

            var jobRunFromDb = Services.JobStorageProvider.GetJobRunsByTriggerId(_currentRun.Job.Id, _currentRun.Trigger.Id).Items.Single();

            Assert.AreEqual(42373, jobRunFromDb.Pid);
        }

        [TestMethod]
        public void StateUpdate_GetsPreparing_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Preparing);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Preparing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarting_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Starting);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Starting, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarted_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Started);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Started, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarted_StartDateIsUpdated()
        {
            SimulateStateUpdate(JobRunStates.Started);

            var actualJobRun = Services.JobStorageProvider.GetJobRunsByTriggerId(_currentRun.Job.Id, _currentRun.Trigger.Id).Items.Single();

            Assert.IsNotNull(actualJobRun.ActualStartDateTimeUtc);
        }

        /// <summary>
        /// The connected state is usually needed if a forked executor signalizes the connection back to the server.
        /// </summary>
        [TestMethod]
        public void StateUpdate_GetsConnected_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Connected);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Connected, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsInitializing_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Initializing);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Initializing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsProcessing_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Processing);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Processing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsFinishing_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Finishing);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Finishing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsCollecting_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Collecting);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Collecting, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsCompleted_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Completed);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Completed, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsFailed_IsStored()
        {
            SimulateStateUpdate(JobRunStates.Failed);

            var actualState = GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Failed, actualState);
        }

        private ComponentModel.JobStorage.Model.JobRunStates GetActualStoredJobRunState()
        {
            return Services.JobStorageProvider.GetJobRunsByTriggerId(_currentRun.Job.Id, _currentRun.Trigger.Id).Items.Single().State;
        }

        private void SimulateStateUpdate(JobRunStates state)
        {
            var progressService = Services.ProgressChannel;

            progressService.PublishStatusUpdate(_currentRun.Id, state);
        }
    }
}
