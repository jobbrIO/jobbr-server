using System.Collections.Generic;
using System.Reflection;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Tests.StorageProvider
{
    public class FaultyJobStorageProvider : IJobStorageProvider
    {
        private readonly IJobStorageProvider inMemoryVersion = new InMemoryJobStorageProvider();

        private bool failAll;

        public List<Job> GetJobs()
        {
            CheckFailAll();
            return inMemoryVersion.GetJobs();
        }

        public long AddJob(Job job)
        {
            CheckFailAll();
            return inMemoryVersion.AddJob(job);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            CheckFailAll();
            return inMemoryVersion.GetTriggersByJobId(jobId);
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.AddTrigger(trigger);
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.AddTrigger(trigger);
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.AddTrigger(trigger);
        }

        public bool DisableTrigger(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.DisableTrigger(triggerId);
        }

        public bool EnableTrigger(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.EnableTrigger(triggerId);
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            CheckFailAll();
            return inMemoryVersion.GetActiveTriggers();
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.GetTriggerById(triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.GetLastJobRunByTriggerId(triggerId);
        }

        public JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.GetFutureJobRunsByTriggerId(triggerId);
        }

        public int AddJobRun(JobRun jobRun)
        {
            CheckFailAll();
            return inMemoryVersion.AddJobRun(jobRun);
        }

        public List<JobRun> GetJobRuns()
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRuns();
        }

        public bool UpdateProgress(long jobRunId, double? progress)
        {
            CheckFailAll();
            return inMemoryVersion.UpdateProgress(jobRunId, progress);
        }

        public bool Update(JobRun jobRun)
        {
            CheckFailAll();
            return inMemoryVersion.Update(jobRun);
        }

        public Job GetJobById(long id)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobById(id);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobByUniqueName(identifier);
        }

        public JobRun GetJobRunById(long id)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRunById(id);
        }

        public List<JobRun> GetJobRunsForUserId(long userId)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRunsForUserId(userId);
        }

        public List<JobRun> GetJobRunsForUserName(string userName)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRunsForUserName(userName);
        }

        public bool Update(Job job)
        {
            CheckFailAll();
            return inMemoryVersion.Update(job);
        }

        public bool Update(InstantTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.Update(trigger);
        }

        public bool Update(ScheduledTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.Update(trigger);
        }

        public bool Update(RecurringTrigger trigger)
        {
            CheckFailAll();
            return inMemoryVersion.Update(trigger);
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRunsByTriggerId(triggerId);
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            CheckFailAll();
            return inMemoryVersion.GetJobRunsByState(state);
        }

        public bool CheckParallelExecution(long triggerId)
        {
            CheckFailAll();
            return inMemoryVersion.CheckParallelExecution(triggerId);
        }

        public void DisableImplementation()
        {
            failAll = true;
        }

        public void EnableImplementation()
        {
            failAll = false;
        }

        private void CheckFailAll()
        {
            if (failAll)
            {
                throw new TargetException("This JobStorageProvider is currently not healthy!");
            }
        }
    }
}