using System;
using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;

namespace Jobbr.Server.ComponentModel.Services
{
    internal class JobManagementServiceImplementation : IJobManagementService
    {
        public List<Job> GetAllJobs()
        {
            return new List<Job>();
        }

        public Job GetJobById(long id)
        {
            throw new NotImplementedException();
        }

        public Job AddJob(Job job)
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

        public long AddTrigger(RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public bool DisableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public bool EnableTrigger(long triggerId)
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

        public List<JobArtefact> GetArtefactForJob(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public Stream GetArtefactAsStream(JobRun jobRun, string filename)
        {
            throw new NotImplementedException();
        }

        public void UpdateTriggerDefinition(long triggerId, string definition)
        {
            throw new NotImplementedException();
        }

        public void UpdatetriggerStartTime(long triggerId, DateTime startDateTimeUtc)
        {
            throw new NotImplementedException();
        }
    }
}