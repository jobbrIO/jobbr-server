using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobRunStates = Jobbr.ComponentModel.Execution.Model.JobRunStates;

namespace Jobbr.Tests.Integration.Execution
{
    [TestClass]
    public class ProgressChannelTests : JobRunExecutionTestBase
    {
        private readonly JobRun currentRun;

        public ProgressChannelTests()
        {
            var job = this.CreateTestJob();
            var trigger = CreateInstantTrigger(job);

            this.currentRun = this.TriggerNewJobRun(trigger);
        }

        [TestMethod]
        public void Infrastructure_TriggeredJobIsReady()
        {
            Assert.IsNotNull(this.currentRun);
        }

        [TestMethod]
        public void ProgressUpdate_With50Percent_IsStored()
        {
            var progressService = this.Services.ProgressChannel;

            progressService.PublishProgressUpdate(this.currentRun.Id, 50);

            var jobRunFromDb = this.Services.JobStorageProvider.GetJobRunsByTriggerId(this.currentRun.Job.Id, this.currentRun.Trigger.Id).Single();

            Assert.AreEqual(50, jobRunFromDb.Progress);
        }

        [TestMethod]
        public void PublishPid_WithRandomInt_IsStored()
        {
            var progressService = this.Services.ProgressChannel;

            progressService.PublishPid(this.currentRun.Id, 42373, "host01");

            var jobRunFromDb = this.Services.JobStorageProvider.GetJobRunsByTriggerId(this.currentRun.Job.Id, this.currentRun.Trigger.Id).Single();

            Assert.AreEqual(42373, jobRunFromDb.Pid);
        }

        [TestMethod]
        public void StateUpdate_GetsPreparing_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Preparing);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Preparing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarting_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Starting);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Starting, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarted_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Started);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Started, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsStarted_StartDateIsUpdated()
        {
            this.SimulateStateUpdate(JobRunStates.Started);

            var actualJobRun = this.Services.JobStorageProvider.GetJobRunsByTriggerId(this.currentRun.Job.Id, this.currentRun.Trigger.Id).Single();

            Assert.IsNotNull(actualJobRun.ActualStartDateTimeUtc);
        }

        /// <summary>
        /// The connected state is usually needed if a forked executor signalizes the connection back to the server
        /// </summary>
        [TestMethod]
        public void StateUpdate_GetsConnected_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Connected);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Connected, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsInitializing_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Initializing);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Initializing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsProcessing_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Processing);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Processing, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsFinishing_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Finishing);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Finishing, actualState);
        }


        [TestMethod]
        public void StateUpdate_GetsCollecting_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Collecting);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Collecting, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsCompleted_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Completed);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Completed, actualState);
        }

        [TestMethod]
        public void StateUpdate_GetsFailed_IsStored()
        {
            this.SimulateStateUpdate(JobRunStates.Failed);

            var actualState = this.GetActualStoredJobRunState();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Failed, actualState);
        }

        private ComponentModel.JobStorage.Model.JobRunStates GetActualStoredJobRunState()
        {
            return this.Services.JobStorageProvider.GetJobRunsByTriggerId(this.currentRun.Job.Id, this.currentRun.Trigger.Id).Single().State;
        }

        private void SimulateStateUpdate(JobRunStates state)
        {
            var progressService = this.Services.ProgressChannel;

            progressService.PublishStatusUpdate(this.currentRun.Id, state);
        }
    }
}
