using System;
using System.Reflection;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.IntegrationTests.Infrastructure
{
    public class FaultyJobStorageProvider : IJobStorageProvider
    {
        public static FaultyJobStorageProvider Instance { get; private set; }

        private readonly IJobStorageProvider inMemoryVersion = new InMemoryJobStorageProvider();

        private bool failAll;

        public FaultyJobStorageProvider()
        {
            Instance = this;
        }

        public void DeleteJob(long jobId)
        {
            throw new NotImplementedException();
        }

        public long GetJobsCount()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobsCount();
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public void AddJob(Job job)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddJob(job);
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggersByJobId(jobId);
        }

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetActiveTriggers(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            this.CheckFailAll();
            this.inMemoryVersion.DisableTrigger(jobId, triggerId);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            this.CheckFailAll();
            this.inMemoryVersion.EnableTrigger(jobId, triggerId);
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
            this.CheckFailAll();
            this.inMemoryVersion.DeleteTrigger(jobId, triggerId);
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggerById(jobId, triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByState(state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByStates(states, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByJobId(jobId, page, pageSize, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        public void AddJobRun(JobRun jobRun)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddJobRun(jobRun);
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRuns();
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            this.CheckFailAll();
            this.inMemoryVersion.UpdateProgress(jobRunId, progress);
        }

        public void ApplyRetention(DateTimeOffset date)
        {
            throw new NotImplementedException();
        }

        public void Update(JobRun jobRun)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobRun);
        }

        public Job GetJobById(long id)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobById(id);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobByUniqueName(identifier);
        }

        public JobRun GetJobRunById(long id)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunById(id);
        }

        public void Update(Job job)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(job);
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public void DisableImplementation()
        {
            this.failAll = true;
        }

        public void EnableImplementation()
        {
            this.failAll = false;
        }

        private void CheckFailAll()
        {
            if (this.failAll)
            {
                throw new TargetException("This JobStorageProvider is currently not healthy!");
            }
        }

        public bool IsAvailable()
        {
            return true;
        }
    }
}