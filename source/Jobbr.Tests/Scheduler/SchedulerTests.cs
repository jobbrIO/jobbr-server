using System;
using System.Linq;

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
        public void JobRunIsScheduled_TriggerGetsUpdated_JobRunScheduleIsAdjusted()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);
            
            var scheduler = new DefaultScheduler(jobService, new CompleteJobberConfiguration());

            var currentMinutesInHour = DateTime.UtcNow.Minute;
            var futureMinute = (currentMinutesInHour + 10) % 60;

            var recurringTrigger = new RecurringTrigger() { Definition = futureMinute + " * * * *", JobId = demoJob.Id, IsActive = true};
            jobService.AddTrigger(recurringTrigger);
            scheduler.Start();

            // Wait for the scheduler to do his work
            WaitFor.HasElements(() => storageProvider.GetJobRuns());
            
            var createdJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            // Base Assertions
            Assert.IsNotNull(createdJobRun, "There should be exact one JobRun which is not null");
            Assert.IsTrue(createdJobRun.PlannedStartDateTimeUtc >= DateTime.UtcNow, "The job run needs to be in the future");
            Assert.AreEqual(futureMinute, createdJobRun.PlannedStartDateTimeUtc.Minute);

            var futureMinute2 = (futureMinute + 2) % 60;

            jobService.UpdateTrigger(recurringTrigger.Id, new RecurringTrigger{ Definition = futureMinute2 + " * * * *" });

            var updatedJobRun = storageProvider.GetJobRuns().FirstOrDefault();

            Assert.AreEqual(futureMinute2, updatedJobRun.PlannedStartDateTimeUtc.Minute, "As per updated definition, the job should now start on every your @ 00-mins");
        }
    }
}
