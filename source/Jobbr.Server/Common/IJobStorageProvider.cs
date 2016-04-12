using System.Collections.Generic;

using Jobbr.Server.Model;
using Jobbr.Shared;
using Jobbr.Common.Model;

namespace Jobbr.Server.Common
{
    public interface IJobStorageProvider
    {
        List<Job> GetJobs();

        long AddJob(Job job);

        List<JobTriggerBase> GetTriggersByJobId(long jobId);

        long AddTrigger(RecurringTrigger trigger);

        long AddTrigger(InstantTrigger trigger);

        long AddTrigger(ScheduledTrigger trigger);

        bool DisableTrigger(long triggerId);

        bool EnableTrigger(long triggerId);

        List<JobTriggerBase> GetActiveTriggers();

        JobTriggerBase GetTriggerById(long triggerId);

        JobRun GetLastJobRunByTriggerId(long triggerId);

        JobRun GetFutureJobRunsByTriggerId(long triggerId);

        int AddJobRun(JobRun jobRun);

        List<JobRun> GetJobRuns();

        bool Update(JobRun jobRun);

        Job GetJobById(long id);

        Job GetJobByUniqueName(string identifier);

        JobRun GetJobRunById(long id);

        List<JobRun> GetJobRunsForUserId(long userId);

        List<JobRun> GetJobRunsForUserName(string userName);

        bool Update(Job job);

        bool Update(InstantTrigger trigger);
        
        bool Update(ScheduledTrigger trigger);

        bool Update(RecurringTrigger trigger);

        List<JobRun> GetJobRunsByTriggerId(long triggerId);

        List<JobRun> GetJobRunsByState(JobRunState state);
    }
}