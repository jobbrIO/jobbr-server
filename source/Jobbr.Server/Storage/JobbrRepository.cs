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

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return this.storageProvider.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
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

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            try
            {
                return this.storageProvider.GetActiveTriggers();
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot read active triggers from storage provider due to an exception. Returning empty list.", e);

                return new PagedResult<JobTriggerBase>
                {
                    TotalItems = 0,
                    Items = new List<JobTriggerBase>(),
                    PageSize = pageSize,
                    Page = page
                };
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

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return this.storageProvider.GetJobRunsByState(state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
        }

        public void AddJob(Job job)
        {
            this.storageProvider.AddJob(job);
        }

        public JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = new JobRun
            {
                Job = job,
                Trigger = trigger,
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

        public void DeleteTrigger(long jobId, long triggerId)
        {
            this.storageProvider.DeleteTrigger(jobId, triggerId);
        }

        public void Update(JobRun jobRun)
        {
            this.storageProvider.Update(jobRun);
        }

        public JobRun GetJobRunById(long jobRunId)
        {
            return this.storageProvider.GetJobRunById(jobRunId);
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, params string[] sort)
        {
            return this.storageProvider.GetJobRunsByJobId(jobId, page, pageSize, sort);
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, params string[] sort)
        {
            return this.storageProvider.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, sort);
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return this.storageProvider.GetTriggerById(jobId, triggerId);
        }

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50)
        {
            return this.storageProvider.GetTriggersByJobId(jobId, page, pageSize);
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            return this.storageProvider.GetJobRuns(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            return this.storageProvider.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, sort);
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            return this.storageProvider.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, sort);
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
                if (jobRun.Trigger.Id == triggerId && jobRun.Job.Id == triggerJobId)
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

                var jobRunsByState = this.GetJobRunsByState(currentState, 1, int.MaxValue);

                foreach (var jobRun in jobRunsByState.Items)
                {
                    yield return jobRun;
                }
            }
        }

        private bool ApplyOtherChanges(RecurringTrigger fromDb, RecurringTrigger updatedOne)
        {
            if (!string.Equals(fromDb.Definition, updatedOne.Definition, StringComparison.OrdinalIgnoreCase))
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