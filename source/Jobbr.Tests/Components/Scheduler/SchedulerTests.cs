using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Components.Scheduler
{
    [TestClass]
    public class SchedulerTests : TestBase
    {
        [TestMethod]
        public void SchedulerStarts_HasScheduledJobsFromPast_WillSetToOmitted()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            this.currentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Scheduled);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Starting);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Completed);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Finishing);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Failed);

            this.scheduler.Start();

            var schedulerJobRuns = this.repository.GetJobRunsByState(JobRunStates.Scheduled);
            var omittedJobRuns = this.repository.GetJobRunsByState(JobRunStates.Omitted);

            Assert.AreEqual(0, schedulerJobRuns.Items.Count, "The only past scheduled jobrun should have been set to omitted");
            Assert.AreEqual(1, omittedJobRuns.Items.Count, "It's assumed that the previous scheduled jobrun is now omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasScheduledJobsInFuture_WillNotTouch()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            this.currentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            this.AddJobRun(currentTime.AddDays(1), JobRunStates.Scheduled);

            this.scheduler.Start();

            var schedulerJobRuns = this.repository.GetJobRunsByState(JobRunStates.Scheduled);

            Assert.AreEqual(1, schedulerJobRuns.Items.Count, "A future scheduled jobrun should not be omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasSRunningJobsFromPast_WillSetToFailed()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            this.currentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Preparing);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Starting);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Started);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Started);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Connected);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Initializing);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Processing);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Finishing);
            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Collecting);

            this.AddJobRun(currentTime.AddDays(-1), JobRunStates.Completed);

            this.scheduler.Start();

            var failedJobRuns = this.repository.GetJobRunsByState(JobRunStates.Failed);

            Assert.AreEqual(9, failedJobRuns.Items.Count, $"Still have jobruns with the following states:\n {string.Join(", ", this.repository.GetJobRuns().Items.Select(jr => jr.State))}");
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenNewestShouldBeExecuted()
        {
            this.scheduler.Start();
            var job = new Job
            {
                MaxConcurrentJobRuns = 1
            };
            this.repository.AddJob(job);
            var dateFrom2091 = new DateTime(2091, 5, 17);
            var dateFrom2092 = new DateTime(2092, 7, 12);
            var dateFrom2093 = new DateTime(2093, 7, 12);
            var firstTrigger = new ScheduledTrigger {JobId = job.Id, IsActive = true, StartDateTimeUtc = dateFrom2091};
            var secondTrigger = new ScheduledTrigger { JobId = job.Id, IsActive = true, StartDateTimeUtc = dateFrom2092 };
            var thirdTrigger = new ScheduledTrigger { JobId = job.Id, IsActive = true, StartDateTimeUtc = dateFrom2093 };
            this.repository.SaveAddTrigger(job.Id, firstTrigger);
            this.repository.SaveAddTrigger(job.Id, secondTrigger);
            this.repository.SaveAddTrigger(job.Id, thirdTrigger);

            this.scheduler.OnTriggerAdded(job.Id, firstTrigger.Id);
            this.scheduler.OnTriggerAdded(job.Id, secondTrigger.Id);

           Assert.AreEqual(1, this.lastIssuedPlan.Count);
           Assert.AreEqual(dateFrom2091, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc);

           this.scheduler.OnTriggerAdded(job.Id, thirdTrigger.Id);

           Assert.AreEqual(1, this.lastIssuedPlan.Count);
           Assert.AreEqual(dateFrom2091, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenLatestShouldNotBeExecuted()
        {
            this.scheduler.Start();
            var job = new Job
            {
                MaxConcurrentJobRuns = 2
            };
            this.repository.AddJob(job);
            var startTimeTrigger1 = new DateTime(2100, 1, 1);
            var startTimeTrigger2 = new DateTime(2200, 1, 1);
            var firstTrigger = new ScheduledTrigger { JobId = job.Id, IsActive = true, StartDateTimeUtc = startTimeTrigger1 };
            var secondTrigger = new ScheduledTrigger { JobId = job.Id, IsActive = true, StartDateTimeUtc = startTimeTrigger2 };
            this.repository.SaveAddTrigger(job.Id, firstTrigger);
            this.repository.SaveAddTrigger(job.Id, secondTrigger);
            this.AddJobRun(new DateTime(2050, 1, 1), JobRunStates.Started, job.Id);
            this.scheduler.OnTriggerAdded(job.Id, firstTrigger.Id);

            this.scheduler.OnTriggerAdded(job.Id, secondTrigger.Id);

            Assert.AreEqual(1, this.lastIssuedPlan.Count);
            Assert.AreEqual(startTimeTrigger1, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        private void AddJobRun(DateTime plannedStartDateTimeUtc, JobRunStates state, long jobId = 0)
        {
            var accordingJobId = jobId != 0 ? jobId : this.demoJob1Id;
            var scheduledTrigger = new InstantTrigger() { JobId = accordingJobId, IsActive = true };
            var demoJob = this.repository.GetJob(accordingJobId);

            var jobRun = this.repository.SaveNewJobRun(demoJob, scheduledTrigger, plannedStartDateTimeUtc);
            jobRun.State = state;
            this.repository.Update(jobRun);
        }
    }
}
