using System;
using System.IO;
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

            this.Services.JobManagementService.UpdateTriggerStartTime(updatedTrigger.Id, updatedTrigger.StartDateTimeUtc);

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

        [TestMethod]
        public void JobRunWithArtefacts_WhenRetrieved_ReturnsAll()
        {
            // Arrange
            this.Services.ArtefactsStorageProvider.Save("1", "file1.txt", new MemoryStream());
            this.Services.ArtefactsStorageProvider.Save("1", "file3.txt", new MemoryStream());

            // Act
            var artefacts = this.Services.JobManagementService.GetArtefactForJob(1);

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(2, artefacts.Count);
        }

        [TestMethod]
        public void JobRunWithNoArtefacts_WhenRetrieved_ReturnsEmptyList()
        {

        }

        [TestMethod]
        public void JobRunWithArtefacts_GetByFileName_ReturnsSingle()
        {

        }

        [TestMethod]
        public void JobRunWithArtefacts_GetByNonExistentFileName_ReturnsSingle()
        {

        }

        [TestMethod]
        public void JobRunWithNoArtefacts_GetByFileName_ReturnsNull()
        {

        }
    }
}
