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
        private readonly IJobbrRepository repository;
        private readonly IMapper mapper;

        public JobQueryService(IJobbrRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public Job GetJobById(long id)
        {
            var job = this.repository.GetJob(id);

            return this.mapper.Map<Job>(job);
        }

        public Job GetJobByUniqueName(string uniqueName)
        {
            var job = this.repository.GetJobByUniqueName(uniqueName);

            return this.mapper.Map<Job>(job);
        }

        public PagedResult<Job> GetJobs(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var jobs = this.repository.GetJobs(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<Job>
            {
                Items = this.mapper.Map<List<Job>>(jobs.Items),
                TotalItems = jobs.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<IJobTrigger> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var triggers = this.repository.GetActiveTriggers(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<IJobTrigger>
            {
                Items = this.mapper.Map<List<IJobTrigger>>(triggers.Items),
                TotalItems = triggers.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public IJobTrigger GetTriggerById(long jobId, long triggerId)
        {
            var trigger = this.repository.GetTriggerById(jobId, triggerId);

            return this.mapper.Map<IJobTrigger>(trigger);
        }

        public PagedResult<IJobTrigger> GetTriggersByJobId(long jobId, int page, int pageSize = 50)
        {
            var triggers = this.repository.GetTriggersByJobId(jobId, page, pageSize);

            return new PagedResult<IJobTrigger>
            {
                Items = this.mapper.Map<List<IJobTrigger>>(triggers.Items),
                TotalItems = triggers.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public JobRun GetJobRunById(long id)
        {
            var jobRun = this.repository.GetJobRunById(id);

            return this.mapper.Map<JobRun>(jobRun);
        }

        public PagedResult<JobRun> GetJobRuns(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var jobruns = this.repository.GetJobRuns(page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByJobId(int jobId, int page = 1, int pageSize = 50, params string[] sort)
        {
            var jobruns = this.repository.GetJobRunsByJobId(jobId, page, pageSize, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByUserId(string userId, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            var jobruns = this.repository.GetJobRunsByUserId(userId, page, pageSize, jobTypeFilter, jobUniqueNameFilter, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, int page = 1, int pageSize = 50, params string[] sort)
        {
            var jobruns = this.repository.GetJobRunsByTriggerId(jobId, triggerId, page, pageSize, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, params string[] sort)
        {
            var jobruns = this.repository.GetJobRunsByUserDisplayName(userDisplayName, page, pageSize, jobTypeFilter, jobUniqueNameFilter, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByState(JobRunStates state, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var jobruns = this.repository.GetJobRunsByState((ComponentModel.JobStorage.Model.JobRunStates)state, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<JobRun> GetJobRunsByStates(JobRunStates[] states, int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            var statesCasted = states.Cast<ComponentModel.JobStorage.Model.JobRunStates>().ToArray();

            var jobruns = this.repository.GetJobRunsByStates(statesCasted, page, pageSize, jobTypeFilter, jobUniqueNameFilter, query, sort);

            return new PagedResult<JobRun>
            {
                Items = this.mapper.Map<List<JobRun>>(jobruns.Items),
                TotalItems = jobruns.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = this.repository.GetLastJobRunByTriggerId(jobId, triggerId, utcNow);

            return this.mapper.Map<JobRun>(jobrun);
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            var jobrun = this.repository.GetNextJobRunByTriggerId(jobId, triggerId, utcNow);

            return this.mapper.Map<JobRun>(jobrun);
        }
    }
}