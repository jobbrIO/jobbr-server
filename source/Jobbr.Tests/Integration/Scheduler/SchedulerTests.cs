using System;
using System.Linq;
using Jobbr.ComponentModel.Management.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Job = Jobbr.ComponentModel.JobStorage.Model.Job;
using JobRunStates = Jobbr.ComponentModel.JobStorage.Model.JobRunStates;

namespace Jobbr.Tests.Integration.Scheduler
{
    [TestClass]
    public class SchedulerTests : RunningJobbrServerTestBase
    {
        [TestMethod]
        public void JobRunIsScheduled_RecurringTriggerGetsUpdated_JobRunScheduleIsAdjusted()
        {
            var jobManagementService = this.Services.JobManagementService;
            var storageProvider = this.Services.JobStorageProvider;

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var currentMinutesInHour = DateTime.UtcNow.Minute;
            var futureMinute = (currentMinutesInHour + 5) % 60;
            var futureMinute2 = (futureMinute + 2) % 60;

            var recurringTrigger = new RecurringTrigger { JobId = 1, Definition = futureMinute + " * * * *", IsActive = true };
            jobManagementService.AddTrigger(1, recurringTrigger);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            // Base Assertions
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun which is not null");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureMinute, createdJobRun.PlannedStartDateTimeUtc.Minute);

            jobManagementService.UpdateTriggerDefinition(1, recurringTrigger.Id, futureMinute2 + " * * * *");

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns().Where(r => r.PlannedStartDateTimeUtc.Minute == futureMinute2).ToList());

            var updatedJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(futureMinute2, updatedJobRun.PlannedStartDateTimeUtc.Minute, "As per updated definition, the job should now start on a different plan");
        }

        [TestMethod]
        public void JobRunIsScheduled_ScheduledTriggerGetsUpdated_JobRunScheduleIsAdjusted()
        {
            var jobManagementService = this.Services.JobManagementService;
            var storageProvider = this.Services.JobStorageProvider;

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var futureDate1 = DateTime.UtcNow.AddHours(1);
            var futureDate2 = DateTime.UtcNow.AddHours(10);

            var recurringTrigger = new ScheduledTrigger() { JobId = 1, StartDateTimeUtc = futureDate1, IsActive = true };
            jobManagementService.AddTrigger(1, recurringTrigger);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            // Base Assertions
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun which is not null");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureDate1, createdJobRun.PlannedStartDateTimeUtc);

            jobManagementService.UpdateTriggerStartTime(1, recurringTrigger.Id, futureDate2);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns().Where(r => r.PlannedStartDateTimeUtc == futureDate2).ToList());

            var updatedJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(futureDate2, updatedJobRun.PlannedStartDateTimeUtc, "As per updated startddate, the job should now start on a different point in time");
        }

        [TestMethod]
        public void JobRunIsScheduler_WhenTriggerGetsDisabled_JobRunWillBeRemoved()
        {
            var jobManagementService = this.Services.JobManagementService;
            var storageProvider = this.Services.JobStorageProvider;

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var futureDate1 = DateTime.UtcNow.AddHours(2);

            var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };
            jobManagementService.AddTrigger(demoJob.Id, trigger);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureDate1, createdJobRun.PlannedStartDateTimeUtc);

            jobManagementService.DisableTrigger(demoJob.Id, trigger.Id);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns().Where(r => r.State == ComponentModel.JobStorage.Model.JobRunStates.Deleted).ToList());

            var jobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(ComponentModel.JobStorage.Model.JobRunStates.Deleted, jobRun.State);
        }

        [TestMethod]
        public void JobRunIsScheduler_WhenTriggerIsEnabled_JobRunWillBeScheduled()
        {
            var jobManagementService = this.Services.JobManagementService;
            var storageProvider = this.Services.JobStorageProvider;

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var futureDate1 = DateTime.UtcNow.AddHours(2);

            var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = false };
            jobManagementService.AddTrigger(demoJob.Id, trigger);

            // Base asserts
            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();
            Assert.IsNull(createdJobRun, "There should be exact no JobRun");

            jobManagementService.EnableTrigger(demoJob.Id, trigger.Id);

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());

            var jobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(JobRunStates.Scheduled, jobRun.State);
        }
    }
}