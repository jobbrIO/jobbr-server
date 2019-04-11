using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Jobbr.Server.JobRegistry;
using Jobbr.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jobbr.Tests.Registration
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void RegisterCustomArtefactStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            var builder = new JobbrBuilder();

            builder.Register<IArtefactsStorageProvider>(typeof(CustomArtefactStorageAdapter));
            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            builder.Create();

            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomArtefactStorageAdapter),
                ExposeAllServicesComponent.Instance.ArtefactsStorageProvider.GetType());
        }

        [TestMethod]
        public void RegisterCustomJobStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            var builder = new JobbrBuilder();

            builder.Register<IJobStorageProvider>(typeof(CustomJobStorageProvider));
            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            builder.Create();

            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomJobStorageProvider),
                ExposeAllServicesComponent.Instance.JobStorageProvider.GetType());
        }

        [TestMethod]
        public void ShouldDeleteJobsAndTriggers_WhenSingleSourceOfTruthIsActivated()
        {
            const string existingJobName = "MyJobExists";
            const long existingJobId = 1;
            const long nonExistingJobId = 2;
            const long existingTriggerId = 10;
            const long nonExistingTriggerId = 20;
            var existingJob = new Job {Id = 1, UniqueName = existingJobName};
            var nonExistingJob = new Job {Id = 2, UniqueName = "DoIDieNow?"};
            var pagedJobs = CreatePagedResult(existingJob, nonExistingJob);
            var triggerForExistingJob = new RecurringTrigger { Id = existingTriggerId, JobId = existingJobId };
            var triggerForNonExistingJob = new RecurringTrigger { Id = nonExistingTriggerId, JobId = nonExistingJobId };
            var storage = new Mock<IJobStorageProvider>();
            storage.Setup(s => s.GetJobs(1, int.MaxValue, null, null, null, false)).Returns(pagedJobs);
            storage.Setup(s => s.GetTriggersByJobId(existingJobId, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(triggerForExistingJob));
            storage.Setup(s => s.GetTriggersByJobId(nonExistingJobId, 1, int.MaxValue, false))
                .Returns(CreatePagedResult<JobTriggerBase>(triggerForNonExistingJob));
            SetupForSuccessfulRun(storage);
            var builder = new JobbrBuilder();
            builder.Add<IJobStorageProvider>(storage.Object);
            builder.AddJobs(repo =>
                repo.AsSingleSourceOfTruth().Define(existingJobName, "CLR.Type"));

            builder.Create().Start(Int32.MaxValue);

            storage.Verify(s => s.DeleteJob(nonExistingJobId), Times.Once);
            storage.Verify(s => s.DeleteTrigger(nonExistingJobId, nonExistingTriggerId), Times.Once);
        }

        private static PagedResult<T> CreatePagedResult<T>(params T[] args)
        {
            var items = new List<T>();
            items.AddRange(args.ToList());
            return new PagedResult<T> { Items = items };
        }

        private static void SetupForSuccessfulRun(Mock<IJobStorageProvider> storage)
        {
            storage.Setup(s => s.GetJobRunsByState(It.IsAny<JobRunStates>(), 1, int.MaxValue, null, null, null, false))
                .Returns(CreatePagedResult<JobRun>());
            storage.Setup(s => s.GetActiveTriggers(1, int.MaxValue, null, null, null))
                .Returns(CreatePagedResult<JobTriggerBase>());
        }
    }

    public interface IPriorizationStrategy: IComparable<JobRun>
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

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }

}

#endregion
