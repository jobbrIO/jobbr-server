using System;
using System.IO;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration.Management
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
            storageProvider.AddTrigger(demoJob.Id, initialTrigger);

            var updatedTrigger = new ScheduledTrigger { Id = initialTrigger.Id, JobId = demoJob.Id, StartDateTimeUtc = futureDate2, IsActive = true };

            this.Services.JobManagementService.UpdateTriggerStartTime(demoJob.Id, updatedTrigger.Id, updatedTrigger.StartDateTimeUtc);

            var assertTrigger = (ScheduledTrigger)storageProvider.GetTriggerById(demoJob.Id, initialTrigger.Id);

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

            var initialTrigger = new RecurringTrigger { JobId = demoJob.Id, Definition = definition1, IsActive = true };
            storageProvider.AddTrigger(demoJob.Id, initialTrigger);

            var updatedTrigger = new RecurringTrigger { Id = initialTrigger.Id, JobId = demoJob.Id, Definition = definition2, IsActive = true };

            this.Services.JobManagementService.UpdateTriggerDefinition(demoJob.Id, updatedTrigger.Id, updatedTrigger.Definition);

            var assertTrigger = (RecurringTrigger)storageProvider.GetTriggerById(demoJob.Id, initialTrigger.Id);

            Assert.AreEqual(definition2, assertTrigger.Definition);
        }

        [TestMethod]
        public void JobRunWithArtefacts_WhenRetrieved_ReturnsAll()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file1.txt", new MemoryStream());
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file3.txt", new MemoryStream());

            // Act
            var artefacts = this.Services.JobManagementService.GetArtefactForJob(1);

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(2, artefacts.Count);
        }

        [TestMethod]
        public void JobRunWithNoArtefacts_WhenRetrieved_ReturnsEmptyList()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var artefacts = this.Services.JobManagementService.GetArtefactForJob(jobRun.Id);

            Assert.IsNotNull(artefacts);
            Assert.AreEqual(0, artefacts.Count);
        }

        [TestMethod]
        public void JobRunWithArtefacts_GetByFileName_ReturnsSingle()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file1.txt", new MemoryStream());
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file3.txt", new MemoryStream());

            // Act
            var artefact = this.Services.JobManagementService.GetArtefactAsStream(jobRun.Id, "file3.txt");

            Assert.IsNotNull(artefact);
        }

        [TestMethod]
        public void JobRunWithNoArtefacts_GetByFileName_ReturnsNull()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var artefact = this.Services.JobManagementService.GetArtefactAsStream(jobRun.Id, "file162z7.txt");

            Assert.IsNull(artefact);
        }

        [TestMethod]
        public void JobRunWithArtefacts_GetByUnknownFileName_ReturnsNull()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file1.txt", new MemoryStream());
            this.Services.ArtefactsStorageProvider.Save(jobRun.Id.ToString(), "file3.txt", new MemoryStream());

            // Act
            var artefact = this.Services.JobManagementService.GetArtefactAsStream(jobRun.Id, "file162z7.txt");

            Assert.IsNull(artefact);
        }
    }
}
