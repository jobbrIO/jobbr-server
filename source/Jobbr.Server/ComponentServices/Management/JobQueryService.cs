using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;

namespace Jobbr.Server.ComponentModel.Services
{
    internal class JobQueryService : IQueryService
    {
        public List<Job> GetAllJobs()
        {
            return new List<Job>();
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