using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Tests.Infrastructure
{
    public class PseudoJobStorageProvider : IJobStorageProvider
    {
        public void DeleteJob(long jobId)
        {
        }

        public long GetJobsCount()
        {
            return 0;
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return new PagedResult<Job>
            {
                Items = new List<Job>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public void AddJob(Job job)
        {
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return null;
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50)
        {
            return new PagedResult<JobTriggerBase>
            {
                Items = new List<JobTriggerBase>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return new PagedResult<JobTriggerBase>
            {
                Items = new List<JobTriggerBase>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return null;
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return null;
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return null;
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 11, int pageSize = 50, params string[] sort)
        {
            return null;
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return new PagedResult<JobRun>
            {
                Items = new List<JobRun>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return new PagedResult<JobRun>
            {
                Items = new List<JobRun>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, params string[] sort)
        {
            return new PagedResult<JobRun>
            {
                Items = new List<JobRun>(),
                TotalItems = 0,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            return null;
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            return null;
        }

        public void AddJobRun(JobRun jobRun)
        {
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return null;
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
        }

        public void Update(JobRun jobRun)
        {
        }

        public Job GetJobById(long id)
        {
            return null;
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return null;
        }

        public JobRun GetJobRunById(long id)
        {
            return null;
        }

        public void Update(Job job)
        {
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
        }

        public bool IsAvailable()
        {
            return true;
        }
    }
}