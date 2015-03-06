using System.Collections.Generic;

using Jobbr.Server.Model;

namespace Jobbr.Server.Common
{
    public interface IJobbrStorageProvider
    {
        List<Job> GetJobs();

        long AddJob(Job job);

        List<JobTriggerBase> GetTriggers(long jobId);

        long AddTrigger(CronTrigger trigger);

        long AddTrigger(InstantTrigger trigger);

        long AddTrigger(StartDateTimeUtcTrigger trigger);

        bool DisableTrigger(long triggerId);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();

        JobTriggerBase GetTriggerById(long triggerId);

        JobRun GetLastJobRunByTriggerId(long triggerId);

        JobRun GetFutureJobRunsByTriggerId(long triggerId);

        int AddJobRun(JobRun jobRun);

        List<JobRun> GetJobRuns();

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="jobRun">
        ///     The job run.
        /// </param>
        bool Update(JobRun jobRun);

        Job GetJobById(long id);
    }
}