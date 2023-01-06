using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.ComponentServices.Management
{
    /// <summary>
    /// Internal class for querying job information.
    /// </summary>
    internal class JobQueryService : IQueryService
    {
        private readonly IJobbrRepository _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobQueryService"/> class.
        /// </summary>
        /// <param name="repository">The Jobbr repository for querying.</param>
        /// <param name="mapper">Object mapper.</param>
        public JobQueryService(IJobbrRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Get a <see cref="Job"/> by job ID.
        /// </summary>
        /// <param name="id"><see cref="Job"/> ID.</param>
        /// <returns>The target <see cref="Job"/>.</returns>
        public Job GetJobById(long id)
        {
            var job = _repository.GetJob(id);

            return _mapper.Map<Job>(job);
        }

        /// <summary>
        /// Get a <see cref="Job"/> by unique name.
        /// </summary>
        /// <param name="uniqueName"><see cref="Job"/>'s unique name.</param>
        /// <returns>The target <see cref="Job"/>.</returns>
        public Job GetJobByUniqueName(string uniqueName)
        {
            var job = _repository.GetJobByUniqueName(uniqueName);

            return _mapper.Map<Job>(job);
        }

        /// <summary>
        /// Get a list of <see cref="Job"/>s.
        /// </summary>
        /// <param name="page">Page of <see cref="Job"/>s.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter"><see cref="Job"/> type filter.</param>
        /// <param name="jobUniqueNameFilter">Unique <see cref="Job"/> name filter.</param>
        /// <param name="query">Search query.</param>
        /// <param name="showDeleted">If deleted jobs should be included.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged <see cref="Job"/> result.</returns>
        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var jobs = _repository.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);

            return new PagedResult<Job>
            {
                Items = _mapper.Map<List<Job>>(jobs.Items),
                TotalItems = jobs.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get a list of <see cref="IJobTrigger"/>s.
        /// </summary>
        /// <param name="page">Page of <see cref="IJobTrigger"/>s.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter"><see cref="Job"/> type filter.</param>
        /// <param name="jobUniqueNameFilter">Unique <see cref="Job"/> name filter.</param>
        /// <param name="query">Search query.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged <see cref="Job"/> result.</returns>
        public PagedResult<IJobTrigger> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var triggers = _repository.GetActiveTriggers(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<IJobTrigger>
            {
                Items = _mapper.Map<List<IJobTrigger>>(triggers.Items),
                TotalItems = triggers.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="IJobTrigger"/> with job and trigger ID.
        /// </summary>
        /// <param name="jobId"><see cref="Job"/> ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <returns>Target <see cref="IJobTrigger"/>.</returns>
        public IJobTrigger GetTriggerById(long jobId, long triggerId)
        {
            var trigger = _repository.GetTriggerById(jobId, triggerId);

            return _mapper.Map<IJobTrigger>(trigger);
        }

        /// <summary>
        /// Get a list of <see cref="IJobTrigger"/>s with <see cref="Job"/> ID.
        /// </summary>
        /// <param name="jobId"><see cref="Job"/> ID.</param>
        /// <param name="page">Page of <see cref="IJobTrigger"/>s.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">If deleted triggers are included.</param>
        /// <returns>Paged <see cref="IJobTrigger"/> result.</returns>
        public PagedResult<IJobTrigger> GetTriggersByJobId(long jobId, int page, int pageSize = 50, bool showDeleted = false)
        {
            var triggers = _repository.GetTriggersByJobId(jobId, page, pageSize, showDeleted);

            return new PagedResult<IJobTrigger>
            {
                Items = _mapper.Map<List<IJobTrigger>>(triggers.Items),
                TotalItems = triggers.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/> by ID.
        /// </summary>
        /// <param name="id"><see cref="JobRun"/> ID.</param>
        /// <returns>Target <see cref="JobRun"/>.</returns>
        public JobRun GetJobRunById(long id)
        {
            var jobRun = _repository.GetJobRunById(id);

            return _mapper.Map<JobRun>(jobRun);
        }

        /// <summary>
        /// Get a list of <see cref="JobRun"/>s based on parameters.
        /// </summary>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Filter for unique <see cref="Job"/> names.</param>
        /// <param name="query">Search query.</param>
        /// <param name="showDeleted">Show deleted <see cref="JobRun"/>s.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of <see cref="JobRun"/>s.</returns>
        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRuns(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get job runs by <see cref="Job"/> ID.
        /// </summary>
        /// <param name="jobId"><see cref="Job"/> ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">If deleted job runs should be included.</param>
        /// <param name="sort">Sort.</param>
        /// <returns><see cref="JobRun"/>s as a paged result.</returns>
        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRunsByJobId(jobId, page, pageSize, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/>s by user ID.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="showDeleted">Show deleted <see cref="JobRun"/>s.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of <see cref="JobRun"/>s.</returns>
        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/>s by trigger ID.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="showDeleted">Show deleted <see cref="JobRun"/>s.</param>
        /// <param name="sort">Sort.</param>
        /// <returns><see cref="JobRun"/>s as a paged result.</returns>
        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/>s by user display name.
        /// </summary>
        /// <param name="userDisplayName">User display name.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter"><see cref="Job"/> type filter.</param>
        /// <param name="jobUniqueNameFilter"><see cref="Job"/> unique name filter.</param>
        /// <param name="showDeleted">Show deleted <see cref="JobRun"/>s.</param>
        /// <param name="sort">Sort.</param>
        /// <returns><see cref="JobRun"/>s as a paged result.</returns>
        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/>s by state.
        /// </summary>
        /// <param name="state">Job run state.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Search query.</param>
        /// <param name="showDeleted">If deleted <see cref="JobRun"/>s should be included.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of <see cref="JobRun"/>s.</returns>
        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var jobruns = _repository.GetJobRunsByState((ComponentModel.JobStorage.Model.JobRunStates)state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Get <see cref="JobRun"/>s by states.
        /// </summary>
        /// <param name="states">List of job run states.</param>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="jobTypeFilter">Job type filter.</param>
        /// <param name="jobUniqueNameFilter">Job unique name filter.</param>
        /// <param name="query">Search query.</param>
        /// <param name="showDeleted">If deleted <see cref="JobRun"/>s should be included.</param>
        /// <param name="sort">Sort.</param>
        /// <returns>Paged result of <see cref="JobRun"/>s.</returns>
        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, bool showDeleted = false, params string[] sort)
        {
            var statesCast = states.Select(s => (ComponentModel.JobStorage.Model.JobRunStates)s).ToArray();

            var jobruns = _repository.GetJobRunsByStates(statesCast, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, showDeleted, sort);

            return new PagedResult<JobRun>
            {
                Items = _mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        /// <summary>
        /// Last <see cref="JobRun"/> by trigger ID.
        /// </summary>
        /// <param name="jobId"><see cref="Job"/> ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">Timestamp in UTC.</param>
        /// <returns>Last <see cref="JobRun"/> with the trigger ID.</returns>
        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = _repository.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);

            return _mapper.Map<JobRun>(jobrun);
        }

        /// <summary>
        /// Next <see cref="JobRun"/> by trigger ID.
        /// </summary>
        /// <param name="jobId"><see cref="Job"/> ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="utcNow">Timestamp in UTC.</param>
        /// <returns>Next <see cref="JobRun"/> with the trigger ID.</returns>
        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = _repository.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);

            return _mapper.Map<JobRun>(jobrun);
        }
    }
}