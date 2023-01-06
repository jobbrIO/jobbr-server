using System;
using System.Reflection;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.IntegrationTests.Infrastructure
{
    public class FaultyJobStorageProvider : IJobStorageProvider
    {
        private readonly IJobStorageProvider _inMemoryVersion = new InMemoryJobStorageProvider();

        private bool _failAll;

        public FaultyJobStorageProvider()
        {
            Instance = this;
        }

        public static FaultyJobStorageProvider Instance { get; private set; }

        public void DeleteJob(long jobId)
        {
            throw new NotImplementedException();
        }

        public long GetJobsCount()
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobsCount();
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public void AddJob(Job job)
        {
            CheckFailAll();
            _inMemoryVersion.AddJob(job);
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            CheckFailAll();
            return _inMemoryVersion.GetTriggersByJobId(jobId);
        }

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetActiveTriggers(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            CheckFailAll();
            _inMemoryVersion.DisableTrigger(jobId, triggerId);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            CheckFailAll();
            _inMemoryVersion.EnableTrigger(jobId, triggerId);
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
            CheckFailAll();
            _inMemoryVersion.DeleteTrigger(jobId, triggerId);
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            CheckFailAll();
            return _inMemoryVersion.GetTriggerById(jobId, triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            CheckFailAll();
            return _inMemoryVersion.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            CheckFailAll();
            return _inMemoryVersion.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByState(state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByStates(states, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByJobId(jobId, page, pageSize, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        public void AddJobRun(JobRun jobRun)
        {
            CheckFailAll();
            _inMemoryVersion.AddJobRun(jobRun);
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRuns();
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            CheckFailAll();
            _inMemoryVersion.UpdateProgress(jobRunId, progress);
        }

        public void ApplyRetention(DateTimeOffset date)
        {
            throw new NotImplementedException();
        }

        public void Update(JobRun jobRun)
        {
            CheckFailAll();
            _inMemoryVersion.Update(jobRun);
        }

        public Job GetJobById(long id)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobById(id);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobByUniqueName(identifier);
        }

        public JobRun GetJobRunById(long id)
        {
            CheckFailAll();
            return _inMemoryVersion.GetJobRunById(id);
        }

        public void Update(Job job)
        {
            CheckFailAll();
            _inMemoryVersion.Update(job);
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            CheckFailAll();
            _inMemoryVersion.Update(jobId, trigger);
        }

        public void DisableImplementation()
        {
            _failAll = true;
        }

        public void EnableImplementation()
        {
            _failAll = false;
        }

        public bool IsAvailable()
        {
            return true;
        }

        private void CheckFailAll()
        {
            if (_failAll)
            {
                throw new TargetException("This JobStorageProvider is currently not healthy!");
            }
        }
    }
}