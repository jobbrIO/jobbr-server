using System;
using System.Data;
using System.Linq;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Jobbr.Server.UnitTests.Storage
{
    [TestClass]
    public class InMemoryJobStorageProviderTests
    {
        private readonly IJobStorageProvider _provider;

        public InMemoryJobStorageProviderTests()
        {
            _provider = new InMemoryJobStorageProvider();
        }

        [TestMethod]
        public void AddJob_ShouldSetId()
        {
            // Arrange
            var jobs = Enumerable.Repeat(new Job(), 4).ToList();

            // Act
            jobs.ForEach(job => _provider.AddJob(job));

            // Assert
            jobs.ShouldAllBe(job => job.Id != 0);
            _provider.GetJobsCount().ShouldBe(jobs.Count);
        }

        [Ignore]
        [TestMethod]
        public void AddJob_NonUniqueName_ShouldThrow()
        {
            // Arrange
            var identifier = "my-unique-identifier";
            var job = new Job { UniqueName = identifier };
            _provider.AddJob(new Job { UniqueName = identifier });

            // Act & Assert
            Should.Throw<DuplicateNameException>(() => _provider.AddJob(job));
        }

        [TestMethod]
        public void AddTrigger_RecurringTrigger_ShouldSetIdAndJobId()
        {
            // Arrange
            var jobId = 1;
            var trigger = new RecurringTrigger();

            // Act
            _provider.AddTrigger(jobId, trigger);

            // Assert
            trigger.JobId.ShouldBe(jobId);
            trigger.Id.ShouldBe(1);
        }

        [TestMethod]
        public void AddTrigger_InstantTrigger_ShouldSetIdAndJobId()
        {
            // Arrange
            var jobId = 1;
            var trigger = new InstantTrigger();

            // Act
            _provider.AddTrigger(jobId, trigger);

            // Assert
            trigger.JobId.ShouldBe(jobId);
            trigger.Id.ShouldBe(1);
        }

        [TestMethod]
        public void AddTrigger_ScheduledTrigger_ShouldSetIdAndJobId()
        {
            // Arrange
            var jobId = 1;
            var trigger = new ScheduledTrigger();

            // Act
            _provider.AddTrigger(jobId, trigger);

            // Assert
            trigger.JobId.ShouldBe(jobId);
            trigger.Id.ShouldBe(1);
        }

        [TestMethod]
        public void AddJobRun_ShouldSetId()
        {
            // Arrange
            var jobRun1 = new JobRun();
            var jobRun2 = new JobRun();

            // Act
            _provider.AddJobRun(jobRun1);
            _provider.AddJobRun(jobRun2);

            // Assert
            jobRun1.Id.ShouldBe(1);
            jobRun2.Id.ShouldBe(2);
        }

        [TestMethod]
        public void GetJobById_ShouldReturnClone()
        {
            // Arrange
            var job = new Job();
            _provider.AddJob(job);

            // Act
            var result = _provider.GetJobById(job.Id);

            // Assert
            result.ShouldNotBeNull().ShouldNotBe(job);
        }

        [TestMethod]
        public void GetJobByUniqueName_ShouldReturnClone()
        {
            // Arrange
            var identifier = "my-unique-identifier";
            var job = new Job { UniqueName = identifier };
            _provider.AddJob(job);

            // Act
            var result = _provider.GetJobByUniqueName(identifier);

            // Assert
            result.ShouldNotBeNull().ShouldNotBe(job);
            result.UniqueName.ShouldBe(identifier);
        }

        [TestMethod]
        public void GetTriggerById_ShouldReturnClone()
        {
            // Arrange
            long jobId = 1, searchJobId = 2;
            var trigger = new RecurringTrigger();
            _provider.AddTrigger(jobId, trigger);

            // Act
            var result = _provider.GetTriggerById(searchJobId, trigger.Id);

            // Assert
            result.ShouldNotBeNull().ShouldNotBe(trigger);
            result.ShouldBeOfType<RecurringTrigger>();
            result.JobId.ShouldBe(jobId);
        }

        [TestMethod]
        public void GetJobRunById_ShouldReturnClone()
        {
            // Arrange
            var jobRun = new JobRun();
            _provider.AddJobRun(jobRun);

            // Act
            var result = _provider.GetJobRunById(jobRun.Id);

            // Assert
            result.ShouldNotBeNull().ShouldNotBe(jobRun);
        }

        [TestMethod]
        public void DisableTrigger_ShouldSetIsActive()
        {
            // Arrange
            var jobId = 1;
            var trigger = new RecurringTrigger { IsActive = true };
            _provider.AddTrigger(jobId, trigger);

            // Act
            _provider.DisableTrigger(jobId, trigger.Id);

            // Assert
            trigger.IsActive.ShouldBeFalse();
        }

        [TestMethod]
        public void DisableTrigger_BadId_ShouldThrow()
        {
            // Arrange
            var jobId = 1; ;
            var trigger = new RecurringTrigger { IsActive = true };
            _provider.AddTrigger(jobId, trigger);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => _provider.DisableTrigger(jobId, trigger.Id + 1));
        }

        [TestMethod]
        public void EnableTrigger_ShouldSetIsActive()
        {
            // Arrange
            var jobId = 1;
            var trigger = new RecurringTrigger { IsActive = false };
            _provider.AddTrigger(jobId, trigger);

            // Act
            _provider.EnableTrigger(jobId, trigger.Id);

            // Assert
            trigger.IsActive.ShouldBeTrue();
        }

        [TestMethod]
        public void EnableTrigger_BadId_ShouldThrow()
        {
            // Arrange
            var jobId = 1; ;
            var trigger = new RecurringTrigger { IsActive = false };
            _provider.AddTrigger(jobId, trigger);

            // Act & Assert
            Should.Throw<InvalidOperationException>(() => _provider.EnableTrigger(jobId, trigger.Id + 1));
        }

        [TestMethod]
        public void Update_Job_ShouldReplaceExisting()
        {
            // Arrange
            var job = new Job();
            _provider.AddJob(job);

            var jobUpdate = new Job { Id = job.Id };
            var identifier = "my-unique-identifier";
            jobUpdate.UniqueName = identifier;
            var jobCount = _provider.GetJobsCount();

            // Act
            _provider.Update(jobUpdate);

            // Assert
            _provider.GetJobsCount().ShouldBe(jobCount);
            job = _provider.GetJobById(job.Id);
            job.UniqueName.ShouldBe(jobUpdate.UniqueName);
        }

        [Ignore]
        [TestMethod]
        public void Update_NewJob_ShouldSetIdOrThrow()
        {
            // Arrange
            var job = new Job { Id = 4 };

            // Act
            _provider.Update(job);

            // Assert
            job.Id.ShouldBe(1);
        }

        [TestMethod]
        public void Update_JobRun_ShouldReplaceExisting()
        {
            // Arrange
            var jobRun = new JobRun();
            _provider.AddJobRun(jobRun);

            var jobRunUpdate = new JobRun { Id = jobRun.Id };
            var jobParameters = "{ key: 2, value: \"test\"}";
            jobRunUpdate.JobParameters = jobParameters;

            // Act
            _provider.Update(jobRunUpdate);

            // Assert
            jobRun = _provider.GetJobRunById(jobRun.Id);
            jobRun.JobParameters.ShouldBe(jobRunUpdate.JobParameters);
        }

        [Ignore]
        [TestMethod]
        public void Update_NewJobRun_ShouldSetIdOrThrow()
        {
            // Arrange
            var jobRun = new JobRun { Id = 4 };

            // Act
            _provider.Update(jobRun);

            // Assert
            jobRun.Id.ShouldBe(1);
        }

        [TestMethod]
        public void UpdateProgress_ShouldSetProgress()
        {
            // Arrange
            var jobRun = new JobRun { Progress = 1 };
            _provider.AddJobRun(jobRun);

            // Act
            _provider.UpdateProgress(jobRun.Id, 2);

            // Assert
            jobRun.Progress.ShouldBe(2);
        }

        [TestMethod]
        public void UpdateProgress_NotFound_ShouldThrow()
        {
            // Act & Assert
            Should.Throw<InvalidOperationException>(() => _provider.UpdateProgress(2, 2));
        }

        [TestMethod]
        public void IsAvailable_ShouldReturnTrue()
        {
            // Act
            var result = _provider.IsAvailable();

            // Assert
            result.ShouldBeTrue();
        }

        [TestMethod]
        public void GetJobsCount_ShouldReturnCount()
        {
            // Arrange
            var jobs = Enumerable.Range(0, 4).Select(x => new Job()).ToList();
            jobs.ForEach(job => _provider.AddJob(job));

            _provider.Update(new Job { Id = 2 }); // Update an existing job -> count remains the same
            _provider.Update(new Job { Id = 128 }); // Update a new job -> count increases

            // Act
            var result = _provider.GetJobsCount();

            // Assert
            result.ShouldBe(jobs.Count + 1);
        }

        [TestMethod]
        public void GetJobs_DefaultSearch_ShouldReturnsPagedList()
        {
            // Arrange
            var jobs = Enumerable.Range(0, 127).Select(x => new Job()).ToList();
            jobs.ForEach(job => _provider.AddJob(job));

            // Act
            var result = _provider.GetJobs();

            // Assert
            result.Items.Count.ShouldBe(50);
            result.TotalItems.ShouldBe(jobs.Count);
            result.Items.ShouldBeSubsetOf(jobs);
        }

        [TestMethod]
        public void GetJobs_UniqueNameFilter_ShouldReturnsPagedList()
        {
            // Arrange
            var jobs = Enumerable.Range(0, 127).Select(x => new Job { UniqueName = $"unique-name-{x}" }).ToList();
            jobs.ForEach(job => _provider.AddJob(job));

            // Act
            var result = _provider.GetJobs(jobUniqueNameFilter: "unique-name-4");

            // Assert
            result.Items.Count.ShouldBe(1);
            result.TotalItems.ShouldBe(1);
        }

        [TestMethod]
        public void GetJobRunsByUserId_ShouldReturnPagedList()
        {
            // Arrange
            var userId = "Test User";
            var jobRuns = Enumerable.Range(0, 127).Select(x => new JobRun { Trigger = new InstantTrigger { UserId = userId } }).ToList();
            var otherJobRuns = Enumerable.Range(0, 127).Select(x => new JobRun { Trigger = new RecurringTrigger() }).ToList();
            jobRuns.ForEach(jobRun => _provider.AddJobRun(jobRun));
            otherJobRuns.ForEach(jobRun => _provider.AddJobRun(jobRun));

            // Act
            var result = _provider.GetJobRunsByUserId(userId);

            // Assert
            result.ShouldNotBeNull();
            result.Items.Count.ShouldBe(50);
            result.TotalItems.ShouldBe(jobRuns.Count);
        }

        [TestMethod]
        public void GetJobRunsByStates_ShouldReturnPagedList()
        {
            // Arrange
            var allStates = Enum.GetValues<JobRunStates>();
            var jobRuns = Enumerable.Range(0, 127).Select(x => new JobRun { State = allStates[x % allStates.Length] }).ToList();
            jobRuns.ForEach(jobRun => _provider.AddJobRun(jobRun));
            var states = new JobRunStates[] { JobRunStates.Initializing, JobRunStates.Preparing, JobRunStates.Connected };
            var statesCount = jobRuns.Count(jobRun => states.Contains(jobRun.State));

            // Act
            var result = _provider.GetJobRunsByStates(states);

            // Assert
            result.TotalItems.ShouldBe(statesCount);
            result.Items.ShouldAllBe(item => states.Contains(item.State));
        }
    }
}
