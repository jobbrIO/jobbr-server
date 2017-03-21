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
            var jobs = this.QueryService.GetAllJobs();

            // Assert
            Assert.IsNotNull(jobs);
            Assert.AreEqual(0, jobs.Count);
        }

        [TestMethod]
        public void HasOneJob_QueryAllJobs_ShouldReturnOne()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJob(new Job());

            // Act
            var jobs = this.QueryService.GetAllJobs();

            // Assert
            Assert.AreEqual(1, jobs.Count);
        }

        [TestMethod]
        public void HasOneJob_QueryJobByExistingId_ReturnsSingle()
        {
            // Arrange
            var id = this.Services.JobStorageProvider.AddJob(new Job());

            // Act
            var job = this.QueryService.GetJobById(id);

            // Assert
            Assert.AreEqual(id, job.Id);
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
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1, JobId = 100});
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2, JobId = 200});
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3, JobId = 300});

            // Act
            var triggers = this.QueryService.GetTriggersByJobId(200);

            // Test
            Assert.IsNotNull(triggers);
            Assert.AreEqual(1, triggers.Count);
            Assert.AreEqual(2, triggers[0].Id);
        }

        [TestMethod]
        public void HasDifferentTriggerTypes_QueryById_ReturnsCurrectType()
        {
            // Arrange
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3 });

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(1);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(2);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(3);

            // Test
            Assert.AreEqual(typeof(ComponentModel.Management.Model.InstantTrigger), instantTypeTrigger.GetType());
            Assert.AreEqual(typeof(ComponentModel.Management.Model.RecurringTrigger), recurringTypeTrigger.GetType());
            Assert.AreEqual(typeof(ComponentModel.Management.Model.ScheduledTrigger), scheduledTypeTrigger.GetType());
        }


        [TestMethod]
        public void HasActiveTriggers_QueryActive_ReturnsAll()
        {
            // Arrange
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3 });

            // Act
            var triggers = this.QueryService.GetActiveTriggers();

            // Test
            Assert.AreEqual(3, triggers.Count);
        }

        [TestMethod]
        public void HasActiveAndInactiveTriggers_QueryActive_ReturnsOnlyActive()
        {
            // Arrange
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3 });
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = false, Id = 4 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = false, Id = 5 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = false, Id = 6 });

            // Act
            var triggers = this.QueryService.GetActiveTriggers();

            // Test
            Assert.AreEqual(3, triggers.Count);
        }

        [TestMethod]
        public void HasActiveTriggers_QueryById_ReturnsSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3 });

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(1);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(2);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(3);

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
            // Arrange
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = false, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = false, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = false, Id = 3 });

            // Act
            var instantTypeTrigger = this.QueryService.GetTriggerById(1);
            var recurringTypeTrigger = this.QueryService.GetTriggerById(2);
            var scheduledTypeTrigger = this.QueryService.GetTriggerById(3);

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
            this.Services.JobStorageProvider.AddTrigger(new InstantTrigger() { IsActive = true, Id = 1 });
            this.Services.JobStorageProvider.AddTrigger(new RecurringTrigger() { IsActive = true, Id = 2 });
            this.Services.JobStorageProvider.AddTrigger(new ScheduledTrigger() { IsActive = true, Id = 3 });

            // Act
            var trigger = this.QueryService.GetTriggerById(42);

            Assert.IsNull(trigger);
        }

        [TestMethod]
        public void HasNoJobRuns_QueryAll_ShouldReturnEmptyList()
        {
            // Act
            var jobs = this.QueryService.GetJobRuns();

            // Assert
            Assert.IsNotNull(jobs);
            Assert.AreEqual(0, jobs.Count);

        }

        [TestMethod]
        public void HasOneJobRun_QueryAll_ShouldReturnOne()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98 });

            // Act
            var jobs = this.QueryService.GetJobRuns();

            // Assert
            Assert.AreEqual(1, jobs.Count);
        }

        [TestMethod]
        public void HasOneJobRun_QueryJobByExistingId_ReturnsSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98 });

            // Act
            var jobRun = this.QueryService.GetJobRunById(98);

            // Assert
            Assert.AreEqual(98, jobRun.Id);
        }

        [TestMethod]
        public void HasOneJobRun_QueryJobByWrongId_ReturnsNull()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98 });

            // Act
            var job = this.QueryService.GetJobRunById(42);

            Assert.IsNull(job);
        }

        [TestMethod]
        public void HasOneJobRun_QueryByExistingTriggerId_ReturnsListWithSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, TriggerId = 34 });

            // Act
            var runs = this.QueryService.GetJobRunsByTriggerId(34);

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual(98, runs[0].Id);
        }

        [TestMethod]
        public void HasOneJobRun_QueryByInExistentTriggerId_ReturnsEmptyList()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, TriggerId = 34 });

            // Act
            var runs = this.QueryService.GetJobRunsByTriggerId(-1);

            Assert.IsNotNull(runs);
            Assert.AreEqual(0, runs.Count);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserId_ReturnsListWithSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserId = 45 });

            // Act
            var runs = this.QueryService.GetJobRunsByUserIdOrderByIdDesc(45);

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual(45, runs[0].UserId);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByInexistentUserId_ReturnsEmptyList()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserId = 45 });

            // Act
            var runs = this.QueryService.GetJobRunsByUserIdOrderByIdDesc(45);

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual(45, runs[0].UserId);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserId_ReturnsSortedListByIdDesc()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserId = 45 });
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 65, UserId = 45 });

            // Act
            var runs = this.QueryService.GetJobRunsByUserIdOrderByIdDesc(45);

            Assert.IsNotNull(runs);
            Assert.AreEqual(2, runs.Count);
            Assert.AreEqual(65, runs[0].Id);
            Assert.AreEqual(98, runs[1].Id);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserName_ReturnsListWithSingle()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserName = "hans" });

            // Act
            var runs = this.QueryService.GetJobRunsByUserNameOrderByIdDesc("hans");

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual(45, runs[0].UserId);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByInexistentUserName_ReturnsEmptyList()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserName = "hans" });

            // Act
            var runs = this.QueryService.GetJobRunsByUserNameOrderByIdDesc("hans");

            Assert.IsNotNull(runs);
            Assert.AreEqual(1, runs.Count);
            Assert.AreEqual(45, runs[0].UserId);
        }

        [TestMethod]
        public void HasOneMatchingJobRun_QueryJobByUserName_ReturnsSortedListByIdDesc()
        {
            // Arrange
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 98, UserName = "hans" });
            this.Services.JobStorageProvider.AddJobRun(new JobRun { Id = 65, UserName = "hans" });

            // Act
            var runs = this.QueryService.GetJobRunsByUserNameOrderByIdDesc("hans");

            Assert.IsNotNull(runs);
            Assert.AreEqual(2, runs.Count);
            Assert.AreEqual(65, runs[0].Id);
            Assert.AreEqual(98, runs[1].Id);
        }
    }
}
