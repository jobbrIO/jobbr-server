using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Logging;

namespace Jobbr.Server.Storage
{
    public class JobbrRepository : IJobbrRepository
    {
        private static readonly ILog Logger = LogProvider.For<JobbrRepository>();

        private readonly IJobStorageProvider storageProvider;

        public JobbrRepository(IJobStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        public List<Job> GetJobs(int page = 0, int pageSize = 50)
        {
            return this.storageProvider.GetJobs(page, pageSize);
        }

        public Job GetJob(long id)
        {
            return this.storageProvider.GetJobById(id);
        }

        public void UpdateJobRunProgress(long jobRunId, double progress)
        {
            this.storageProvider.UpdateProgress(jobRunId, progress);
        }

        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            jobRun.Pid = id;

            this.storageProvider.Update(jobRun);
        }

        public JobRun GetJobRun(long id)
        {
            return this.storageProvider.GetJobRunById(id);
        }

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            return this.storageProvider.GetTriggersByJobId(jobId);
        }

        public void SaveAddTrigger(long jobId, RecurringTrigger trigger)
        {
            this.storageProvider.AddTrigger(jobId, trigger);
        }

        public void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = this.storageProvider.GetJobRunById(jobRunId);
            jobRun.PlannedStartDateTimeUtc = plannedStartDateTimeUtc;

            this.Update(jobRun);
        }

        public void SaveAddTrigger(long jobId, ScheduledTrigger trigger)
        {
            this.storageProvider.AddTrigger(jobId, trigger);
        }

        public void SaveAddTrigger(long jobId, InstantTrigger trigger)
        {
            this.storageProvider.AddTrigger(jobId, trigger);
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.storageProvider.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.storageProvider.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            // TODO: Move this logic to the storage adapter which can implement this more efficient
            var trigger = this.storageProvider.GetTriggerById(jobId, triggerId);

            this.storageProvider.EnableTrigger(jobId, triggerId);
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            try
            {
                return this.storageProvider.GetActiveTriggers();
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot read active triggers from storage provider due to an exception. Returning empty list.", e);

                return new List<JobTriggerBase>();
            }
        }

        public JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges)
        {
            var triggerFromDb = this.storageProvider.GetTriggerById(jobId, trigger.Id);

            hadChanges = false;

            if (trigger.IsActive != triggerFromDb.IsActive)
            {
                // Activated or deactivated
                triggerFromDb.IsActive = trigger.IsActive;
                hadChanges = true;
            }

            hadChanges = hadChanges || this.ApplyOtherChanges(triggerFromDb as dynamic, trigger as dynamic);

            if (hadChanges)
            {
                if (trigger is InstantTrigger)
                {
                    this.storageProvider.Update(jobId, trigger as InstantTrigger);
                }

                if (trigger is ScheduledTrigger)
                {
                    this.storageProvider.Update(jobId, trigger as ScheduledTrigger);
                }

                if (trigger is RecurringTrigger)
                {
                    this.storageProvider.Update(jobId, trigger as RecurringTrigger);
                }
            }

            return triggerFromDb;
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            return this.storageProvider.GetJobRunsByState(state);
        }

        public void AddJob(Job job)
        {
            this.storageProvider.AddJob(job);
        }

        public JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = new JobRun
            {
                JobId = job.Id,
                TriggerId = trigger.Id,
                JobParameters = job.Parameters,
                InstanceParameters = trigger.Parameters,
                State = JobRunStates.Scheduled,
                PlannedStartDateTimeUtc = plannedStartDateTimeUtc
            };

            this.storageProvider.AddJobRun(jobRun);

            return jobRun;
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            this.storageProvider.DisableTrigger(jobId, triggerId);
        }

        public void Update(JobRun jobRun)
        {
            this.storageProvider.Update(jobRun);
        }

        public JobRun GetJobRunById(long jobRunId)
        {
            return this.storageProvider.GetJobRunById(jobRunId);
        }

        public List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId)
        {
            return this.storageProvider.GetJobRunsByTriggerId(jobId, triggerId);
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return this.storageProvider.GetTriggerById(jobId, triggerId);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return this.storageProvider.GetTriggersByJobId(jobId);
        }

        public List<JobRun> GetJobRuns(int page = 0, int pageSize = 50)
        {
            return this.storageProvider.GetJobRuns();
        }

        public List<JobRun> GetJobRunsForUserId(string userId)
        {
            return this.storageProvider.GetJobRunsByUserId(userId);
        }

        public List<JobRun> GetJobRunsByUserDisplayName(string userName)
        {
            return this.storageProvider.GetJobRunsByUserDisplayName(userName);
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return this.storageProvider.GetJobByUniqueName(identifier);
        }

        public void Delete(JobRun jobRun)
        {
            var jobRunFromStorage = this.storageProvider.GetJobRunById(jobRun.Id);

            jobRunFromStorage.State = JobRunStates.Deleted;
            this.storageProvider.Update(jobRunFromStorage);
        }

        public IEnumerable<JobRun> GetRunningJobs()
        {
            var minState = (int)JobRunStates.Scheduled + 1;
            var maxState = (int)JobRunStates.Completed - 1;

            var jobRunsByStateRange = this.GetJobRunsByStateRange((JobRunStates)minState, (JobRunStates)maxState);

            foreach (var jobRun in jobRunsByStateRange)
            {
                yield return jobRun;
            }
        }

        public IEnumerable<JobRun> GetRunningJobs(long triggerJobId, long triggerId)
        {
            var runningJobs = this.GetRunningJobs();

            foreach (var jobRun in runningJobs)
            {
                if (jobRun.TriggerId == triggerId && jobRun.JobId == triggerJobId)
                {
                    yield return jobRun;
                }
            }
        }

        public IEnumerable<JobRun> GetJobRunsByStateRange(JobRunStates minState, JobRunStates maxState)
        {
            if (maxState < minState)
            {
                throw new ArgumentOutOfRangeException(nameof(maxState), $"The parameter '{nameof(maxState)}' should not be lower than '{nameof(minState)}'");
            }

            for (var i = minState; i <= maxState; i++)
            {
                var currentState = (JobRunStates)i;

                var jobRunsByState = this.GetJobRunsByState(currentState);

                foreach (var jobRun in jobRunsByState)
                {
                    yield return jobRun;
                }
            }
        }

        private bool ApplyOtherChanges(RecurringTrigger fromDb, RecurringTrigger updatedOne)
        {
            if (fromDb.Definition != updatedOne.Definition)
            {
                fromDb.Definition = updatedOne.Definition;
                return true;
            }

            return false;
        }

        private bool ApplyOtherChanges(ScheduledTrigger fromDb, ScheduledTrigger updatedOne)
        {
            if (fromDb.StartDateTimeUtc != updatedOne.StartDateTimeUtc)
            {
                fromDb.StartDateTimeUtc = updatedOne.StartDateTimeUtc;

                return true;
            }

            return false;
        }

        private bool ApplyOtherChanges(InstantTrigger fromDb, InstantTrigger updatedOne)
        {
            Logger.WarnFormat("Cannot change an instant trigger!");

            return false;
        }

        private bool ApplyOtherChanges(object fromDb, object updatedOne)
        {
            Logger.WarnFormat("Unknown trigger types: From: {1}, To: {2}!", fromDb.GetType(), updatedOne.GetType());
            return false;
        }
    }
}