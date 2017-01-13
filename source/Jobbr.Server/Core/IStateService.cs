using System;

using Jobbr.Common.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    ///     The stateService interface.
    /// </summary>
    public interface IStateService
    {
        /// <summary>
        ///     The trigger updated.
        /// </summary>
        event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        long CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc);

        bool CheckParallelExecution(long triggerId);

        /// <summary>
        ///     The job run modification.
        /// </summary>
        event EventHandler<JobRunModificationEventArgs> JobRunModification;

        void UpdateJobRunState(JobRun jobRun, JobRunState state);

        void UpdateJobRunDirectories(JobRun jobRun, string workDir, string tempDir);

        void SetPidForJobRun(JobRun jobRun, int id);

        void SetJobRunStartTime(JobRun jobRun, DateTime startDateTimeUtc);

        void SetJobRunEndTime(JobRun jobRun, DateTime endDateTimeUtc);

        void UpdateJobRunProgress(long jobRunId, double percent);

        void UpdatePlannedStartDate(long jobRunId, DateTime plannedStartDateTimeUtc);

        void DipathOnTriggerUpdate(JobTriggerBase triggerFromDb);
    }
}