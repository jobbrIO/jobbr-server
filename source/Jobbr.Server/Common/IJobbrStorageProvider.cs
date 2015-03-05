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
    }
}