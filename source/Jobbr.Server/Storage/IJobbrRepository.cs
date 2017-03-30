using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    public interface IJobbrRepository
    {
        List<Job> GetJobs(int page = 0, int pageSize = 50);

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

        List<JobTriggerBase> GetActiveTriggers();

        JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges);

        List<JobRun> GetJobRunsByState(JobRunStates state);

        void AddJob(Job job);

        JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc);

        void DisableTrigger(long jobId, long triggerId);

        void Update(JobRun jobRun);

        JobRun GetJobRunById(long jobRunId);

        JobTriggerBase GetTriggerById(long jobId, long triggerId);

        List<JobTriggerBase> GetTriggersByJobId(long jobId);

        List<JobRun> GetJobRuns(int page = 0, int pageSize = 50);

        Job GetJobByUniqueName(string identifier);

        void Delete(JobRun jobRun);

        List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId);

        List<JobRun> GetJobRunsForUserId(string userId);

        List<JobRun> GetJobRunsByUserDisplayName(string userName);

        JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);

        JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);

        bool CheckParallelExecution(long triggerId);
    }
}