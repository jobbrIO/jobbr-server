using System;
using System.Collections.Generic;

using Jobbr.Common;
using Jobbr.Common.Model;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The JobService interface.
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// The trigger updated.
        /// </summary>
        event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        List<Job> GetAllJobs();

        Job GetJob(long id);

        Job AddJob(Job job);

        JobRun GetJobRun(long id);

        List<JobTriggerBase> GetTriggers(long jobId);

        long AddTrigger(RecurringTrigger trigger);

        long AddTrigger(ScheduledTrigger trigger);

        long AddTrigger(InstantTrigger trigger);

        bool DisableTrigger(long triggerId, bool enableNotification);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();

        JobRun GetLastJobRunByTriggerId(long triggerId);

        JobRun GetNextJobRunByTriggerId(long triggerId);

        long CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc);

        /// <summary>
        /// The job run modification.
        /// </summary>
        event EventHandler<JobRunModificationEventArgs> JobRunModification;

        List<JobRun> GetJobRuns(JobRunState state);

        void UpdateJobRunState(JobRun jobRun, JobRunState state);

        void UpdateJobRunDirectories(JobRun jobRun, string workDir, string tempDir);

        void SetPidForJobRun(JobRun jobRun, int id);

        void SetJobRunStartTime(JobRun jobRun, DateTime startDateTimeUtc);

        void SetJobRunEndTime(JobRun jobRun, DateTime endDateTimeUtc);

        void UpdateJobRunProgress(JobRun jobRun, double percent);

        void UpdatePlannedStartDate(JobRun plannedNextRun);

        void UpdateTrigger(long id, JobTriggerBase trigger);
    }
}