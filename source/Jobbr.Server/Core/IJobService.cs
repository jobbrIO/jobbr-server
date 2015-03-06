using System;
using System.Collections.Generic;

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

        /// <summary>
        /// The job added.
        /// </summary>
        event EventHandler<JobEventArgs> JobAdded;

        List<Job> GetAllJobs();

        Job GetJob(long id);

        long AddJob(Job job);

        List<JobRun> GetJobAllRuns();

        JobRun GetJobRun(long id);

        List<JobTriggerBase> GetTriggers(long jobId);

        long AddTrigger(CronTrigger trigger);

        long AddTrigger(StartDateTimeUtcTrigger trigger);

        long AddTrigger(InstantTrigger trigger);

        bool DisableTrigger(long triggerId);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();

        JobRun GetLastJobRunByTriggerId(long triggerId);

        JobRun GetNextJobRunByTriggerId(long triggerId);

        int CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc);
    }
}