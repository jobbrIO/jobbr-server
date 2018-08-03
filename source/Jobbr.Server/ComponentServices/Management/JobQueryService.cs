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
            var jobs = this.repository.GetJobs();

            return new PagedResult<Job>
            {
                Items = this.mapper.Map<List<Job>>(jobs),
                TotalItems = jobs.TotalItems,
                Page = page,
                PageSize = pageSize
            };
        }

        public PagedResult<IJobTrigger> GetActiveTriggers(int page = 1, int pageSize = 50, string jobTypeFilter = null, string jobUniqueNameFilter = null, string query = null, params string[] sort)
        {
            throw new NotImplementedException();
        }

        public IJobTrigger GetTriggerById(long jobId, long triggerId)
        {
            var trigger = this.repository.GetTriggerById(jobId, triggerId);

            return this.mapper.Map<IJobTrigger>(trigger);
        }

        public List<IJobTrigger> GetTriggersByJobId(long jobId)
        {
            var triggers = this.repository.GetTriggersByJobId(jobId);

            return this.mapper.Map<List<IJobTrigger>>(triggers);
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
                Items = this.mapper.Map<List<JobRun>>(jobruns),
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
                Items = this.mapper.Map<List<JobRun>>(jobruns),
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
                Items = this.mapper.Map<List<JobRun>>(jobruns),
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
                Items = this.mapper.Map<List<JobRun>>(jobruns),
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
                Items = this.mapper.Map<List<JobRun>>(jobruns),
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