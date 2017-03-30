using System.Collections.Generic;
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

        public List<Job> GetJobs(int page = 0, int pageSize = 50)
        {
            var jobs = this.repository.GetJobs();

            return this.mapper.Map<List<Job>>(jobs);
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

        public List<IJobTrigger> GetActiveTriggers()
        {
            var triggers = this.repository.GetActiveTriggers();

            return this.mapper.Map<List<IJobTrigger>>(triggers);
        }

        public List<JobRun> GetJobRuns(long page = 0, long pageSize = 50)
        {
            var jobRuns = this.repository.GetJobRuns();

            return this.mapper.Map<List<JobRun>>(jobRuns);
        }

        public JobRun GetJobRunById(long id)
        {
            var jobRun = this.repository.GetJobRunById(id);

            return this.mapper.Map<JobRun>(jobRun);
        }

        public List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId)
        {
            var jobRun = this.repository.GetJobRunsByTriggerId(jobId, triggerId);

            return this.mapper.Map<List<JobRun>>(jobRun);
        }

        public List<JobRun> GetJobRunsByUserIdOrderByIdDesc(string userId)
        {
            var jobRun = this.repository.GetJobRunsForUserId(userId);

            return this.mapper.Map<List<JobRun>>(jobRun);
        }

        public List<JobRun> GetJobRunsByUserDisplayNameOrderByIdDesc(string userName)
        {
            var jobRun = this.repository.GetJobRunsByUserDisplayName(userName);

            return this.mapper.Map<List<JobRun>>(jobRun);
        }
    }
}