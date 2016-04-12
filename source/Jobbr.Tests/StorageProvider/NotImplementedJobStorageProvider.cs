using System;
using System.Collections.Generic;

using Jobbr.Server.Common;
using Jobbr.Server.Model;
using Jobbr.Common.Model;

namespace Jobbr.Tests.StorageProvider
{
    public class NotImplementedJobStorageProvider : IJobStorageProvider
    {
        public virtual List<Job> GetJobs()
        {
            throw new System.NotImplementedException();
        }

        public virtual long AddJob(Job job)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            throw new System.NotImplementedException();
        }

        public virtual long AddTrigger(RecurringTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public virtual long AddTrigger(InstantTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public virtual long AddTrigger(ScheduledTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool DisableTrigger(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool EnableTrigger(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobTriggerBase> GetActiveTriggers()
        {
            throw new System.NotImplementedException();
        }

        public virtual JobTriggerBase GetTriggerById(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public virtual JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public virtual JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public virtual int AddJobRun(JobRun jobRun)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobRun> GetJobRuns()
        {
            throw new System.NotImplementedException();
        }

        public virtual bool Update(JobRun jobRun)
        {
            throw new System.NotImplementedException();
        }

        public virtual Job GetJobById(long id)
        {
            throw new System.NotImplementedException();
        }

        public virtual Job GetJobByUniqueName(string identifier)
        {
            throw new System.NotImplementedException();
        }

        public virtual JobRun GetJobRunById(long id)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsForUserId(long userId)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsForUserName(string userName)
        {
            throw new System.NotImplementedException();
        }

        public virtual bool Update(Job job)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(InstantTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(ScheduledTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public bool Update(RecurringTrigger trigger)
        {
            throw new System.NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            throw new System.NotImplementedException();
        }

        public List<JobRun> GetJobRunsByState(JobRunState state)
        {
            throw new NotImplementedException();
        }
    }
}