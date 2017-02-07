using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Execution
{
    [TestClass]
    public class DefaultJobExecutorTests
    {
        [TestMethod]
        public void PreparedJobRun_IsUpdated_PreparedStartDateIsAdjusted()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var storageProvider = new InMemoryJobStorageProvider();

            //var demoJob = new Job();
            //storageProvider.AddJob(demoJob);

            //var configuration = new CompleteJobberConfiguration();

            //var jobbrRepository = new JobbrRepository(storageProvider);
            //var stateService = new StateService(configuration, jobbrRepository);

            //var jobService = new JobManagementService(jobbrRepository, stateService);

            //var scheduler = new OldScheduler(stateService, configuration, jobbrRepository, jobService);

            //var futureDate1 = DateTime.UtcNow.AddHours(2);
            //var futureDate2 = DateTime.UtcNow.AddHours(4);

            //var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };

            //var triggerId = jobService.AddTrigger(trigger);
            //scheduler.Start();

            //// Wait for the scheduler to do his work
            //var hasJobRuns = WaitFor.HasElements(() => storageProvider.GetJobRuns());

            //Assert.IsTrue(hasJobRuns, "There should be at least one element after a given amount of time");

            //var trigger2 = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate2, IsActive = true };

            //jobService.UpdateTrigger(triggerId, trigger2);

            //var jobRunFromStorage = storageProvider.GetJobRuns().Single();

            //Assert.AreEqual(trigger2.StartDateTimeUtc, jobRunFromStorage.PlannedStartDateTimeUtc);
        }

        [TestMethod]
        public void PreparedJobRun_IsDeleted_WhenCorrespondingTriggerWasDeactivated()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var storageProvider = new InMemoryJobStorageProvider();

            //var demoJob = new Job();
            //storageProvider.AddJob(demoJob);

            //var configuration = new CompleteJobberConfiguration();

            //var jobbrRepository = new JobbrRepository(storageProvider);
            //var stateService = new StateService(configuration, jobbrRepository);

            //var jobService = new JobManagementService(jobbrRepository, stateService);

            //var scheduler = new OldScheduler(stateService, configuration, jobbrRepository, jobService);

            //var futureDate1 = DateTime.UtcNow.AddHours(2);

            //var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };

            //var triggerId = jobService.AddTrigger(trigger);

            //scheduler.Start();

            //// Wait for the scheduler to do his work
            //var hasJobRuns = WaitFor.HasElements(() => storageProvider.GetJobRunsByState(JobRunState.Scheduled));

            //// Precondition
            //Assert.IsTrue(hasJobRuns, "There should be at least one element after a given amount of time");

            //jobService.DisableTrigger(triggerId);

            //// Wait for the scheduler to do his work
            //var hasNoElements = WaitFor.HasZeroElements(() => storageProvider.GetJobRunsByState(JobRunState.Scheduled));

            //Assert.IsTrue(hasNoElements, "Jobrun should be removed when disabled corresponding trigger");
        }
    }
}
