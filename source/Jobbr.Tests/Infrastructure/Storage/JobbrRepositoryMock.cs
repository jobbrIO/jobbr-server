using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;
using System;
using System.Collections.Generic;

namespace Jobbr.Tests.Infrastructure.Storage
{
    public class JobbrRepositoryMock : IJobbrRepository
    {
        public const long JobId = 1968;

        public void AddJob(Job job)
        {
            job ??= new Job();
            job.Id = JobId;
        }

        public void Delete(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public void DeleteTrigger(long jobId, long triggerId)
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

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public Job GetJob(long id)
        {
            throw new NotImplementedException();
        }

        public Job GetJobByUniqueName(string identifier)
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRun(long id)
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRunById(long jobRunId)
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

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JobRun> GetJobRunsByStateRange(JobRunStates minState, JobRunStates maxState)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
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

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
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

        public IEnumerable<JobRun> GetRunningJobs(long triggerJobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<JobRun> GetRunningJobs()
        {
            throw new NotImplementedException();
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page, int pageSize = 50, bool showDeleted = false)
        {
            throw new NotImplementedException();
        }

        public void SaveAddTrigger(long jobId, RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void SaveAddTrigger(long jobId, ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void SaveAddTrigger(long jobId, InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc)
        {
            throw new NotImplementedException();
        }

        public JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges)
        {
            throw new NotImplementedException();
        }

        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            throw new NotImplementedException();
        }

        public void Update(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public void UpdateJobRunProgress(long jobRunId, double progress)
        {
            throw new NotImplementedException();
        }

        public void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc)
        {
            throw new NotImplementedException();
        }
    }
}
