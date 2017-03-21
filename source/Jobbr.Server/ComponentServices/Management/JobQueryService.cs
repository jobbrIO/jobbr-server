using System;
using System.Collections.Generic;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class JobQueryService : IQueryService
    {
        private readonly IJobStorageProvider jobStorageProvider;
        private readonly IMapper mapper;

        public JobQueryService(IJobStorageProvider jobStorageProvider, IMapper mapper)
        {
            this.jobStorageProvider = jobStorageProvider;
            this.mapper = mapper;
        }

        public List<Job> GetAllJobs()
        {
            var jobs = this.jobStorageProvider.GetJobs();

            return this.mapper.Map<List<Job>>(jobs);
        }

        public Job GetJobById(long id)
        {
            throw new NotImplementedException();
        }

        public Job GetJobByUniqueName(string uniqueName)
        {
            throw new NotImplementedException();
        }

        public List<IJobTrigger> GetActiveTriggers()
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRunById(long id)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRuns()
        {
            throw new NotImplementedException();
        }

        public IJobTrigger GetTriggerById(long triggerId)
        {
            throw new NotImplementedException();
        }

        public List<IJobTrigger> GetTriggersByJobId(long jobId)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByUserOrderByIdDesc(long userId)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByUserNameOrderOrderByIdDesc(string userName)
        {
            throw new NotImplementedException();
        }
    }
}