using System;
using System.Collections.Generic;
using System.Reflection;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Tests.Infrastructure.StorageProvider
{
    public class FaultyJobStorageProvider : IJobStorageProvider
    {
        public static FaultyJobStorageProvider Instance { get; private set; }

        private readonly IJobStorageProvider inMemoryVersion = new InMemoryJobStorageProvider();

        private bool failAll;

        public FaultyJobStorageProvider()
        {
            Instance = this;
        }

        public List<Job> GetJobs()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobs();
        }

        public long AddJob(Job job)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.AddJob(job);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggersByJobId(jobId);
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.AddTrigger(trigger);
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.AddTrigger(trigger);
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.AddTrigger(trigger);
        }

        public bool DisableTrigger(long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.DisableTrigger(triggerId);
        }

        public bool EnableTrigger(long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.EnableTrigger(triggerId);
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetActiveTriggers();
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggerById(triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetLastJobRunByTriggerId(triggerId, utcNow);
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetNextJobRunByTriggerId(triggerId, utcNow);
        }

        public int AddJobRun(JobRun jobRun)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.AddJobRun(jobRun);
        }

        public List<JobRun> GetJobRuns()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRuns();
        }

        public bool UpdateProgress(long jobRunId, double? progress)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.UpdateProgress(jobRunId, progress);
        }

        public bool Update(JobRun jobRun)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.Update(jobRun);
        }

        public Job GetJobById(long id)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobById(id);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobByUniqueName(identifier);
        }

        public JobRun GetJobRunById(long id)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunById(id);
        }

        public List<JobRun> GetJobRunsByUserId(long userId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserId(userId);
        }

        public List<JobRun> GetJobRunsByUserName(string userName)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserName(userName);
        }

        public bool Update(Job job)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.Update(job);
        }

        public bool Update(InstantTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.Update(trigger);
        }

        public bool Update(ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.Update(trigger);
        }

        public bool Update(RecurringTrigger trigger)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.Update(trigger);
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByTriggerId(triggerId);
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByState(state);
        }

        public bool CheckParallelExecution(long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.CheckParallelExecution(triggerId);
        }

        public void DisableImplementation()
        {
            this.failAll = true;
        }

        public void EnableImplementation()
        {
            this.failAll = false;
        }

        private void CheckFailAll()
        {
            if (this.failAll)
            {
                throw new TargetException("This JobStorageProvider is currently not healthy!");
            }
        }
    }
}