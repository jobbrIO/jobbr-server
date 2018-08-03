using System;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.ComponentModel.Management;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Management
{
    [TestClass]
    public class JobQueryServiceTests : InitializedJobbrServerTestBase
    {
        public IQueryService QueryService => this.Services.QueryService;

        [TestMethod]
        public void HasNoJobs_QueryAllJobs_ShouldReturnEmptyList()
        {
            // Act
            var jobs = this.QueryService.GetJobs();

            // Assert
            Assert.IsNotNull(jobs);
            Assert.AreEqual(0, jobs.Items.Count);
        }

        [TestMethod]
        public void HasOneJob_QueryAllJobs_ShouldReturnOne()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJob(new Job());

            // Act
            var jobs = this.QueryService.GetJobs();

            // Assert
            Assert.AreEqual(1, jobs.Items.Count);
        }

        [TestMethod]
        public void HasOneJob_QueryJobByExistingId_ReturnsSingle()
        {
            // Arrange
            var job = new Job();
            this.Services.JobStorageProvider.AddJob(job);

            // Act
            var jobQueried = this.QueryService.GetJobById(job.Id);

            // Assert
            Assert.AreEqual(job.Id, jobQueried.Id);
        }

        [TestMethod]
        public void HasOneJobWithTrigger_QueryJobByExistingId_ReturnsJobWithTrigger()
        {
            // Arrange
            var job = new Job();
            this.Services.JobStorageProvider.AddJob(job);
            
            var trigger = new RecurringTrigger() { Definition = "* * * * *" };
            this.Services.JobStorageProvider.AddTrigger(job.Id, trigger);

            // Act
            var result = this.QueryService.GetJobById(job.Id);
            
        }

        [TestMethod]
        public void HasOneJob_QueryJobByWrongId_ReturnsNull()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJob(new Job { Id = 13 });

            // Act
            var job = this.QueryService.GetJobById(42);

            Assert.IsNull(job);
        }

        [TestMethod]
        public void HasOneJob_QueryJobByExistingUniqueName_ReturnsSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJob(new Job { UniqueName = "MyJob" });

            // Act
            var job = this.QueryService.GetJobByUniqueName("MyJob");

            Assert.AreEqual("MyJob", job.UniqueName);
        }

        [TestMethod]
        public void HasOneJob_QueryJobByNonexistingUniqueName_ReturnsNull()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJob(new Job { UniqueName = "MyJob" });

            // Act
            var job = this.QueryService.GetJobByUniqueName("haklkjdijl");

            Assert.IsNull(job);
        }

        [TestMethod]
        public void HasTriggerWithJobId_QueryByMatchingJobId_ReturnsListWithSingle()
        {
            // Arrange
            var instantTrigger = new InstantTrigger { IsActive = true };
            var recurringTrigger = new RecurringTrigger() { IsActive = true };
            var scheduledTrigger = new ScheduledTrigger { IsActive = true };

            this.Services.JobStorageProvider.AddTrigger(100, instantTrigger);
            this.Services.JobStorageProvider.AddTrigger(200, recurringTrigger);
            this.Services.JobStorageProvider.AddTrigger(300, scheduledTrigger);

            // Act
            var triggers = this.QueryService.GetTriggersByJobId(200, 1, 50).Items;
            var assertingRecurringTrigger = triggers[0] as ComponentModel.Management.Model.RecurringTrigger;

            // Test
            Assert.IsNotNull(triggers);
            Assert.AreEqual(1, triggers.Count);
            Assert.AreEqual(recurringTrigger.Id, assertingRecurringTrigger.Id);
        }

        [TestMethod]
        public void Recurring_Trigger_Properties_Mapped()
        {
            // Arrange
            var recurringTrigger = new RecurringTrigger()
            {
                IsActive = true,
                Definition = "* * * * *",
                StartDateTimeUtc = DateTime.UtcNow,
                EndDateTimeUtc = DateTime.UtcNow.AddHours(1),
                NoParallelExecution = true,
            };

            this.Services.JobStorageProvider.AddTrigger(1000, recurringTrigger);

            // Act
            var result = this.QueryService.GetTriggersByJobId(1000, 1, 50).Items[0] as ComponentModel.Management.Model.RecurringTrigger;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(recurringTrigger.Definition, result.Definition);
            Assert.AreEqual(recurringTrigger.StartDateTimeUtc, result.StartDateTimeUtc);
            Assert.AreEqual(recurringTrigger.EndDateTimeUtc, result.EndDateTimeUtc);
            Assert.AreEqual(recurringTrigger.NoParallelExecution, result.NoParallelExecution);
        }

        [TestMethod]
        public void HasDifferentTriggerTypes_QueryById_ReturnsCorrectType()
        {
            // Arrange
            const long jobId = 1;
            var instantTrigger = new InstantTrigger();
            var recurringTrigger = new RecurringTrigger();
            var scheduledTrigger = new ScheduledTrigger();
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger);
            this.Services.JobStorageProvider.AddTrigger(jobId, recurringTrigger);
            this.Services.JobStorageProvider.AddTrigger(jobId, scheduledTrigger);

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(jobId, instantTrigger.Id);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(jobId, recurringTrigger.Id);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(jobId, scheduledTrigger.Id);

            // Test
            Assert.AreEqual(typeof(ComponentModel.Management.Model.InstantTrigger), instantTypeTrigger.GetType());
            Assert.AreEqual(typeof(ComponentModel.Management.Model.RecurringTrigger), recurringTypeTrigger.GetType());
            Assert.AreEqual(typeof(ComponentModel.Management.Model.ScheduledTrigger), scheduledTypeTrigger.GetType());
        }


        [TestMethod]
        public void HasActiveTriggers_QueryActive_ReturnsAll()
        {
            const long jobId = 1;

            // Arrange
            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = true });

            // Act
            var triggers = this.QueryService.GetActiveTriggers();

            // Test
            Assert.AreEqual(3, triggers.Items.Count);
        }

        [TestMethod]
        public void HasActiveAndInactiveTriggers_QueryActive_ReturnsOnlyActive()
        {
            const long jobId = 1;

            // Arrange
            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = false });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = false });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = false });

            // Act
            var triggers = this.QueryService.GetActiveTriggers();

            // Test
            Assert.AreEqual(3, triggers.Items.Count);
        }

        [TestMethod]
        public void HasActiveTriggers_QueryById_ReturnsSingle()
        {
            const long jobId = 1;

            // Arrange
            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = true });

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(jobId, 1);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(jobId, 2);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(jobId, 3);

            // Test
            Assert.IsNotNull(instantTypeTrigger);
            Assert.IsNotNull(recurringTypeTrigger);
            Assert.IsNotNull(scheduledTypeTrigger);

            Assert.AreEqual(1, instantTypeTrigger.Id);
            Assert.AreEqual(2, recurringTypeTrigger.Id);
            Assert.AreEqual(3, scheduledTypeTrigger.Id);
        }

        [TestMethod]
        public void HasInactiveTriggers_QueryById_ReturnsSingle()
        {
            const long jobId = 1;

            // Arrange
            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = false });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = false });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = false });

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(jobId, 1);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(jobId, 2);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(jobId, 3);

            // Test
            Assert.IsNotNull(instantTypeTrigger);
            Assert.IsNotNull(recurringTypeTrigger);
            Assert.IsNotNull(scheduledTypeTrigger);

            Assert.AreEqual(1, instantTypeTrigger.Id);
            Assert.AreEqual(2, recurringTypeTrigger.Id);
            Assert.AreEqual(3, scheduledTypeTrigger.Id);
        }

        [TestMethod]
        public void HasActiveTriggers_QueryByInExistentId_ReturnsNull()
        {
            const long jobId = 1;

            this.Services.JobStorageProvider.AddTrigger(jobId, new InstantTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new RecurringTrigger { IsActive = true });
            this.Services.JobStorageProvider.AddTrigger(jobId, new ScheduledTrigger { IsActive = true });

            // Act
            var trigger = this.QueryService.GetTriggerById(jobId, 42);

            Assert.IsNull(trigger);
        }

        [TestMethod]
        public void HasNoJobRuns_QueryAll_ShouldReturnEmptyList()
        {
            // Act
            var jobs = this.QueryService.GetJobRuns();

            // Assert
            Assert.IsNotNull(jobs);
            Assert.AreEqual(0, jobs.Items.Count);

        }

        [TestMethod]
        public void HasOneJobRun_QueryAll_ShouldReturnOne()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var jobs = this.QueryService.GetJobRuns();

            // Assert
            Assert.AreEqual(1, jobs.Items.Count);
        }

        [TestMethod]
        public void HasOneJobRun_QueryJobByExistingId_ReturnsSingle()
        {
            // Arrange
            var jobRunToAdd = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRunToAdd);

            // Act
            var jobRun = this.QueryService.GetJobRunById(jobRunToAdd.Id);

            // Assert
            Assert.AreEqual(jobRunToAdd.Id, jobRun.Id);
        }

        [TestMethod]
        public void HasOneJobRun_QueryJobByWrongId_ReturnsNull()
        {
            // Arrange
            var jobRun = new JobRun();
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var job = this.QueryService.GetJobRunById(42);

            Assert.IsNull(job);
        }

        [TestMethod]
        public void HasOneJobRun_QueryByExistingTriggerId_ReturnsListWithSingle()
        {
            // Arrange
            var job = new Job { Id = 1337 };
            var trigger = new ScheduledTrigger { Id = 34 };
            
            var jobRun = new JobRun { Job = job, Trigger = trigger };
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var runs = this.QueryService.GetJobRunsByTriggerId(1337, 34);

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Items.Count);
            Assert.AreEqual(jobRun.Id, runs.Items[0].Id);
            Assert.AreEqual(34, runs.Items[0].TriggerId);
            Assert.AreEqual(1337, runs.Items[0].JobId);
        }

        [TestMethod]
        public void HasOneJobRun_QueryByInExistentTriggerId_ReturnsEmptyList()
        {
            // Arrange
            var job = new Job { Id = 1000 };
            var trigger = new ScheduledTrigger { Id = 34 };
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Job = job, Trigger = trigger });

            // Act
            var runs = this.QueryService.GetJobRunsByTriggerId(-1, -1);

            Assert.IsNotNull(runs);
            Assert.AreEqual(0, runs.Items.Count);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserId_ReturnsListWithSingle()
        {
            const long jobId = 1;

            // Arrange
            var job = new Job { Id = jobId };
            var instantTrigger = new InstantTrigger { UserId = "45" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger);

            var jobRun = new JobRun { Trigger = instantTrigger, Job = job };
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var runs = this.QueryService.GetJobRunsByUserId("45");

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Items.Count);
            Assert.AreEqual(jobRun.Id, runs.Items[0].Id);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByInexistentUserId_ReturnsEmptyList()
        {
            const long jobId = 1;

            // Arrange
            var job = new Job { Id = jobId };
            var instantTrigger = new InstantTrigger { UserId = "45" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger);
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Trigger = instantTrigger, Job = job });

            // Act
            var runs = this.QueryService.GetJobRunsByUserId("88");

            Assert.IsNotNull(runs);
            Assert.AreEqual(0, runs.Items.Count);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserId_ReturnsSortedListByIdDesc()
        {
            const long jobId = 1;

            // Arrange
            var job = new Job { Id = jobId };
            var instantTrigger1 = new InstantTrigger { UserId = "45" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger1);

            var instantTrigger2 = new InstantTrigger { UserId = "45" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger2);

            var jobRun1 = new JobRun { Job = job, Trigger = instantTrigger1 };
            this.Services.JobStorageProvider.AddJobRun(jobRun1);

            var jobRun2 = new JobRun { Job = job, Trigger = instantTrigger2 };
            this.Services.JobStorageProvider.AddJobRun(jobRun2);

            // Act
            var runs = this.QueryService.GetJobRunsByUserId("45");

            Assert.IsNotNull(runs);
            Assert.AreEqual(2, runs.Items.Count);
            Assert.AreEqual(jobRun2.Id, runs.Items[0].Id);
            Assert.AreEqual(jobRun1.Id, runs.Items[1].Id);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserName_ReturnsListWithSingle()
        {
            const long jobId = 1;
            // Arrange
            var instantTrigger = new InstantTrigger { UserDisplayName = "hans" };
            var job = new Job { Id = jobId };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger);

            this.Services.JobStorageProvider.AddJobRun(new JobRun { Job = job, Trigger = instantTrigger });

            // Act
            var runs = this.QueryService.GetJobRunsByUserDisplayName("hans");

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Items.Count);
            Assert.AreEqual(instantTrigger.Id, runs.Items[0].TriggerId);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByInexistentUserName_ReturnsEmptyList()
        {
            const long jobId = 1;

            // Arrange
            var instantTrigger = new InstantTrigger { UserDisplayName = "hans" };
            var job = new Job { Id = jobId };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger);

            var jobRun = new JobRun { Job = job, Trigger = instantTrigger };
            this.Services.JobStorageProvider.AddJobRun(jobRun);

            // Act
            var runs = this.QueryService.GetJobRunsByUserDisplayName("blablablabl");

            Assert.IsNotNull(runs);
            Assert.AreEqual(0, runs.Items.Count);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserName_ReturnsSortedListByIdDesc()
        {
            const long jobId = 1;

            // Arrange
            var instantTrigger1 = new InstantTrigger { UserDisplayName = "hans" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger1);

            var instantTrigger2 = new InstantTrigger { UserDisplayName = "hans" };
            this.Services.JobStorageProvider.AddTrigger(jobId, instantTrigger2);

            var jobRun1 = new JobRun { Trigger = instantTrigger1 };
            this.Services.JobStorageProvider.AddJobRun(jobRun1);

            var jobRun2 = new JobRun { Trigger = instantTrigger2 };
            this.Services.JobStorageProvider.AddJobRun(jobRun2);

            // Act
            var runs = this.QueryService.GetJobRunsByUserDisplayName("hans");

            Assert.IsNotNull(runs);
            Assert.AreEqual(2, runs.Items.Count);
            Assert.AreEqual(jobRun2.Id, runs.Items[0].Id);
            Assert.AreEqual(jobRun1.Id, runs.Items[1].Id);
        }
    }
}
