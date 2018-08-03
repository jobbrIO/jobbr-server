using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    public interface IJobbrRepository
    {
        PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort);

        Job GetJob(long id);

        void UpdateJobRunProgress(long jobRunId, double progress);

        void SetPidForJobRun(JobRun jobRun, int id);

        JobRun GetJobRun(long id);

        List<JobTriggerBase> GetTriggers(long jobId);

        void SaveAddTrigger(long jobId, RecurringTrigger trigger);

        void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc);

        void SaveAddTrigger(long jobId, ScheduledTrigger trigger);

        void SaveAddTrigger(long jobId, InstantTrigger trigger);

        void EnableTrigger(long jobId, long triggerId);

        PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort);

        JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges);

        PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort);

        IEnumerable<JobRun> GetRunningJobs(long triggerJobId, long triggerId);

        IEnumerable<JobRun> GetRunningJobs();

        IEnumerable<JobRun> GetJobRunsByStateRange(JobRunStates minState, JobRunStates maxState);

        void AddJob(Job job);

        JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc);

        void DisableTrigger(long jobId, long triggerId);

        void DeleteTrigger(long jobId, long triggerId);

        void Update(JobRun jobRun);

        JobRun GetJobRunById(long jobRunId);

        JobTriggerBase GetTriggerById(long jobId, long triggerId);

        List<JobTriggerBase> GetTriggersByJobId(long jobId);

        PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort);

        Job GetJobByUniqueName(string identifier);

        void Delete(JobRun jobRun);

        PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, params string[] sort);

        PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort);

        PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort);

        JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);

        JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);
    }
}