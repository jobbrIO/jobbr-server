using System;
using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    public class InMemoryJobStorageProvider : IJobStorageProvider
    {
        private readonly List<JobTriggerBase> localTriggers = new List<JobTriggerBase>();

        private readonly List<Job> localJobs = new List<Job>();

        private readonly List<JobRun> localJobRuns = new List<JobRun>();

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return this.localTriggers.Where(t => t.IsActive == true).ToList().Clone();
        }

        public List<Job> GetJobs(int page = 0, int pageSize = 50)
        {
            return this.localJobs.Clone();
        }

        public List<JobRun> GetJobRuns(long page = 0, long pageSize = 50)
        {
            return this.localJobRuns.Clone();
        }

        public void AddJob(Job job)
        {
            var maxJobId = this.localJobs.Count + 1;
            job.Id = maxJobId;
            this.localJobs.Add(job);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return this.localTriggers.Where(t => t.JobId == jobId).ToList().Clone();
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            this.localTriggers.Add(trigger);
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            this.localTriggers.Add(trigger);
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            this.localTriggers.Add(trigger);
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            var trigger = this.localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = false;
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            var trigger = this.localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = true;
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return this.localTriggers.FirstOrDefault(t => t.Id == triggerId).Clone();
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.TriggerId == triggerId && jr.ActualEndDateTimeUtc < utcNow).Clone();
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.TriggerId == triggerId && jr.PlannedStartDateTimeUtc >= utcNow).Clone();
        }

        public void AddJobRun(JobRun jobRun)
        {
            var maxJobRunId = this.localJobRuns.Count + 1;

            jobRun.Id = maxJobRunId;

            this.localJobRuns.Add(jobRun);
        }

        public void Update(JobRun jobRun)
        {
            this.localJobRuns.Remove(this.localJobRuns.FirstOrDefault(jr => jr.Id == jobRun.Id));
            this.localJobRuns.Add(jobRun);
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            var jobRun = this.localJobRuns.First(p => p.Id == jobRunId);
            jobRun.Progress = progress;
        }

        public Job GetJobById(long id)
        {
            return this.localJobs.FirstOrDefault(j => j.Id == id).Clone();
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return this.localJobs.FirstOrDefault(j => j.UniqueName == identifier).Clone();
        }

        public JobRun GetJobRunById(long id)
        {
            return this.localJobRuns.FirstOrDefault(j => j.Id == id).Clone();
        }

        public List<JobRun> GetJobRunsByUserId(string userId, long page = 0, long pageSize = 50)
        {
            var allTriggers = this.localTriggers.Where(t => t.UserId == userId).Select(t => t.Id);

            return this.localJobRuns.Where(jr => allTriggers.Contains(jr.TriggerId)).OrderByDescending(r => r.Id).ToList().Clone();
        }

        public List<JobRun> GetJobRunsByUserDisplayName(string userName, long page = 0, long pageSize = 50)
        {
            var allTriggers = this.localTriggers.Where(t => t.UserDisplayName == userName).Select(t => t.Id);

            return this.localJobRuns.Where(jr => allTriggers.Contains(jr.TriggerId)).OrderByDescending(r => r.Id).ToList().Clone();
        }

        public void Update(Job job)
        {
            this.localJobs.Remove(this.localJobs.FirstOrDefault(j => j.Id == job.Id));
            this.localJobs.Add(job);
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            this.localTriggers.Remove(this.localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            this.localTriggers.Add(trigger);
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            this.localTriggers.Remove(this.localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            this.localTriggers.Add(trigger);
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            this.localTriggers.Remove(this.localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            this.localTriggers.Add(trigger);
        }

        public List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, long page = 0, long pageSize = 50)
        {
            return this.localJobRuns.Where(jr => jr.TriggerId == triggerId).ToList().Clone();
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state, long page = 0, long pageSize = 50)
        {
            return this.localJobRuns.Where(jr => jr.State == state).ToList().Clone();
        }

        public bool IsAvailable()
        {
            return true;
        }

#pragma warning disable CA1024 // Use properties where appropriate.
        public long GetJobsCount()
#pragma warning restore CA1024 // Use properties where appropriate.
        {
            return this.localJobs.Count;
        }
    }
}
