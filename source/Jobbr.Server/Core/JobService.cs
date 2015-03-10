using System;
using System.Collections.Generic;
using System.Linq;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The job repository.
    /// </summary>
    public class JobService : IJobService
    {
        private readonly IJobStorageProvider storageProvider;

        public JobService(IJobStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        /// <summary>
        /// The trigger updated.
        /// </summary>
        public event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        /// <summary>
        /// The job added.
        /// </summary>
        public event EventHandler<JobEventArgs> JobAdded;

        /// <summary>
        /// The job run modification.
        /// </summary>
        public event EventHandler<JobRunModificationEventArgs> JobRunModification;

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
            return this.storageProvider.AddJob(job);
        }

        public List<JobRun> GetJobRuns(JobRunState state)
        {
            // TODO: Performance
            return this.storageProvider.GetJobRuns().Where(jr => jr.State == state).ToList();
        }

        public void UpdateJobRunState(JobRun jobRun, JobRunState state)
        {
            jobRun.State = state;

            this.storageProvider.Update(jobRun);

            this.OnJobRunModification(new JobRunModificationEventArgs() { JobRun = jobRun });
        }

        public void UpdateJobRunDirectories(JobRun jobRun, string workDir, string tempDir)
        {
            jobRun.WorkingDir = workDir;
            jobRun.TempDir = tempDir;

            this.storageProvider.Update(jobRun);
        }

        public void SetJobRunStartTime(JobRun jobRun, DateTime startDateTimeUtc)
        {
            jobRun.ActualStartDateTimeUtc = startDateTimeUtc;
            this.storageProvider.Update(jobRun);
        }

        public void SetJobRunEndTime(JobRun jobRun, DateTime endDateTimeUtc)
        {
            var fromDb = this.storageProvider.GetJobRunById(jobRun.Id);

            fromDb.ActualEndDateTimeUtc = endDateTimeUtc;
            this.storageProvider.Update(fromDb);
        }

        public void UpdateJobRunProgress(JobRun jobRun, double percent)
        {
            var fromDb = this.storageProvider.GetJobRunById(jobRun.Id);

            fromDb.Progress = percent;
            this.storageProvider.Update(fromDb);
        }

        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            jobRun.Pid = id;

            this.storageProvider.Update(jobRun);
        }

        public List<JobRun> GetJobAllRuns()
        {
            // TODO: Performance
            return this.storageProvider.GetJobRuns();
        }

        public JobRun GetJobRun(long id)
        {
            // TODO: Performance
            return this.storageProvider.GetJobRuns().FirstOrDefault(jr => jr.Id == id);
        }

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            return this.storageProvider.GetTriggers(jobId);
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = this.storageProvider.AddTrigger(trigger);
            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return trigger.Id;
        }

        public bool DisableTrigger(long triggerId, bool enableNotification = true)
        {
            var trigger = this.storageProvider.GetTriggerById(triggerId);

            if (!trigger.IsActive)
            {
                return false;
            }

            this.storageProvider.DisableTrigger(triggerId);

            if (enableNotification)
            {
                this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });
            }

            return true;
        }

        public bool EnableTrigger(long triggerId)
        {
            var trigger = this.storageProvider.GetTriggerById(triggerId);

            if (trigger.IsActive)
            {
                return false;
            }

            this.storageProvider.EnableTrigger(triggerId);

            this.OnTriggerUpdate(new JobTriggerEventArgs { Trigger = trigger });

            return true;
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return this.storageProvider.GetActiveTriggers();
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            return this.storageProvider.GetLastJobRunByTriggerId(triggerId);
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId)
        {
            return this.storageProvider.GetFutureJobRunsByTriggerId(triggerId);
        }

        public long CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc)
        {
            var jobRun = new JobRun()
            {
                JobId = job.Id,
                TriggerId = trigger.Id,
                JobParameters = job.Parameters,
                InstanceParameters = trigger.Parameters,
                UniqueId = Guid.NewGuid().ToString(),
                State = JobRunState.Scheduled,
                PlannedStartDateTimeUtc = startDateTimeUtc
            };
            
            jobRun.Id = this.storageProvider.AddJobRun(jobRun);
            
            this.OnJobRunModification(new JobRunModificationEventArgs
                                          {
                                              Job = job,
                                              Trigger = trigger,
                                              JobRun = jobRun
                                          });
            return jobRun.Id;
        }

        protected virtual void OnTriggerUpdate(JobTriggerEventArgs e)
        {
            var handler = this.TriggerUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnJobRunModification(JobRunModificationEventArgs e)
        {
            var handler = this.JobRunModification;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
