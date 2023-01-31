using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Components.Scheduler
{
    [TestClass]
    public class SchedulerTests : TestBase
    {
        [TestMethod]
        public void SchedulerStarts_HasScheduledJobsFromPast_WillSetToOmitted()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            CurrentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Scheduled);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Starting);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Completed);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Finishing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Failed);

            Scheduler.Start();

            var schedulerJobRuns = Repository.GetJobRunsByState(JobRunStates.Scheduled);
            var omittedJobRuns = Repository.GetJobRunsByState(JobRunStates.Omitted);

            Assert.AreEqual(0, schedulerJobRuns.Items.Count, "The only past scheduled jobrun should have been set to omitted");
            Assert.AreEqual(1, omittedJobRuns.Items.Count, "It's assumed that the previous scheduled jobrun is now omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasScheduledJobsInFuture_WillNotTouch()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            CurrentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            AddJobRun(currentTime.AddDays(1), JobRunStates.Scheduled);

            Scheduler.Start();

            var schedulerJobRuns = Repository.GetJobRunsByState(JobRunStates.Scheduled);

            Assert.AreEqual(1, schedulerJobRuns.Items.Count, "A future scheduled jobrun should not be omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasSRunningJobsFromPast_WillSetToFailed()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            CurrentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Preparing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Starting);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Started);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Started);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Connected);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Initializing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Processing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Finishing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Collecting);

            AddJobRun(currentTime.AddDays(-1), JobRunStates.Completed);

            Scheduler.Start();

            var failedJobRuns = Repository.GetJobRunsByState(JobRunStates.Failed);

            Assert.AreEqual(9, failedJobRuns.Items.Count, $"Still have jobruns with the following states:\n {string.Join(", ", Repository.GetJobRuns().Items.Select(jr => jr.State))}");
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenNewestShouldBeExecuted()
        {
            Scheduler.Start();
            var jobId = AddAndSaveJob(1);
            var dateFrom2091 = new DateTime(2091, 5, 17);
            var dateFrom2092 = new DateTime(2092, 7, 12);
            var dateFrom2093 = new DateTime(2093, 7, 12);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2091 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2092 };
            var thirdTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2093 };
            Repository.SaveAddTrigger(jobId, firstTrigger);
            Repository.SaveAddTrigger(jobId, secondTrigger);
            Repository.SaveAddTrigger(jobId, thirdTrigger);

            Scheduler.OnTriggerAdded(jobId, firstTrigger.Id);
            Scheduler.OnTriggerAdded(jobId, secondTrigger.Id);

            Assert.AreEqual(1, LastIssuedPlan.Count);
            Assert.AreEqual(dateFrom2091, LastIssuedPlan.Single().PlannedStartDateTimeUtc);

            Scheduler.OnTriggerAdded(jobId, thirdTrigger.Id);

            Assert.AreEqual(1, LastIssuedPlan.Count);
            Assert.AreEqual(dateFrom2091, LastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenLatestShouldNotBeExecuted()
        {
            Scheduler.Start();
            var jobId = AddAndSaveJob(2);
            var startTimeTrigger1 = new DateTime(2100, 1, 1);
            var startTimeTrigger2 = new DateTime(2200, 1, 1);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger1 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger2 };
            Repository.SaveAddTrigger(jobId, firstTrigger);
            Repository.SaveAddTrigger(jobId, secondTrigger);
            AddJobRun(new DateTime(2050, 1, 1), JobRunStates.Started, jobId);
            Scheduler.OnTriggerAdded(jobId, firstTrigger.Id);

            Scheduler.OnTriggerAdded(jobId, secondTrigger.Id);

            Assert.AreEqual(1, LastIssuedPlan.Count);
            Assert.AreEqual(startTimeTrigger1, LastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void ShouldQueueJobWhenJobRunEnded()
        {
            Scheduler.Start();
            var jobId = AddAndSaveJob(1);
            var startTimeTrigger1 = new DateTime(2100, 1, 1);
            var startTimeTrigger2 = new DateTime(2200, 1, 1);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger1 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger2 };
            Repository.SaveAddTrigger(jobId, firstTrigger);
            Repository.SaveAddTrigger(jobId, secondTrigger);
            Scheduler.OnTriggerAdded(jobId, firstTrigger.Id);
            var jobRunId = LastIssuedPlan.Single().Id;

            Scheduler.OnJobRunEnded(jobRunId);

            Assert.AreEqual(1, LastIssuedPlan.Count);
            Assert.AreEqual(startTimeTrigger1, LastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        private void AddJobRun(DateTime plannedStartDateTimeUtc, JobRunStates state, long jobId = 0)
        {
            var accordingJobId = jobId != 0 ? jobId : DemoJob1Id;
            var scheduledTrigger = new InstantTrigger { JobId = accordingJobId, IsActive = true };
            var demoJob = Repository.GetJob(accordingJobId);

            var jobRun = Repository.SaveNewJobRun(demoJob, scheduledTrigger, plannedStartDateTimeUtc);
            jobRun.State = state;
            Repository.Update(jobRun);
        }

        private long AddAndSaveJob(int maxConcurrentJobRuns)
        {
            var job = new Job
            {
                MaxConcurrentJobRuns = maxConcurrentJobRuns
            };
            Repository.AddJob(job);
            return job.Id;
        }
    }
}
