using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Jobbr.Server.JobRegistry;
using Jobbr.Tests.Integration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jobbr.Tests.Registration
{
    [TestClass]
    public class BuilderTests
    {
        ILoggerFactory _loggerFactory;

        [TestInitialize]
        public void Initialize()
        {
            _loggerFactory = new LoggerFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _loggerFactory.Dispose();
        }

        [TestMethod]
        public void RegisterCustomArtefactStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactory);

            builder.Register<IArtefactsStorageProvider>(typeof(CustomArtefactStorageAdapter));
            builder.AppendTypeToCollection<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            // Act
            builder.Create();

            // Assert
            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomArtefactStorageAdapter),
                ExposeAllServicesComponent.Instance.ArtefactsStorageProvider.GetType());
        }

        [TestMethod]
        public void RegisterCustomJobStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactory);

            builder.Register<IJobStorageProvider>(typeof(CustomJobStorageProvider));
            builder.AppendTypeToCollection<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            // Act
            builder.Create();

            // Assert
            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomJobStorageProvider),
                ExposeAllServicesComponent.Instance.JobStorageProvider.GetType());
        }

        [TestMethod]
        public void ShouldDeleteJobsAndTriggers_WhenSingleSourceOfTruthIsActivated()
        {
            // Arrange
            const string existingJobName = "MyJobExists";
            const long existingJobId = 1;
            const long nonExistingJobId = 2;
            const long existingTriggerId = 10;
            const long nonExistingTriggerId = 20;

            var existingJob = new Job { Id = 1, UniqueName = existingJobName };
            var nonExistingJob = new Job { Id = 2, UniqueName = "DoIDieNow?" };
            var pagedJobs = CreatePagedResult(existingJob, nonExistingJob);
            var triggerForExistingJob = new RecurringTrigger { Id = existingTriggerId, JobId = existingJobId };
            var triggerForNonExistingJob = new RecurringTrigger { Id = nonExistingTriggerId, JobId = nonExistingJobId };
            
            var storageMock = new Mock<IJobStorageProvider>();
            storageMock.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false))
                .Returns(pagedJobs);
            storageMock.Setup(s => s.GetTriggersByJobId(existingJobId, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(triggerForExistingJob));
            storageMock.Setup(s => s.GetTriggersByJobId(nonExistingJobId, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(triggerForNonExistingJob));
            SetupForSuccessfulRun(storageMock);
            
            var builder = new JobbrBuilder(_loggerFactory);
            builder.Add<IJobStorageProvider>(storageMock.Object);
            builder.AddJobs(_loggerFactory, repo => repo.AsSingleSourceOfTruth().Define(existingJobName, "CLR.Type"));

            // Act
            builder.Create().Start();

            // Assert
            Assert.IsFalse(triggerForNonExistingJob.IsActive);
            Assert.IsTrue(triggerForNonExistingJob.Deleted);
            _loggerFactory.Dispose();
        }

        [TestMethod]
        public void ShouldDeleteOrphanedTriggers_WhenSingleSourceOfTruthIsActivated()
        {
            // Arrange
            const string jobName = "JobName";
            var storageMock = new Mock<IJobStorageProvider>();
            var builder = new JobbrBuilder(_loggerFactory);
            var job = new Job { Id = 1, UniqueName = jobName };
            var trigger = new RecurringTrigger { Definition = "0 * * * *", JobId = 1, Id = 1 };
            var toDeleteTrigger = new RecurringTrigger { Definition = "0 1 * * *", JobId = 1, Id = 10 };

            storageMock.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false)).Returns(CreatePagedResult(job));
            storageMock.Setup(s => s.GetTriggersByJobId(1, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(trigger, toDeleteTrigger));
            storageMock.Setup(s => s.GetJobByUniqueName(jobName)).Returns(job);
            SetupForSuccessfulRun(storageMock);

            builder.Add<IJobStorageProvider>(storageMock.Object);
            builder.AddJobs(_loggerFactory, repo =>
                repo.AsSingleSourceOfTruth().Define(jobName, "CLR.Type").WithTrigger("0 * * * *"));

            // Act
            builder.Create().Start();

            // Assert
            Assert.IsFalse(job.Deleted);
            Assert.IsFalse(toDeleteTrigger.IsActive);
            Assert.IsTrue(toDeleteTrigger.Deleted);
        }

        [TestMethod]
        public void ShouldReActivateJob_WhenJobIsRedefined()
        {
            // Arrange
            const string jobName = "JobName";
            var storageMock = new Mock<IJobStorageProvider>();
            var builder = new JobbrBuilder(_loggerFactory);
            var job = new Job { Id = 1, UniqueName = jobName };
            storageMock.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false)).Returns(CreatePagedResult(job));
            builder.AddJobs(_loggerFactory, repo =>
                repo.AsSingleSourceOfTruth().Define(jobName, "CLR.Type").WithTrigger("0 * * * *"));
            var jobbr = builder.Create();

            // Act
            jobbr.Start();
            jobbr.Stop();
            jobbr.Start();

            // Assert
            Assert.IsFalse(job.Deleted);
        }

        [TestMethod]
        public void ShouldOmitScheduledJob_WhenJobIsDeactivated()
        {
            // Arrange
            const string existingJobName = "MyJobExists";
            const long nonExistingJobId = 2;
            var existingJob = new Job { Id = 1, UniqueName = existingJobName };
            var nonExistingJob = new Job { Id = 2, UniqueName = "DoIDieNow?" };
            var pagedJobs = CreatePagedResult(existingJob, nonExistingJob);
            var toOmitJobRun = new JobRun { State = JobRunStates.Scheduled, Deleted = false };

            var storageMock = new Mock<IJobStorageProvider>();
            storageMock.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false)).Returns(pagedJobs);
            storageMock.Setup(s => s.GetJobRunsByJobId((int)nonExistingJobId, 1, int.MaxValue, false))
                .Returns(CreatePagedResult(toOmitJobRun));

            var builder = new JobbrBuilder(_loggerFactory);
            builder.Add<IJobStorageProvider>(storageMock.Object);
            builder.AddJobs(_loggerFactory, repo => repo.AsSingleSourceOfTruth().Define(existingJobName, "CLR.Type"));

            // Act
            builder.Create().Start();

            // Assert
            Assert.IsTrue(toOmitJobRun.Deleted);
            Assert.AreEqual(JobRunStates.Omitted, toOmitJobRun.State);
        }

        [TestMethod]
        public void ShouldOmitScheduledJob_WhenTriggerIsDeactivated()
        {
            // Arrange
            const string jobName = "JobName";
            var storageMock = new Mock<IJobStorageProvider>();
            var builder = new JobbrBuilder(_loggerFactory);
            var job = new Job { Id = 1, UniqueName = jobName };
            var toDeleteTrigger = new RecurringTrigger { Definition = "0 1 * * *", JobId = 1, Id = 10 };
            var toOmitJobRun = new JobRun { State = JobRunStates.Scheduled, Deleted = false };

            storageMock.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false)).Returns(CreatePagedResult(job));
            storageMock.Setup(s => s.GetTriggersByJobId(1, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(toDeleteTrigger));
            storageMock.Setup(s => s.GetJobByUniqueName(jobName)).Returns(job);
            storageMock.Setup(s => s.GetJobRunsByTriggerId(1, 10, 1, int.MaxValue, false))
                .Returns(CreatePagedResult(toOmitJobRun));
            
            builder.Add<IJobStorageProvider>(storageMock.Object);
            builder.AddJobs(_loggerFactory, repo =>
                repo.AsSingleSourceOfTruth().Define(jobName, "CLR.Type").WithTrigger("* * * * *"));

            // Act
            builder.Create().Start();
            
            // Assert
            Assert.IsTrue(toOmitJobRun.Deleted);
            Assert.AreEqual(JobRunStates.Omitted, toOmitJobRun.State);
        }

        private static PagedResult<T> CreatePagedResult<T>(params T[] args)
        {
            var items = new List<T>();
            items.AddRange(args.ToList());
            return new PagedResult<T> { Items = items };
        }

        private static void SetupForSuccessfulRun(Mock<IJobStorageProvider> storageMock)
        {
            storageMock.Setup(s => s.GetJobRunsByState(It.IsAny<JobRunStates>(), 1, int.MaxValue, null, null, null, false))
                .Returns(CreatePagedResult<JobRun>());
            storageMock.Setup(s => s.GetActiveTriggers(1, int.MaxValue, null, null, null))
                .Returns(CreatePagedResult<JobTriggerBase>());
            var anyId = It.IsAny<long>();
            storageMock.Setup(s => s.GetJobRunsByJobId(It.IsAny<int>(), 1, int.MaxValue, false)).Returns(CreatePagedResult<JobRun>());
            storageMock.Setup(s => s.GetJobRunsByTriggerId(anyId, anyId, 1, int.MaxValue, false)).Returns(CreatePagedResult<JobRun>());
        }
    }

    public interface IPrioritizationStrategy: IComparable<JobRun>
    {
        
    }

    #region Fake Implementations

    public class CustomArtefactStorageAdapter : IArtefactsStorageProvider
    {
        public void Save(string container, string fileName, Stream content)
        {
        }

        public Stream Load(string container, string fileName)
        {
            return null;
        }

        public List<JobbrArtefact> GetArtefacts(string container)
        {
            return null;
        }
    }

    public class CustomJobStorageProvider : IJobStorageProvider
    {
        public void AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public void DeleteJob(long jobId)
        {
            throw new NotImplementedException();
        }

        public long GetJobsCount()
        {
            throw new NotImplementedException();
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public Job GetJobById(long id)
        {
            throw new NotImplementedException();
        }

        public Job GetJobByUniqueName(string identifier)
        {
            throw new NotImplementedException();
        }

        public void Update(Job job)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddJobRun(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRunById(long id)
        {
            throw new NotImplementedException();
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public void Update(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            throw new NotImplementedException();
        }

        public void ApplyRetention(DateTimeOffset date)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }

}

#endregion
