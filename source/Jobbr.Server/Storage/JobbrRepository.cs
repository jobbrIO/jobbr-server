using System;
using System.Collections.Generic;
using System.Linq;
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

        public List<Job> GetAllJobs()
        {
            return this.storageProvider.GetJobs();
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

        public void SaveAddTrigger(RecurringTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
        }

        public void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = this.storageProvider.GetJobRunById(jobRunId);
            jobRun.PlannedStartDateTimeUtc = plannedStartDateTimeUtc;

            this.Update(jobRun);
        }

        public void SaveAddTrigger(ScheduledTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
        }

        public void SaveAddTrigger(InstantTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            return this.storageProvider.GetLastJobRunByTriggerId(triggerId);
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId, DateTime now)
        {
            return this.storageProvider.GetJobRunsByTriggerId(triggerId).FirstOrDefault(r => r.PlannedStartDateTimeUtc >= now);
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId)
        {
            return this.storageProvider.GetFutureJobRunsByTriggerId(triggerId);
        }

        public bool EnableTrigger(long triggerId)
        {
            // TODO: Move this logic to the storage adapter which can implement this more efficient
            var trigger = this.storageProvider.GetTriggerById(triggerId);

            if (trigger.IsActive)
            {
                return false;
            }

            this.storageProvider.EnableTrigger(triggerId);
            return true;
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

        public JobTriggerBase SaveUpdateTrigger(long id, JobTriggerBase trigger, out bool hadChanges)
        {
            var triggerFromDb = this.storageProvider.GetTriggerById(id);

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
                    this.storageProvider.Update(trigger as InstantTrigger);
                }

                if (trigger is ScheduledTrigger)
                {
                    this.storageProvider.Update(trigger as ScheduledTrigger);
                }

                if (trigger is RecurringTrigger)
                {
                    this.storageProvider.Update(trigger as RecurringTrigger);
                }
            }
            return triggerFromDb;
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

        public bool CheckParallelExecution(long triggerId)
        {
            return this.storageProvider.CheckParallelExecution(triggerId);
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            return this.storageProvider.GetJobRunsByState(state);
        }

        public long AddJob(Job job)
        {
            return this.storageProvider.AddJob(job);
        }

        public JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = new JobRun
            {
                JobId = job.Id,
                TriggerId = trigger.Id,
                JobParameters = job.Parameters,
                InstanceParameters = trigger.Parameters,
                UniqueId = Guid.NewGuid(),
                State = JobRunStates.Scheduled,
                PlannedStartDateTimeUtc = plannedStartDateTimeUtc
            };

            jobRun.Id = this.storageProvider.AddJobRun(jobRun);
            return jobRun;
        }

        public bool DisableTrigger(long triggerId)
        {
            // TODO: Move this logic to the storage adapter which can implement this more efficient
            var trigger = this.storageProvider.GetTriggerById(triggerId);

            if (!trigger.IsActive)
            {
                return false;
            }

            this.storageProvider.DisableTrigger(triggerId);
            return true;
        }

        public void Update(JobRun jobRun)
        {
            this.storageProvider.Update(jobRun);
        }

        public JobRun GetJobRunById(long jobRunId)
        {
            return this.storageProvider.GetJobRunById(jobRunId);
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            return this.storageProvider.GetJobRunsByTriggerId(triggerId);
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            return this.storageProvider.GetTriggerById(triggerId);
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return this.storageProvider.GetTriggersByJobId(jobId);
        }

        public List<JobRun> GetAllJobRuns()
        {
            return this.storageProvider.GetJobRuns();
        }

        public List<JobRun> GetJobRunsForUserId(long userId)
        {
            return this.storageProvider.GetJobRunsForUserId(userId);
        }

        public List<JobRun> GetJobRunsForUserName(string userName)
        {
            return this.storageProvider.GetJobRunsForUserName(userName);
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
    }
}