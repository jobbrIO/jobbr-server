using System;
using System.Collections.Generic;
using System.Linq;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Model;

namespace Jobbr.Tests.StorageProvider
{
    public class InMemoryJobStorageProvider : IJobStorageProvider
    {
        List<JobTriggerBase> localTriggers = new List<JobTriggerBase>();

        private List<Job> localJobs = new List<Job>();

        private List<JobRun> localJobRuns = new List<JobRun>();

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return this.localTriggers.Where(t => t.IsActive == true).ToList();
        }

        public List<Job> GetJobs()
        {
            return this.localJobs;
        }

        public List<JobRun> GetJobRuns()
        {
            return this.localJobRuns;
        }

        public long AddJob(Job job)
        {
            var maxJobId = this.localJobs.Count + 1;
            job.Id = maxJobId;
            this.localJobs.Add(job);

            return maxJobId;
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return this.localTriggers.Where(t => t.JobId == jobId).ToList();
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            this.localTriggers.Add(trigger);

            return newTriggerId;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            this.localTriggers.Add(trigger);

            return newTriggerId;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            var newTriggerId = this.localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            this.localTriggers.Add(trigger);

            return newTriggerId;
        }

        public bool DisableTrigger(long triggerId)
        {
            var trigger = this.localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = false;
            return true;
        }

        public bool EnableTrigger(long triggerId)
        {
            var trigger = this.localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = true;
            return true;
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            return this.localTriggers.FirstOrDefault(t => t.Id == triggerId);
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.TriggerId == triggerId);
        }

        public JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.TriggerId == triggerId && jr.PlannedStartDateTimeUtc >= DateTime.UtcNow);
        }

        public int AddJobRun(JobRun jobRun)
        {
            var maxJobRunId = this.localJobRuns.Count + 1;

            jobRun.Id = maxJobRunId;

            this.localJobRuns.Add(jobRun);

            return maxJobRunId;
        }

        public bool Update(JobRun jobRun)
        {
            this.localJobRuns.Remove(this.localJobRuns.FirstOrDefault(jr => jr.Id == jobRun.Id));
            this.localJobRuns.Add(jobRun);

            return true;
        }

        public Job GetJobById(long id)
        {
            return this.localJobs.FirstOrDefault(j => j.Id == id);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return this.localJobs.FirstOrDefault(j => j.UniqueName == identifier);
        }

        public JobRun GetJobRunById(long id)
        {
            return this.localJobRuns.FirstOrDefault(j => j.Id == id);
        }

        public List<JobRun> GetJobRunsForUserId(long userId)
        {
            var allTriggers = this.localTriggers.Where(t => t.UserId == userId).Select(t => t.Id);

            return this.localJobRuns.Where(jr => allTriggers.Contains(jr.TriggerId)).ToList();
        }

        public List<JobRun> GetJobRunsForUserName(string userName)
        {
            var allTriggers = this.localTriggers.Where(t => t.UserName == userName).Select(t => t.Id);

            return this.localJobRuns.Where(jr => allTriggers.Contains(jr.TriggerId)).ToList();
        }

        public bool Update(Job job)
        {
            this.localJobs.Remove(this.localJobs.FirstOrDefault(j => j.Id == job.Id));
            this.localJobs.Add(job);

            return true;
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            return this.localJobRuns.Where(jr => jr.TriggerId == triggerId).ToList();
        }
    }
}
