using System;
using System.Collections.Generic;

using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Tests.Setup;
using Jobbr.Tests.StorageProvider;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Execution
{
    [TestClass]
    public class DefaultJobExecutorTests
    {
        [TestMethod]
        public void PreparedJobRun_IsUpdated_PreparedStartDateIsAdjusted()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);

            var executor = new DefaultJobExecutor(jobService, new CompleteJobberConfiguration());

            var futureDate1 = DateTime.UtcNow.AddHours(2);
            var futureDate2 = DateTime.UtcNow.AddHours(4);

            var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };

            var jobRunId = jobService.CreateJobRun(demoJob, trigger, trigger.StartDateTimeUtc);
            var jobRun = jobService.GetJobRun(jobRunId);

            // Wait for the scheduler to do his work
            var hasJobRuns = WaitFor.HasElements(() => storageProvider.GetJobRuns());

            Assert.IsTrue(hasJobRuns, "There should be at least one element after a given amount of time");

            executor.Start();

            jobRun.PlannedStartDateTimeUtc = futureDate2;

            jobService.UpdatePlannedStartDate(jobRun);
        }

        [TestMethod]
        public void PreparedJobRun_IsDeleted_WhenCorrespondingTriggerWasDeactivated()
        {
            var storageProvider = new InMemoryJobStorageProvider();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var jobService = new JobService(storageProvider);

            var executor = new DefaultJobExecutor(jobService, new CompleteJobberConfiguration());

            var futureDate1 = DateTime.UtcNow.AddHours(2);

            var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };

            jobService.AddTrigger(trigger);
            var jobRunId = jobService.CreateJobRun(demoJob, trigger, trigger.StartDateTimeUtc);

            // Wait for the scheduler to do his work
            var hasJobRuns = WaitFor.HasElements(() => storageProvider.GetJobRuns());

            Assert.IsTrue(hasJobRuns, "There should be at least one element after a given amount of time");

            executor.Start();

            jobService.DisableTrigger(jobRunId);
        }
    }
}
