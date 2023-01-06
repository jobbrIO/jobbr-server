using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// In-memory job storage provider.
    /// </summary>
    public class InMemoryJobStorageProvider : IJobStorageProvider
    {
        private readonly IDictionary<string, Expression<Func<JobTriggerBase, object>>> _triggerOrderByMapping = new Dictionary<string, Expression<Func<JobTriggerBase, object>>>
        {
            { nameof(JobTriggerBase.UserDisplayName), e => e.UserDisplayName },
            { nameof(JobTriggerBase.UserId), e => e.UserId },
            { nameof(JobTriggerBase.Comment), e => e.Comment },
            { nameof(JobTriggerBase.Parameters), e => e.Parameters },
            { nameof(JobTriggerBase.Id), e => e.Id },
            { nameof(JobTriggerBase.JobId), e => e.JobId },
            { nameof(JobTriggerBase.CreatedDateTimeUtc), e => e.CreatedDateTimeUtc }
        };

        private readonly IDictionary<string, Expression<Func<Job, object>>> _jobOrderByMapping = new Dictionary<string, Expression<Func<Job, object>>>
        {
            { nameof(Job.Id), e => e.Id },
            { nameof(Job.Title), e => e.Title },
            { nameof(Job.Type), e => e.Type },
            { nameof(Job.UniqueName), e => e.UniqueName },
            { nameof(Job.CreatedDateTimeUtc), e => e.CreatedDateTimeUtc },
            { nameof(Job.UpdatedDateTimeUtc), e => e.UpdatedDateTimeUtc }
        };

        private readonly IDictionary<string, Expression<Func<JobRun, object>>> _jobRunOrderByMapping = new Dictionary<string, Expression<Func<JobRun, object>>>
        {
            { nameof(JobRun.Id), e => e.Id },
            { nameof(JobRun.ActualStartDateTimeUtc), e => e.ActualStartDateTimeUtc },
            { nameof(JobRun.InstanceParameters), e => e.InstanceParameters },
            { nameof(JobRun.JobParameters), e => e.JobParameters },
            { nameof(JobRun.PlannedStartDateTimeUtc), e => e.PlannedStartDateTimeUtc },
            { nameof(JobRun.ActualEndDateTimeUtc), e => e.ActualEndDateTimeUtc },
            { nameof(JobRun.EstimatedEndDateTimeUtc), e => e.EstimatedEndDateTimeUtc }
        };

        private readonly List<JobTriggerBase> _localTriggers = new ();
        private readonly List<Job> _localJobs = new ();
        private readonly List<JobRun> _localJobRuns = new ();

        /// <summary>
        /// Get job triggers by job ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted job triggers.</param>
        /// <returns>Paged list of job triggers.</returns>
        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            var enumerable = _localTriggers.Where(t => t.JobId == jobId);

            enumerable = ApplyFiltersAndPaging(page, pageSize, null, null, null, enumerable, out var totalItems);

            var list = enumerable.ToList().Clone();

            return new PagedResult<JobTriggerBase>
            {
                Page = page,
                PageSize = pageSize,
                Items = list,
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get active triggers.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type.</param>
        /// <param name="jobUniqueNameFilter">Job unique name.</param>
        /// <param name="query">Query.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>A paged list result of active triggers.</returns>
        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var enumerable = _localTriggers.Where(t => t.IsActive);

            enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, enumerable, out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.CreatedDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _triggerOrderByMapping);
            }

            var list = enumerable.ToList().Clone();

            return new PagedResult<JobTriggerBase>
            {
                Page = page,
                PageSize = pageSize,
                Items = list,
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Included deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of job runs.</returns>
        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, _localJobRuns, out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs by job ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted job runs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of job runs.</returns>
        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            var enumerable = ApplyFiltersAndPaging(page, pageSize, null, null, null, _localJobRuns, out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Include deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of job runs.</returns>
        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            var enumerable = ApplyFiltersAndPaging(page, pageSize, null, null, null, _localJobRuns.Where(p => p.Trigger.Id == triggerId), out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get trigger by ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <returns>Found job trigger.</returns>
        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return _localTriggers.FirstOrDefault(t => t.Id == triggerId).Clone();
        }

        /// <summary>
        /// Get last job run by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">Current time in UTC.</param>
        /// <returns>Last job run.</returns>
        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return _localJobRuns.FirstOrDefault(jr => jr.Trigger.Id == triggerId && jr.ActualEndDateTimeUtc < utcNow).Clone();
        }

        /// <summary>
        /// Get next job run by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">Current time in UTC.</param>
        /// <returns>Next job run.</returns>
        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return _localJobRuns.FirstOrDefault(jr => jr.Trigger.Id == triggerId && jr.PlannedStartDateTimeUtc >= utcNow).Clone();
        }

        /// <summary>
        /// Get job runs by state.
        /// </summary>
        /// <param name="state">State.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of job runs.</returns>
        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, _localJobRuns.Where(p => p.State == state), out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs by states.
        /// </summary>
        /// <param name="states">States.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of job runs.</returns>
        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, _localJobRuns.Where(p => states.Contains(p.State)), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="showDeleted">Include deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Found job runs as a paged list.</returns>
        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, null, _localJobRuns.Where(p => string.Equals(p.Trigger.UserId, userId, StringComparison.OrdinalIgnoreCase)), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.Id);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get job runs by user display name.
        /// </summary>
        /// <param name="userDisplayName">User display name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="showDeleted">Include deleted job runs in the result.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Found job runs as a paged result.</returns>
        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, null, _localJobRuns.Where(p => string.Equals(p.Trigger.UserDisplayName, userDisplayName, StringComparison.OrdinalIgnoreCase)), out var totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.Id);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Search jobs.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Query.</param>
        /// <param name="showDeleted">Include deleted jobs.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Found jobs as a paged result.</returns>
        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, _localJobs, out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.CreatedDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, _jobOrderByMapping);
            }

            return new PagedResult<Job>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        /// <summary>
        /// Get by job ID.
        /// </summary>
        /// <param name="id">Job ID.</param>
        /// <returns>The job.</returns>
        public Job GetJobById(long id)
        {
            return _localJobs.FirstOrDefault(j => j.Id == id).Clone();
        }

        /// <summary>
        /// Get job by unique name.
        /// </summary>
        /// <param name="identifier">Unique name.</param>
        /// <returns>The found job.</returns>
        public Job GetJobByUniqueName(string identifier)
        {
            return _localJobs.FirstOrDefault(j => string.Equals(j.UniqueName, identifier, StringComparison.OrdinalIgnoreCase)).Clone();
        }

        /// <summary>
        /// Get job run by job run ID.
        /// </summary>
        /// <param name="id">Job run ID.</param>
        /// <returns>Found job run or null.</returns>
        public JobRun GetJobRunById(long id)
        {
            return _localJobRuns.FirstOrDefault(j => j.Id == id).Clone();
        }

        /// <summary>
        /// Add job.
        /// </summary>
        /// <param name="job">Job to add.</param>
        public void AddJob(Job job)
        {
            var maxJobId = _localJobs.Count + 1;
            job.Id = maxJobId;
            _localJobs.Add(job);
        }

        /// <summary>
        /// Add recurring trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to add.</param>
        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            var newTriggerId = _localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// Add instant trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to add.</param>
        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            var newTriggerId = _localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// Add scheduled trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to add.</param>
        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            var newTriggerId = _localTriggers.Count + 1;
            trigger.Id = newTriggerId;
            trigger.JobId = jobId;
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// Add job run.
        /// </summary>
        /// <param name="jobRun">Job run to add.</param>
        public void AddJobRun(JobRun jobRun)
        {
            var maxJobRunId = _localJobRuns.Count + 1;

            jobRun.Id = maxJobRunId;

            _localJobRuns.Add(jobRun);
        }

        /// <summary>
        /// Disable trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        public void DisableTrigger(long jobId, long triggerId)
        {
            var trigger = _localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = false;
        }

        /// <summary>
        /// Enable trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        public void EnableTrigger(long jobId, long triggerId)
        {
            var trigger = _localTriggers.Single(t => t.Id == triggerId);
            trigger.IsActive = true;
        }

        /// <summary>
        /// Delete trigger. Unimplemented.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <exception cref="NotImplementedException">Throws always.</exception>
        public void DeleteTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Delete job. Unimplemented.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <exception cref="NotImplementedException">Throws always.</exception>
        public void DeleteJob(long jobId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update job run.
        /// </summary>
        /// <param name="jobRun">New job run data.</param>
        public void Update(JobRun jobRun)
        {
            _localJobRuns.Remove(_localJobRuns.FirstOrDefault(jr => jr.Id == jobRun.Id));
            _localJobRuns.Add(jobRun);
        }

        /// <summary>
        /// Update job run progress.
        /// </summary>
        /// <param name="jobRunId">Job run ID.</param>
        /// <param name="progress">Job run progress.</param>
        public void UpdateProgress(long jobRunId, double? progress)
        {
            var jobRun = _localJobRuns.First(p => p.Id == jobRunId);
            jobRun.Progress = progress;
        }

        /// <summary>
        /// Apply retention. Unimplemented.
        /// </summary>
        /// <param name="date">Offset.</param>
        /// <exception cref="NotImplementedException">Throws always.</exception>
        public void ApplyRetention(DateTimeOffset date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update job.
        /// </summary>
        /// <param name="job">New job information.</param>
        public void Update(Job job)
        {
            _localJobs.Remove(_localJobs.FirstOrDefault(j => j.Id == job.Id));
            _localJobs.Add(job);
        }

        /// <summary>
        /// Update an instant trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to update.</param>
        public void Update(long jobId, InstantTrigger trigger)
        {
            _localTriggers.Remove(_localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// Update a scheduled trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to update.</param>
        public void Update(long jobId, ScheduledTrigger trigger)
        {
            _localTriggers.Remove(_localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// Update a recurring trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="trigger">Trigger to update.</param>
        public void Update(long jobId, RecurringTrigger trigger)
        {
            _localTriggers.Remove(_localTriggers.FirstOrDefault(j => j.Id == trigger.Id));
            _localTriggers.Add(trigger);
        }

        /// <summary>
        /// If in-memory provider is available.
        /// </summary>
        /// <returns>If available.</returns>
        public bool IsAvailable()
        {
            return true;
        }

        /// <summary>
        /// Get jobs count in storage.
        /// </summary>
        /// <returns>Jobs count.</returns>
        public long GetJobsCount()
        {
            return _localJobs.Count;
        }

        private IEnumerable<JobTriggerBase> ApplyFiltersAndPaging(int page, int pageSize, string jobTypeFilter, string jobUniqueNameFilter, string query, IEnumerable<JobTriggerBase> enumerable, out int totalItems)
        {
            if (string.IsNullOrWhiteSpace(jobTypeFilter) == false)
            {
                var jobs = _localJobs.Where(p => string.Equals(p.Type, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
                var jobIds = jobs.Select(s => s.Id).ToList();

                enumerable = enumerable.Where(p => jobIds.Contains(p.JobId));
            }

            if (string.IsNullOrWhiteSpace(jobUniqueNameFilter) == false)
            {
                var jobs = _localJobs.Where(p => string.Equals(p.UniqueName, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
                var jobIds = jobs.Select(s => s.Id).ToList();

                enumerable = enumerable.Where(p => jobIds.Contains(p.JobId));
            }

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                enumerable = enumerable.Where(p => p.Parameters.Contains(query) || p.UserDisplayName.Contains(query) || p.UserId.Contains(query));
            }

            totalItems = enumerable.Count();

            enumerable = enumerable.Skip((page - 1) * pageSize);
            enumerable = enumerable.Take(pageSize);

            return enumerable;
        }

        private IEnumerable<Job> ApplyFiltersAndPaging(int page, int pageSize, string jobTypeFilter, string jobUniqueNameFilter, string query, IEnumerable<Job> enumerable, out int totalItems)
        {
            if (string.IsNullOrWhiteSpace(jobTypeFilter) == false)
            {
                enumerable = enumerable.Where(p => string.Equals(p.Type, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(jobUniqueNameFilter) == false)
            {
                enumerable = enumerable.Where(p => string.Equals(p.UniqueName, jobUniqueNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                enumerable = enumerable.Where(p => p.Parameters.Contains(query) || p.Title.Contains(query) || p.Type.Contains(query) || p.UniqueName.Contains(query));
            }

            totalItems = enumerable.Count();

            enumerable = enumerable.Skip((page - 1) * pageSize);
            enumerable = enumerable.Take(pageSize);

            return enumerable;
        }

        private IEnumerable<JobRun> ApplyFiltersAndPaging(int page, int pageSize, string jobTypeFilter, string jobUniqueNameFilter, string query, IEnumerable<JobRun> enumerable, out int totalItems)
        {
            if (string.IsNullOrWhiteSpace(jobTypeFilter) == false)
            {
                enumerable = enumerable.Where(p => string.Equals(p.Job.Type, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(jobUniqueNameFilter) == false)
            {
                enumerable = enumerable.Where(p => string.Equals(p.Job.UniqueName, jobUniqueNameFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (string.IsNullOrWhiteSpace(query) == false)
            {
                enumerable = enumerable.Where(p => p.InstanceParameters.Contains(query) || p.JobParameters.Contains(query) || p.Job.Title.Contains(query) || p.Job.Type.Contains(query) || p.Job.UniqueName.Contains(query));
            }

            totalItems = enumerable.Count();

            enumerable = enumerable.Skip((page - 1) * pageSize);
            enumerable = enumerable.Take(pageSize);

            return enumerable;
        }

        private static IEnumerable<T> ApplySorting<T>(string[] sort, IEnumerable<T> enumerable, IDictionary<string, Expression<Func<T, object>>> mapping)
        {
            for (var i = 0; i < sort.Length; ++i)
            {
                var sortProperty = sort[i];
                bool ascending = true;

                if (sortProperty.StartsWith("-"))
                {
                    sortProperty = sortProperty.TrimStart('-');
                    @ascending = false;
                }

                Expression<Func<T, object>> expression;

                if (mapping.TryGetValue(sortProperty, out expression) == false)
                {
                    continue;
                }

                var compiled = expression.Compile();

                if (i == 0)
                {
                    if (@ascending)
                    {
                        enumerable = enumerable.OrderBy(compiled);
                    }
                    else
                    {
                        enumerable = enumerable.OrderByDescending(compiled);
                    }
                }
                else
                {
                    var orderedEnumerable = (IOrderedEnumerable<T>)enumerable;

                    if (@ascending)
                    {
                        enumerable = orderedEnumerable.ThenBy(compiled);
                    }
                    else
                    {
                        enumerable = orderedEnumerable.ThenByDescending(compiled);
                    }
                }
            }

            return enumerable;
        }
    }
}
