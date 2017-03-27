using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    public interface IJobbrRepository
    {
        List<Job> GetAllJobs();

        Job GetJob(long id);

        void UpdateJobRunProgress(long jobRunId, double progress);

        void SetPidForJobRun(JobRun jobRun, int id);

        JobRun GetJobRun(long id);

        List<JobTriggerBase> GetTriggers(long jobId);

        void SaveAddTrigger(RecurringTrigger trigger);

        void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc);

        void SaveAddTrigger(ScheduledTrigger trigger);

        void SaveAddTrigger(InstantTrigger trigger);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();

        JobTriggerBase SaveUpdateTrigger(long id, JobTriggerBase trigger, out bool hadChanges);

        bool CheckParallelExecution(long triggerId);

        List<JobRun> GetJobRunsByState(JobRunStates state);

        long AddJob(Job job);

        JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc);

        bool DisableTrigger(long triggerId);

        void Update(JobRun jobRun);

        JobRun GetJobRunById(long jobRunId);

        JobTriggerBase GetTriggerById(long triggerId);

        List<JobTriggerBase> GetTriggersByJobId(long jobId);

        List<JobRun> GetAllJobRuns();

        Job GetJobByUniqueName(string identifier);

        void Delete(JobRun jobRun);
        List<JobRun> GetJobRunsByTriggerId(long triggerId);
        List<JobRun> GetJobRunsForUserId(long userId);
        List<JobRun> GetJobRunsForUserName(string userName);

        JobRun GetNextJobRunByTriggerId(long triggerId, DateTime utcNow);

        JobRun GetLastJobRunByTriggerId(long triggerId, DateTime utcNow);

    }
}