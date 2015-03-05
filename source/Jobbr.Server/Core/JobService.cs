using System;
using System.Collections.Generic;
using System.Linq;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The job repository.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobbrStorageProvider storageProvider;

        /// <summary>
        /// The trigger updated.
        /// </summary>
        public event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        /// <summary>
        /// The job added.
        /// </summary>
        public event EventHandler<JobEventArgs> JobAdded;

        public JobService(IJobbrStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        public List<Job> GetAllJobs()
        {
            return this.storageProvider.GetJobs();
        }

        public Job GetJob(long id)
        {
            // TODO: Performance
            return this.storageProvider.GetJobs().FirstOrDefault(j => j.Id == id);
        }

        public long AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public List<Job> GetJobAllRuns()
        {
            throw new NotImplementedException();
        }

        public Job GetJobRun(long id)
        {
            throw new NotImplementedException();
        }

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            return this.storageProvider.GetTriggers(jobId);
        }

        public long AddTrigger(CronTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobIdis required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public long AddTrigger(StartDateTimeUtcTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobIdis required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobIdis required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public bool DisableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public bool EnableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return this.storageProvider.GetActiveTriggers();
        }

        protected virtual void OnTriggerUpdate(JobTriggerEventArgs e)
        {
            var handler = this.TriggerUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
