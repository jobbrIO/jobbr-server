using System;
using System.Linq;
using Jobbr.Common.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Tests.Setup;
using Jobbr.Tests.StorageProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Scheduler
{
    [TestClass]
    public class SchedulerTests
    {
        [TestMethod]
        public void JobRunIsScheduled_RecurringTriggerGetsUpdated_JobRunScheduleIsAdjusted()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);

            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());

            var currentMinutesInHour = DateTime.UtcNow.Minute;
            var futureMinute = (currentMinutesInHour + 5)%60;
            var futureMinute2 = (futureMinute + 2)%60;

            var recurringTrigger = new RecurringTrigger {Definition = futureMinute + " * * * *", JobId = demoJob.Id, IsActive = true};
            jobService.AddTrigger(recurringTrigger);
            scheduler.Start();

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            // Base Assertions
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun which is not null");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureMinute, createdJobRun.PlannedStartDateTimeUtc.Minute);

            jobService.UpdateTrigger(recurringTrigger.Id, new RecurringTrigger {Definition = futureMinute2 + " * * * *", IsActive = true});

            var updatedJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(futureMinute2, updatedJobRun.PlannedStartDateTimeUtc.Minute, "As per updated definition, the job should now start on a different plan");
        }

        [TestMethod]
        public void JobRunIsScheduler_JobRunWillBeRemoved_WhenTriggerGetsDisabled()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);

            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());
            var futureDate1 = DateTime.UtcNow.AddHours(2);

            var trigger = new ScheduledTrigger {JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true};
            jobService.AddTrigger(trigger);

            scheduler.Start();

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun which is not null");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureDate1, createdJobRun.PlannedStartDateTimeUtc);

            jobService.DisableTrigger(trigger.Id);
            var jobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(JobRunState.Deleted, jobRun.State);
        }

        [TestMethod]
        public void JobRunIsScheduler_JobRunWillBeScheduled_WhenTriggerIsEnabled()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);

            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());
            var futureDate1 = DateTime.UtcNow.AddHours(2);

            var trigger = new ScheduledTrigger {JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = false};
            jobService.AddTrigger(trigger);

            scheduler.Start();

            // Base asserts
            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();
            Assert.IsNull(createdJobRun, "There should be exact no JobRun");

            jobService.EnableTrigger(trigger.Id);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var jobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(JobRunState.Scheduled, jobRun.State);
        }

        [TestMethod]
        public void NoParallelExecutionDisabled_TriggerWhileJobIsStillRunning_NextJobRunIsCreated()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var demoJob2 = new Job();
            storageProvider.AddJob(demoJob2);

            var jobService = new JobService(storageProvider);

            var recurringTrigger = new RecurringTrigger {Definition = "* * * * *", JobId = demoJob.Id, IsActive = true, NoParallelExecution = false, StartDateTimeUtc = DateTime.UtcNow.AddDays(-1)};
            jobService.AddTrigger(recurringTrigger);

            var trigger = new ScheduledTrigger {JobId = demoJob2.Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(10), IsActive = true};
            jobService.AddTrigger(trigger);

            storageProvider.AddJobRun(new JobRun {State = JobRunState.Processing, TriggerId = recurringTrigger.Id, JobId = demoJob.Id});

            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());

            scheduler.Start();

            WaitFor.MinElements(() => storageProvider.GetJobRuns(), 3);

            var jobRuns = storageProvider.GetJobRuns();

            Assert.AreEqual(3, jobRuns.Count);
        }

        [TestMethod]
        public void NoParallelExecutionEnabled_TriggerWhileJobIsStillRunning_NextJobRunIsPrevented()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var demoJob2 = new Job();
            storageProvider.AddJob(demoJob2);

            var jobService = new JobService(storageProvider);

            var recurringTrigger = new RecurringTrigger {Definition = "* * * * *", JobId = demoJob.Id, IsActive = true, NoParallelExecution = true, StartDateTimeUtc = DateTime.UtcNow.AddDays(-1)};
            jobService.AddTrigger(recurringTrigger);

            var trigger = new ScheduledTrigger {JobId = demoJob2.Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(10), IsActive = true};
            jobService.AddTrigger(trigger);

            storageProvider.AddJobRun(new JobRun {State = JobRunState.Processing, TriggerId = recurringTrigger.Id, JobId = demoJob.Id});

            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());

            scheduler.Start();

            WaitFor.MinElements(() => storageProvider.GetJobRuns(), 2);

            var jobRuns = storageProvider.GetJobRuns();

            Assert.AreEqual(2, jobRuns.Count);
            Assert.AreEqual(1, jobRuns[0].JobId);
            Assert.AreEqual(2, jobRuns[1].JobId);
        }
    }
}