using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Tests.Infrastructure.StorageProvider
{
    public class NotImplementedJobStorageProvider : IJobStorageProvider
    {
        public static NotImplementedJobStorageProvider Instance { get; private set; }

        public NotImplementedJobStorageProvider()
        {
            Instance = this;
        }

        public virtual List<Job> GetJobs()
        {
            throw new NotImplementedException();
        }

        public virtual long AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            throw new NotImplementedException();
        }

        public virtual long AddTrigger(RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public virtual long AddTrigger(InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public virtual long AddTrigger(ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public virtual bool DisableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public virtual bool EnableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobTriggerBase> GetActiveTriggers()
        {
            throw new NotImplementedException();
        }

        public virtual JobTriggerBase GetTriggerById(long triggerId)
        {
            throw new NotImplementedException();
        }

        public virtual JobRun GetLastJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public virtual JobRun GetNextJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public virtual int AddJobRun(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobRun> GetJobRuns()
        {
            throw new NotImplementedException();
        }

        public bool UpdateProgress(long jobRunId, double? progress)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public virtual Job GetJobById(long id)
        {
            throw new NotImplementedException();
        }

        public virtual Job GetJobByUniqueName(string identifier)
        {
            throw new NotImplementedException();
        }

        public virtual JobRun GetJobRunById(long id)
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRunById(Guid uniqueId)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsByUserId(long userId)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public virtual bool Update(Job job)
        {
            throw new NotImplementedException();
        }

        public bool Update(InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public bool Update(ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public bool Update(RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public virtual List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            throw new NotImplementedException();
        }

        public bool CheckParallelExecution(long triggerId)
        {
            throw new NotImplementedException();
        }
    }
}