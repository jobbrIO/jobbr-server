using System;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Management
{
    [TestClass]
    public class JobManagementTests : InitializedJobbrServerTestBase
    {
        [TestMethod]
        public void JobService_ExistingScheduledTriggerIsUpdated_UpdateIsPersisted()
        {
            var demoJob = new Job();
            var storageProvider = this.Services.JobStorageProvider;
            storageProvider.AddJob(demoJob);

            var futureDate1 = DateTime.UtcNow.AddHours(2);
            var futureDate2 = DateTime.UtcNow.AddHours(5);

            var initialTrigger = new ScheduledTrigger { JobId = demoJob.Id, StartDateTimeUtc = futureDate1, IsActive = true };
            storageProvider.AddTrigger(initialTrigger);

            var updatedTrigger = new ScheduledTrigger { Id = initialTrigger.Id, JobId = demoJob.Id, StartDateTimeUtc = futureDate2, IsActive = true };

            this.Services.JobManagementService.UpdatetriggerStartTime(updatedTrigger.Id, updatedTrigger.StartDateTimeUtc);

            var assertTrigger = (ScheduledTrigger)storageProvider.GetTriggerById(initialTrigger.Id);

            Assert.AreEqual(futureDate2, assertTrigger.StartDateTimeUtc);
        }

        [TestMethod]
        public void JobService_ExistingRecurringTriggerIsUpdated_UpdateIsPersisted()
        {
            var demoJob = new Job();
            var storageProvider = this.Services.JobStorageProvider;
            storageProvider.AddJob(demoJob);

            var definition1 = "1 1 1 1 1";
            var definition2 = "1 1 1 1 3";

            var initialTrigger = new RecurringTrigger() { JobId = demoJob.Id, Definition = definition1, IsActive = true };
            storageProvider.AddTrigger(initialTrigger);

            var updatedTrigger = new RecurringTrigger() { Id = initialTrigger.Id, JobId = demoJob.Id, Definition = definition2, IsActive = true };

            this.Services.JobManagementService.UpdateTriggerDefinition(updatedTrigger.Id, updatedTrigger.Definition);

            var assertTrigger = (RecurringTrigger)storageProvider.GetTriggerById(initialTrigger.Id);

            Assert.AreEqual(definition2, assertTrigger.Definition);
        }
    }
}
