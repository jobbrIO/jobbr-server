using System;

using Jobbr.Common.Model;
using Jobbr.Server.Core;
using Jobbr.Tests.Setup;
using Jobbr.Tests.StorageProvider;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Core
{
    [TestClass]
    public class JobServiceTests
    {
        [TestMethod]
        public void JobService_ScheduledTriggerIsUpdated_UpdateIsPersisted()
        {
            var storageProvider = new InMemoryJobStorageProvider();
            var configuration = new CompleteJobberConfiguration();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var futureDate1 = DateTime.UtcNow.AddHours(2);
            var futureDate2 = DateTime.UtcNow.AddHours(5);

            var trigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };

            storageProvider.AddTrigger(trigger);

            var jobbrRepository = new JobbrRepository(storageProvider);
            var stateService = new StateService(configuration, jobbrRepository);
            var service = new JobManagementService(jobbrRepository, stateService);

            var triggerCopy = (ScheduledTrigger)storageProvider.GetTriggerById(trigger.Id).Clone();
            triggerCopy.StartDateTimeUtc = futureDate2;

            service.UpdateTrigger(triggerCopy.Id, triggerCopy);

            var assertTrigger = (ScheduledTrigger)storageProvider.GetTriggerById(trigger.Id).Clone();

            Assert.AreEqual(futureDate2, assertTrigger.StartDateTimeUtc);
        }

        [TestMethod]
        public void JobService_RecurringTriggerIsUpdated_UpdateIsPersisted()
        {
            var storageProvider = new InMemoryJobStorageProvider();
            var configuration = new CompleteJobberConfiguration();

            var demoJob = new Job();
            storageProvider.AddJob(demoJob);

            var definition1 = "1 1 1 1 1";
            var definition2 = "1 1 1 1 1";

            var trigger = new RecurringTrigger() { JobId = demoJob.Id, Definition = definition1, IsActive = true };

            storageProvider.AddTrigger(trigger);

            var jobbrRepository = new JobbrRepository(storageProvider);
            var stateService = new StateService(configuration, jobbrRepository);
            var service = new JobManagementService(jobbrRepository, stateService);

            var triggerCopy = (RecurringTrigger)storageProvider.GetTriggerById(trigger.Id).Clone();
            triggerCopy.Definition = definition2;

            service.UpdateTrigger(triggerCopy.Id, triggerCopy);

            var assertTrigger = (RecurringTrigger)storageProvider.GetTriggerById(trigger.Id).Clone();

            Assert.AreEqual(definition2, assertTrigger.Definition);
        }
    
    }
}
