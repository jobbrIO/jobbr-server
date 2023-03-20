using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// Jobbr repository implementation.
    /// </summary>
    public class JobbrRepository : IJobbrRepository
    {
        private readonly ILogger<JobbrRepository> _logger;
        private readonly IJobStorageProvider _jobStorageProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrRepository"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="jobStorageProvider">Job storage provider.</param>
        public JobbrRepository(ILoggerFactory loggerFactory, IJobStorageProvider jobStorageProvider)
        {
            _logger = loggerFactory.CreateLogger<JobbrRepository>();
            _jobStorageProvider = jobStorageProvider;
        }

        /// <inheritdoc/>
        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        /// <inheritdoc/>
        public Job GetJob(long id)
        {
            return _jobStorageProvider.GetJobById(id);
        }

        /// <inheritdoc/>
        public void UpdateJobRunProgress(long jobRunId, double progress)
        {
            _jobStorageProvider.UpdateProgress(jobRunId, progress);
        }

        /// <inheritdoc/>
        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            jobRun.Pid = id;

            _jobStorageProvider.Update(jobRun);
        }

        /// <inheritdoc/>
        public JobRun GetJobRun(long id)
        {
            return _jobStorageProvider.GetJobRunById(id);
        }

        /// <inheritdoc/>
        public void SaveAddTrigger(long jobId, RecurringTrigger trigger)
        {
            _jobStorageProvider.AddTrigger(jobId, trigger);
        }

        /// <inheritdoc/>
        public void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc)
        {
            var jobRun = _jobStorageProvider.GetJobRunById(jobRunId);
            jobRun.PlannedStartDateTimeUtc = plannedStartDateTimeUtc;

            Update(jobRun);
        }

        /// <inheritdoc/>
        public void SaveAddTrigger(long jobId, ScheduledTrigger trigger)
        {
            _jobStorageProvider.AddTrigger(jobId, trigger);
        }

        /// <inheritdoc/>
        public void SaveAddTrigger(long jobId, InstantTrigger trigger)
        {
            _jobStorageProvider.AddTrigger(jobId, trigger);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByStates(states, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        /// <inheritdoc/>
        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return _jobStorageProvider.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        /// <inheritdoc/>
        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return _jobStorageProvider.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);
        }

        /// <inheritdoc/>
        public void EnableTrigger(long jobId, long triggerId)
        {
            // TODO: Move this logic to the storage adapter which can implement this more efficient
            var trigger = _jobStorageProvider.GetTriggerById(jobId, triggerId);

            _jobStorageProvider.EnableTrigger(jobId, triggerId);
        }

        /// <inheritdoc/>
        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            try
            {
                return _jobStorageProvider.GetActiveTriggers(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Cannot read active triggers from storage provider due to an exception. Returning empty list.");

                return new PagedResult<JobTriggerBase>
                {
                    TotalItems = 0,
                    Items = new List<JobTriggerBase>(),
                    PageSize = pageSize,
                    Page = page,
                };
            }
        }

        /// <inheritdoc/>
        public JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges)
        {
            var triggerFromDb = _jobStorageProvider.GetTriggerById(jobId, trigger.Id);

            hadChanges = false;

            if (trigger.IsActive != triggerFromDb.IsActive)
            {
                // Activated or deactivated
                triggerFromDb.IsActive = trigger.IsActive;
                hadChanges = true;
            }

            hadChanges = hadChanges || ApplyOtherChanges(triggerFromDb as dynamic, trigger as dynamic);

            if (hadChanges)
            {
                trigger.CreatedDateTimeUtc = triggerFromDb.CreatedDateTimeUtc;

                if (trigger is InstantTrigger)
                {
                    _jobStorageProvider.Update(jobId, trigger as InstantTrigger);
                }

                if (trigger is ScheduledTrigger)
                {
                    _jobStorageProvider.Update(jobId, trigger as ScheduledTrigger);
                }

                if (trigger is RecurringTrigger)
                {
                    _jobStorageProvider.Update(jobId, trigger as RecurringTrigger);
                }
            }

            return triggerFromDb;
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByState(state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        /// <inheritdoc/>
        public void AddJob(Job job)
        {
            _jobStorageProvider.AddJob(job);
        }

        /// <inheritdoc/>
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

            _jobStorageProvider.AddJobRun(jobRun);

            return jobRun;
        }

        /// <inheritdoc/>
        public void DisableTrigger(long jobId, long triggerId)
        {
            _jobStorageProvider.DisableTrigger(jobId, triggerId);
        }

        /// <inheritdoc/>
        public void DeleteTrigger(long jobId, long triggerId)
        {
            _jobStorageProvider.DeleteTrigger(jobId, triggerId);
        }

        /// <inheritdoc/>
        public void Update(JobRun jobRun)
        {
            _jobStorageProvider.Update(jobRun);
        }

        /// <inheritdoc/>
        public JobRun GetJobRunById(long jobRunId)
        {
            return _jobStorageProvider.GetJobRunById(jobRunId);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByJobId(jobId, page, pageSize, showDeleted, sort);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, showDeleted, sort);
        }

        /// <inheritdoc/>
        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return _jobStorageProvider.GetTriggerById(jobId, triggerId);
        }

        /// <inheritdoc/>
        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            return _jobStorageProvider.GetTriggersByJobId(jobId, page, pageSize, showDeleted);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRuns(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        /// <inheritdoc/>
        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            return _jobStorageProvider.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);
        }

        /// <inheritdoc/>
        public Job GetJobByUniqueName(string identifier)
        {
            return _jobStorageProvider.GetJobByUniqueName(identifier);
        }

        /// <inheritdoc/>
        public void Delete(JobRun jobRun)
        {
            var jobRunFromStorage = _jobStorageProvider.GetJobRunById(jobRun.Id);

            jobRunFromStorage.State = JobRunStates.Deleted;
            _jobStorageProvider.Update(jobRunFromStorage);
        }

        /// <inheritdoc/>
        public IEnumerable<JobRun> GetRunningJobs()
        {
            var minState = (int)JobRunStates.Scheduled + 1;
            var maxState = (int)JobRunStates.Completed - 1;

            var jobRunsByStateRange = GetJobRunsByStateRange((JobRunStates)minState, (JobRunStates)maxState);

            foreach (var jobRun in jobRunsByStateRange)
            {
                yield return jobRun;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<JobRun> GetRunningJobs(long triggerJobId, long triggerId)
        {
            var runningJobs = GetRunningJobs();

            foreach (var jobRun in runningJobs)
            {
                if (jobRun.Trigger.Id == triggerId && jobRun.Job.Id == triggerJobId)
                {
                    yield return jobRun;
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<JobRun> GetJobRunsByStateRange(JobRunStates minState, JobRunStates maxState)
        {
            if (maxState < minState)
            {
                throw new ArgumentOutOfRangeException(nameof(maxState), $"The parameter '{nameof(maxState)}' should not be lower than '{nameof(minState)}'");
            }

            for (var i = minState; i <= maxState; i++)
            {
                var currentState = (JobRunStates)i;

                var jobRunsByState = GetJobRunsByState(currentState, 1, int.MaxValue);

                foreach (var jobRun in jobRunsByState.Items)
                {
                    yield return jobRun;
                }
            }
        }

        private bool ApplyOtherChanges(RecurringTrigger fromDb, RecurringTrigger updatedOne)
        {
            bool hadChanges = false;

            if (!string.Equals(fromDb.Definition, updatedOne.Definition, StringComparison.OrdinalIgnoreCase))
            {
                fromDb.Definition = updatedOne.Definition;
                hadChanges = true;
            }

            if (fromDb.NoParallelExecution != updatedOne.NoParallelExecution)
            {
                fromDb.NoParallelExecution = updatedOne.NoParallelExecution;
                hadChanges = true;
            }

            if (fromDb.StartDateTimeUtc != updatedOne.StartDateTimeUtc)
            {
                fromDb.StartDateTimeUtc = updatedOne.StartDateTimeUtc;
                hadChanges = true;
            }

            if (fromDb.EndDateTimeUtc != updatedOne.EndDateTimeUtc)
            {
                fromDb.EndDateTimeUtc = updatedOne.EndDateTimeUtc;
                hadChanges = true;
            }

            if (ApplyBaseChanges(fromDb, updatedOne))
            {
                hadChanges = true;
            }

            return hadChanges;
        }

        private bool ApplyOtherChanges(ScheduledTrigger fromDb, ScheduledTrigger updatedOne)
        {
            bool hadChanges = false;

            if (fromDb.StartDateTimeUtc != updatedOne.StartDateTimeUtc)
            {
                fromDb.StartDateTimeUtc = updatedOne.StartDateTimeUtc;

                hadChanges = true;
            }

            if (ApplyBaseChanges(fromDb, updatedOne))
            {
                hadChanges = true;
            }

            return hadChanges;
        }

        private bool ApplyBaseChanges(JobTriggerBase fromDb, JobTriggerBase updatedOne)
        {
            bool hadChanges = false;

            if (string.Equals(fromDb.Comment, updatedOne.Comment, StringComparison.Ordinal) == false)
            {
                fromDb.Comment = updatedOne.Comment;
                hadChanges = true;
            }

            if (string.Equals(fromDb.UserId, updatedOne.UserId, StringComparison.Ordinal) == false)
            {
                fromDb.UserId = updatedOne.UserId;
                hadChanges = true;
            }

            if (string.Equals(fromDb.UserDisplayName, updatedOne.UserDisplayName, StringComparison.Ordinal) == false)
            {
                fromDb.UserDisplayName = updatedOne.UserDisplayName;
                hadChanges = true;
            }

            if (string.Equals(fromDb.Parameters, updatedOne.Parameters, StringComparison.Ordinal) == false)
            {
                fromDb.Parameters = updatedOne.Parameters;
                hadChanges = true;
            }

            return hadChanges;
        }

        private bool ApplyOtherChanges(InstantTrigger fromDb, InstantTrigger updatedOne)
        {
            _logger.LogWarning("Cannot change an instant trigger!");

            return false;
        }

        private bool ApplyOtherChanges(object fromDb, object updatedOne)
        {
            _logger.LogWarning("Unknown trigger types: From: {fromDbType}, To: {updatedType}!", fromDb.GetType(), updatedOne.GetType());

            return false;
        }
    }
}