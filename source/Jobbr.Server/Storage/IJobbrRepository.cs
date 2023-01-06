using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// Jobbr repository.
    /// </summary>
    public interface IJobbrRepository
    {
        /// <summary>
        /// Search jobs.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">If deleted jobs should be included.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list result of jobs matching the search criteria.</returns>
        PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Get job based on ID.
        /// </summary>
        /// <param name="id">Job ID.</param>
        /// <returns>Job matching the ID.</returns>
        Job GetJob(long id);

        /// <summary>
        /// Update job run progress.
        /// </summary>
        /// <param name="jobRunId">Job run ID.</param>
        /// <param name="progress">Progress amount.</param>
        void UpdateJobRunProgress(long jobRunId, double progress);

        /// <summary>
        /// Set process ID for job run.
        /// </summary>
        /// <param name="jobRun">Job run.</param>
        /// <param name="id">Process ID.</param>
        void SetPidForJobRun(JobRun jobRun, int id);

        /// <summary>
        /// Get job run based on ID.
        /// </summary>
        /// <param name="id">Job run ID.</param>
        /// <returns>Found <see cref="JobRun"/>.</returns>
        JobRun GetJobRun(long id);

        /// <summary>
        /// Save and add a recurring trigger to a <see cref="Job"/>.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to save.</param>
        void SaveAddTrigger(long jobId, RecurringTrigger trigger);

        /// <summary>
        /// Update planned start time (UTC).
        /// </summary>
        /// <param name="jobRunId">Job run ID.</param>
        /// <param name="plannedStartDateTimeUtc">New planned start time in UTC.</param>
        void UpdatePlannedStartDateTimeUtc(long jobRunId, DateTime plannedStartDateTimeUtc);

        /// <summary>
        /// Save and add a scheduled trigger to a <see cref="Job"/>.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to save.</param>
        void SaveAddTrigger(long jobId, ScheduledTrigger trigger);

        /// <summary>
        /// Save and add an instant trigger to a <see cref="Job"/>.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to save.</param>
        void SaveAddTrigger(long jobId, InstantTrigger trigger);

        /// <summary>
        /// Enable a trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void EnableTrigger(long jobId, long triggerId);

        /// <summary>
        /// Get active triggers.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of active triggers.</returns>
        PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort);

        /// <summary>
        /// Update a trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger info.</param>
        /// <param name="hadChanges">If info has been changed.</param>
        /// <returns>The trigger that was updated.</returns>
        JobTriggerBase SaveUpdateTrigger(long jobId, JobTriggerBase trigger, out bool hadChanges);

        /// <summary>
        /// Get running jobs.
        /// </summary>
        /// <param name="triggerJobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <returns>List of running jobs.</returns>
        IEnumerable<JobRun> GetRunningJobs(long triggerJobId, long triggerId);

        /// <summary>
        /// Get all running jobs.
        /// </summary>
        /// <returns>All running jobs.</returns>
        IEnumerable<JobRun> GetRunningJobs();

        /// <summary>
        /// Get job runs within certain states.
        /// </summary>
        /// <param name="minState">Minimum state.</param>
        /// <param name="maxState">Maximum state.</param>
        /// <returns>List of job runs that are between the states.</returns>
        IEnumerable<JobRun> GetJobRunsByStateRange(JobRunStates minState, JobRunStates maxState);

        /// <summary>
        /// Add a <see cref="Job"/>.
        /// </summary>
        /// <param name="job">Job to add.</param>
        void AddJob(Job job);

        /// <summary>
        /// Save a new job run.
        /// </summary>
        /// <param name="job">Job.</param>
        /// <param name="trigger">Job trigger.</param>
        /// <param name="plannedStartDateTimeUtc">Planned start time in UTC.</param>
        /// <returns>The new <see cref="JobRun"/>.</returns>
        JobRun SaveNewJobRun(Job job, JobTriggerBase trigger, DateTime plannedStartDateTimeUtc);

        /// <summary>
        /// Disable trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void DisableTrigger(long jobId, long triggerId);

        /// <summary>
        /// Delete trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void DeleteTrigger(long jobId, long triggerId);

        /// <summary>
        /// Update job run.
        /// </summary>
        /// <param name="jobRun">Updated information.</param>
        void Update(JobRun jobRun);

        /// <summary>
        /// Get job run.
        /// </summary>
        /// <param name="jobRunId">Job run ID.</param>
        /// <returns>Found job run.</returns>
        JobRun GetJobRunById(long jobRunId);

        /// <summary>
        /// Get trigger with ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <returns>The job trigger.</returns>
        JobTriggerBase GetTriggerById(long jobId, long triggerId);

        /// <summary>
        /// Get triggers by job ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted triggers.</param>
        /// <returns>Paged result of triggers.</returns>
        PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page, int pageSize = 50, bool showDeleted = false);

        /// <summary>
        /// Get job by it's unique name.
        /// </summary>
        /// <param name="identifier">Unique name.</param>
        /// <returns>The target job.</returns>
        Job GetJobByUniqueName(string identifier);

        /// <summary>
        /// Delete job run.
        /// </summary>
        /// <param name="jobRun">Job run to delete.</param>
        void Delete(JobRun jobRun);

        /// <summary>
        /// Search job runs.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with job ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Type filter for job.</param>
        /// <param name="jobUniqueNameFilter">Unique name filter for job.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with user display name.
        /// </summary>
        /// <param name="userDisplayName">User display name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Type filter for job.</param>
        /// <param name="jobUniqueNameFilter">Unique name filter for job.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with state.
        /// </summary>
        /// <param name="state">Job run state.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Type filter for job.</param>
        /// <param name="jobUniqueNameFilter">Unique name filter for job.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Search job runs with states.
        /// </summary>
        /// <param name="states">Job run states.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Type filter for job.</param>
        /// <param name="jobUniqueNameFilter">Unique name filter for job.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list of search result job runs.</returns>
        PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort);

        /// <summary>
        /// Get last job run by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">UTC time.</param>
        /// <returns>The last job run matching the search terms.</returns>
        JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);

        /// <summary>
        /// Get next job run by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">UTC time.</param>
        /// <returns>The next job run matching the search terms.</returns>
        JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow);
    }
}