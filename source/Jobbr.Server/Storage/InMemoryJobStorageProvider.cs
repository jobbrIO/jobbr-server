using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Storage
{
    public class InMemoryJobStorageProvider : IJobStorageProvider
    {
        private readonly IDictionary<string, Expression<Func<JobTriggerBase, object>>> TriggerOrderByMapping = new Dictionary<string, Expression<Func<JobTriggerBase, object>>>
        {
            {nameof(JobTriggerBase.UserDisplayName), e => e.UserDisplayName},
            {nameof(JobTriggerBase.UserId), e => e.UserId},
            {nameof(JobTriggerBase.Comment), e => e.Comment},
            {nameof(JobTriggerBase.Parameters), e => e.Parameters},
            {nameof(JobTriggerBase.Id), e => e.Id},
            {nameof(JobTriggerBase.JobId), e => e.JobId},
            {nameof(JobTriggerBase.CreatedDateTimeUtc), e => e.CreatedDateTimeUtc},
        };

        private readonly IDictionary<string, Expression<Func<Job, object>>> JobOrderByMapping = new Dictionary<string, Expression<Func<Job, object>>>
        {
            {nameof(Job.Id), e => e.Id},
            {nameof(Job.Title), e => e.Title},
            {nameof(Job.Type), e => e.Type},
            {nameof(Job.UniqueName), e => e.UniqueName},
            {nameof(Job.CreatedDateTimeUtc), e => e.CreatedDateTimeUtc},
            {nameof(Job.UpdatedDateTimeUtc), e => e.UpdatedDateTimeUtc},
        };

        private readonly IDictionary<string, Expression<Func<JobRun, object>>> JobRunOrderByMapping = new Dictionary<string, Expression<Func<JobRun, object>>>
        {
            {nameof(JobRun.Id), e => e.Id},
            {nameof(JobRun.ActualStartDateTimeUtc), e => e.ActualStartDateTimeUtc},
            {nameof(JobRun.InstanceParameters), e => e.InstanceParameters},
            {nameof(JobRun.JobParameters), e => e.JobParameters},
            {nameof(JobRun.PlannedStartDateTimeUtc), e => e.PlannedStartDateTimeUtc},
            {nameof(JobRun.ActualEndDateTimeUtc), e => e.ActualEndDateTimeUtc},
            {nameof(JobRun.EstimatedEndDateTimeUtc), e => e.EstimatedEndDateTimeUtc},
        };

        private readonly List<JobTriggerBase> localTriggers = new List<JobTriggerBase>();

        private readonly List<Job> localJobs = new List<Job>();

        private readonly List<JobRun> localJobRuns = new List<JobRun>();

        public PagedResult<JobTriggerBase> GetTriggersByJobId(long jobId, int page = 1, int pageSize = 50, bool showDeleted = false)
        {
            var enumerable = this.localTriggers.Where(t => t.JobId == jobId);

            int totalItems;

            enumerable = this.ApplyFiltersAndPaging(page, pageSize, null, null, null, enumerable, out totalItems);

            var list = enumerable.ToList().Clone();

            return new PagedResult<JobTriggerBase>
            {
                Page = page,
                PageSize = pageSize,
                Items = list,
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobTriggerBase> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var enumerable = this.localTriggers.Where(t => t.IsActive);

            int totalItems;

            enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, enumerable, out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.CreatedDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.TriggerOrderByMapping);
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

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, this.localJobRuns, out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, null, null, null, this.localJobRuns, out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, null, null, null, this.localJobRuns.Where(p => p.Trigger.Id == triggerId), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            return this.localTriggers.FirstOrDefault(t => t.Id == triggerId).Clone();
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.Trigger.Id == triggerId && jr.ActualEndDateTimeUtc < utcNow).Clone();
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            return this.localJobRuns.FirstOrDefault(jr => jr.Trigger.Id == triggerId && jr.PlannedStartDateTimeUtc >= utcNow).Clone();
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, this.localJobRuns.Where(p => p.State == state), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, this.localJobRuns.Where(p => states.Contains(p.State)), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.PlannedStartDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, null, this.localJobRuns.Where(p => string.Equals(p.Trigger.UserId, userId, StringComparison.OrdinalIgnoreCase)), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.Id);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems; 

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, null, this.localJobRuns.Where(p => string.Equals(p.Trigger.UserDisplayName, userDisplayName, StringComparison.OrdinalIgnoreCase)), out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.Id);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobRunOrderByMapping);
            }

            return new PagedResult<JobRun>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            int totalItems;

            var enumerable = this.ApplyFiltersAndPaging(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, this.localJobs, out totalItems);

            if (sort == null || sort.Length == 0)
            {
                enumerable = enumerable.OrderByDescending(o => o.CreatedDateTimeUtc);
            }
            else
            {
                enumerable = ApplySorting(sort, enumerable, this.JobOrderByMapping);
            }

            return new PagedResult<Job>
            {
                Page = page,
                PageSize = pageSize,
                Items = enumerable.ToList(),
                TotalItems = totalItems,
            };
        }

        public Job GetJobById(long id)
        {
            return this.localJobs.FirstOrDefault(j => j.Id == id).Clone();
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return this.localJobs.FirstOrDefault(j => string.Equals(j.UniqueName, identifier, StringComparison.OrdinalIgnoreCase)).Clone();
        }

        public JobRun GetJobRunById(long id)
        {
            return this.localJobRuns.FirstOrDefault(j => j.Id == id).Clone();
        }

        public void AddJob(Job job)
        {
            var maxJobId = this.localJobs.Count + 1;
            job.Id = maxJobId;
            this.localJobs.Add(job);
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

        public void AddJobRun(JobRun jobRun)
        {
            var maxJobRunId = this.localJobRuns.Count + 1;

            jobRun.Id = maxJobRunId;

            this.localJobRuns.Add(jobRun);
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

        public void DeleteTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void DeleteJob(long jobId)
        {
            throw new NotImplementedException();
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

        private IEnumerable<JobTriggerBase> ApplyFiltersAndPaging(int page, int pageSize, string jobTypeFilter, string jobUniqueNameFilter, string query, IEnumerable<JobTriggerBase> enumerable, out int totalItems)
        {
            if (string.IsNullOrWhiteSpace(jobTypeFilter) == false)
            {
                var jobs = this.localJobs.Where(p => string.Equals(p.Type, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
                var jobIds = jobs.Select(s => s.Id).ToList();

                enumerable = enumerable.Where(p => jobIds.Contains(p.JobId));
            }

            if (string.IsNullOrWhiteSpace(jobUniqueNameFilter) == false)
            {
                var jobs = this.localJobs.Where(p => string.Equals(p.UniqueName, jobTypeFilter, StringComparison.OrdinalIgnoreCase));
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
                    var orderedEnumerable = (IOrderedEnumerable<T>) enumerable;

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
