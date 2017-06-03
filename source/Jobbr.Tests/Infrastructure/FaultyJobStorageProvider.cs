using System;
using System.Collections.Generic;
using System.Reflection;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Tests.Infrastructure
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

        public long GetJobsCount()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobsCount();
        }

        public List<Job> GetJobs(int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobs(page, pageSize);
        }

        public void AddJob(Job job)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddJob(job);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggersByJobId(jobId);
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddTrigger(jobId, trigger);
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            this.CheckFailAll();
            this.inMemoryVersion.DisableTrigger(jobId, triggerId);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            this.CheckFailAll();
            this.inMemoryVersion.EnableTrigger(jobId, triggerId);
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetActiveTriggers();
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetTriggerById(jobId, triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public void AddJobRun(JobRun jobRun)
        {
            this.CheckFailAll();
            this.inMemoryVersion.AddJobRun(jobRun);
        }

        public List<JobRun> GetJobRuns(int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRuns();
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            this.CheckFailAll();
            this.inMemoryVersion.UpdateProgress(jobRunId, progress);
        }

        public void Update(JobRun jobRun)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobRun);
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

        public List<JobRun> GetJobRunsByUserId(string userId, int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserId(userId, page, pageSize);
        }

        public List<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize);
        }

        public void Update(Job job)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(job);
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            this.CheckFailAll();
            this.inMemoryVersion.Update(jobId, trigger);
        }

        public List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize);
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state, int page = 0, int pageSize = 50)
        {
            this.CheckFailAll();
            return this.inMemoryVersion.GetJobRunsByState(state, page, pageSize);
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

        public bool IsAvailable()
        {
            return true;
        }
    }
}