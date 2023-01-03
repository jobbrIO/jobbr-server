using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class JobQueryService : IQueryService
    {
        private readonly IJobbrRepository _repository;
        private readonly IMapper _mapper;

        public JobQueryService(IJobbrRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public Job GetJobById(long id)
        {
            var job = _repository.GetJob(id);

            return _mapper.Map<Job>(job);
        }

        public Job GetJobByUniqueName(string uniqueName)
        {
            var job = _repository.GetJobByUniqueName(uniqueName);

            return _mapper.Map<Job>(job);
        }

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

        public IJobTrigger GetTriggerById(long jobId, long triggerId)
        {
            var trigger = _repository.GetTriggerById(jobId, triggerId);

            return _mapper.Map<IJobTrigger>(trigger);
        }

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

        public JobRun GetJobRunById(long id)
        {
            var jobRun = _repository.GetJobRunById(id);

            return _mapper.Map<JobRun>(jobRun);
        }

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

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = _repository.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);

            return _mapper.Map<JobRun>(jobrun);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = _repository.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);

            return _mapper.Map<JobRun>(jobrun);
        }
    }
}