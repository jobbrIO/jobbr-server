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

        List<Job> GetJobAllRuns();

        Job GetJobRun(long id);

        List<JobTriggerBase> GetTriggers(long jobId);

        long AddTrigger(CronTrigger trigger);

        long AddTrigger(StartDateTimeUtcTrigger trigger);

        long AddTrigger(InstantTrigger trigger);

        bool DisableTrigger(long triggerId);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();
    }
}