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
            currentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Scheduled);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Starting);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Completed);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Finishing);
            AddJobRun(currentTime.AddDays(-1), JobRunStates.Failed);

            scheduler.Start();

            var schedulerJobRuns = repository.GetJobRunsByState(JobRunStates.Scheduled);
            var omittedJobRuns = repository.GetJobRunsByState(JobRunStates.Omitted);

            Assert.AreEqual(0, schedulerJobRuns.Items.Count, "The only past scheduled jobrun should have been set to omitted");
            Assert.AreEqual(1, omittedJobRuns.Items.Count, "It's assumed that the previous scheduled jobrun is now omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasScheduledJobsInFuture_WillNotTouch()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            currentTimeProvider.Set(currentTime);

            // Add a couple of jobruns
            AddJobRun(currentTime.AddDays(1), JobRunStates.Scheduled);

            scheduler.Start();

            var schedulerJobRuns = repository.GetJobRunsByState(JobRunStates.Scheduled);

            Assert.AreEqual(1, schedulerJobRuns.Items.Count, "A future scheduled jobrun should not be omitted");
        }

        [TestMethod]
        public void SchedulerStarts_HasSRunningJobsFromPast_WillSetToFailed()
        {
            var currentTime = new DateTime(2017, 04, 06, 0, 0, 0);
            currentTimeProvider.Set(currentTime);

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

            scheduler.Start();

            var failedJobRuns = repository.GetJobRunsByState(JobRunStates.Failed);

            Assert.AreEqual(9, failedJobRuns.Items.Count, $"Still have jobruns with the following states:\n {string.Join(", ", repository.GetJobRuns().Items.Select(jr => jr.State))}");
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenNewestShouldBeExecuted()
        {
            scheduler.Start();
            var jobId = AddAndSaveJob(1);
            var dateFrom2091 = new DateTime(2091, 5, 17);
            var dateFrom2092 = new DateTime(2092, 7, 12);
            var dateFrom2093 = new DateTime(2093, 7, 12);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2091 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2092 };
            var thirdTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = dateFrom2093 };
            repository.SaveAddTrigger(jobId, firstTrigger);
            repository.SaveAddTrigger(jobId, secondTrigger);
            repository.SaveAddTrigger(jobId, thirdTrigger);

            scheduler.OnTriggerAdded(jobId, firstTrigger.Id);
            scheduler.OnTriggerAdded(jobId, secondTrigger.Id);

            Assert.AreEqual(1, lastIssuedPlan.Count);
            Assert.AreEqual(dateFrom2091, lastIssuedPlan.Single().PlannedStartDateTimeUtc);

            scheduler.OnTriggerAdded(jobId, thirdTrigger.Id);

            Assert.AreEqual(1, lastIssuedPlan.Count);
            Assert.AreEqual(dateFrom2091, lastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void GivenMultipleScheduledJobRuns_WhenLimitingAmountOfParallelRuns_ThenLatestShouldNotBeExecuted()
        {
            scheduler.Start();
            var jobId = AddAndSaveJob(2);
            var startTimeTrigger1 = new DateTime(2100, 1, 1);
            var startTimeTrigger2 = new DateTime(2200, 1, 1);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger1 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger2 };
            repository.SaveAddTrigger(jobId, firstTrigger);
            repository.SaveAddTrigger(jobId, secondTrigger);
            AddJobRun(new DateTime(2050, 1, 1), JobRunStates.Started, jobId);
            scheduler.OnTriggerAdded(jobId, firstTrigger.Id);

            scheduler.OnTriggerAdded(jobId, secondTrigger.Id);

            Assert.AreEqual(1, lastIssuedPlan.Count);
            Assert.AreEqual(startTimeTrigger1, lastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void ShouldQueueJobWhenJobRunEnded()
        {
            scheduler.Start();
            var jobId = AddAndSaveJob(1);
            var startTimeTrigger1 = new DateTime(2100, 1, 1);
            var startTimeTrigger2 = new DateTime(2200, 1, 1);
            var firstTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger1 };
            var secondTrigger = new ScheduledTrigger { JobId = jobId, IsActive = true, StartDateTimeUtc = startTimeTrigger2 };
            repository.SaveAddTrigger(jobId, firstTrigger);
            repository.SaveAddTrigger(jobId, secondTrigger);
            scheduler.OnTriggerAdded(jobId, firstTrigger.Id);
            var jobRunId = lastIssuedPlan.Single().Id;

            scheduler.OnJobRunEnded(jobRunId);

            Assert.AreEqual(1, lastIssuedPlan.Count);
            Assert.AreEqual(startTimeTrigger1, lastIssuedPlan.Single().PlannedStartDateTimeUtc);
        }

        private void AddJobRun(DateTime plannedStartDateTimeUtc, JobRunStates state, long jobId = 0)
        {
            var accordingJobId = jobId != 0 ? jobId : demoJob1Id;
            var scheduledTrigger = new InstantTrigger() { JobId = accordingJobId, IsActive = true };
            var demoJob = repository.GetJob(accordingJobId);

            var jobRun = repository.SaveNewJobRun(demoJob, scheduledTrigger, plannedStartDateTimeUtc);
            jobRun.State = state;
            repository.Update(jobRun);
        }

        private long AddAndSaveJob(int maxConcurrentJobRuns)
        {
            var job = new Job
            {
                MaxConcurrentJobRuns = maxConcurrentJobRuns
            };
            repository.AddJob(job);
            return job.Id;
        }
    }
}
