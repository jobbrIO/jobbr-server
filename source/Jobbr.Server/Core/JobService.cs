using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The job repository.
    /// </summary>
    public class JobService : IJobService
    {
        private static readonly ILog Logger = LogProvider.For<JobService>();

        private readonly IJobStorageProvider storageProvider;

        private readonly string jobRunnerProcessName;

        public JobService(IJobStorageProvider storageProvider, IJobbrConfiguration configuration)
        {
            this.storageProvider = storageProvider;

            var exeName = new FileInfo(configuration.JobRunnerExeResolver.Invoke()).Name;
            this.jobRunnerProcessName = exeName.Substring(0, exeName.Length - ".exe".Length);

            Logger.Log(LogLevel.Debug, () => "New instance of a JobService has been created.");
        }

        /// <summary>
        /// The trigger updated.
        /// </summary>
        public event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        /// <summary>
        /// The job run modification.
        /// </summary>
        public event EventHandler<JobRunModificationEventArgs> JobRunModification;

        public List<Job> GetAllJobs()
        {
            return storageProvider.GetJobs();
        }

        public Job GetJob(long id)
        {
            return storageProvider.GetJobById(id);
        }

        public Job AddJob(Job job)
        {
            var id = storageProvider.AddJob(job);

            job.Id = id;

            return job;
        }

        public List<JobRun> GetJobRuns(JobRunState state)
        {
            return storageProvider.GetJobRunsByState(state);
        }

        public void UpdateJobRunState(JobRun jobRun, JobRunState state)
        {
            jobRun.State = state;
            storageProvider.Update(jobRun);

            Logger.InfoFormat("[{0}] The JobRun with id: {1} has switched to the '{2}'-State", jobRun.UniqueId, jobRun.Id, state);

            OnJobRunModification(new JobRunModificationEventArgs {JobRun = jobRun});
        }

        public void UpdateJobRunDirectories(JobRun jobRun, string workDir, string tempDir)
        {
            jobRun.WorkingDir = workDir;
            jobRun.TempDir = tempDir;

            storageProvider.Update(jobRun);
        }

        public void SetJobRunStartTime(JobRun jobRun, DateTime startDateTimeUtc)
        {
            jobRun.ActualStartDateTimeUtc = startDateTimeUtc;
            storageProvider.Update(jobRun);

            OnJobRunModification(new JobRunModificationEventArgs {JobRun = jobRun});
        }

        public void SetJobRunEndTime(JobRun jobRun, DateTime endDateTimeUtc)
        {
            var fromDb = storageProvider.GetJobRunById(jobRun.Id);

            fromDb.ActualEndDateTimeUtc = endDateTimeUtc;
            storageProvider.Update(fromDb);
        }

        public void UpdateJobRunProgress(long jobRunId, double progress)
        {
            storageProvider.UpdateProgress(jobRunId, progress);
        }

        public void UpdatePlannedStartDate(JobRun jobRun)
        {
            var fromDb = storageProvider.GetJobRunById(jobRun.Id);
            fromDb.PlannedStartDateTimeUtc = jobRun.PlannedStartDateTimeUtc;

            var jobFromDb = storageProvider.GetJobById(jobRun.JobId);

            OnJobRunModification(new JobRunModificationEventArgs {Job = jobFromDb, JobRun = jobRun});
            storageProvider.Update(fromDb);
        }

        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            jobRun.Pid = id;

            storageProvider.Update(jobRun);
        }

        public JobRun GetJobRun(long id)
        {
            return storageProvider.GetJobRunById(id);
        }

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            return storageProvider.GetTriggersByJobId(jobId);
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = storageProvider.AddTrigger(trigger);
            OnTriggerUpdate(new JobTriggerEventArgs {Trigger = trigger});

            return trigger.Id;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = storageProvider.AddTrigger(trigger);
            OnTriggerUpdate(new JobTriggerEventArgs {Trigger = trigger});

            return trigger.Id;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            if (trigger.JobId == 0)
            {
                throw new ArgumentException("JobId is required", "trigger.JobId");
            }

            trigger.Id = storageProvider.AddTrigger(trigger);
            OnTriggerUpdate(new JobTriggerEventArgs {Trigger = trigger});

            return trigger.Id;
        }

        public bool DisableTrigger(long triggerId, bool enableNotification = true)
        {
            var trigger = storageProvider.GetTriggerById(triggerId);

            if (!trigger.IsActive)
            {
                return false;
            }

            trigger.IsActive = false;
            storageProvider.DisableTrigger(triggerId);

            if (enableNotification)
            {
                OnTriggerUpdate(new JobTriggerEventArgs {Trigger = trigger});
            }

            return true;
        }

        public bool EnableTrigger(long triggerId)
        {
            var trigger = storageProvider.GetTriggerById(triggerId);

            if (trigger.IsActive)
            {
                return false;
            }

            trigger.IsActive = true;
            storageProvider.EnableTrigger(triggerId);

            OnTriggerUpdate(new JobTriggerEventArgs {Trigger = trigger});

            return true;
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            try
            {
                return storageProvider.GetActiveTriggers();
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot read active triggers from storage provider due to an exception. Returning empty list.", e);

                return new List<JobTriggerBase>();
            }
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            return storageProvider.GetLastJobRunByTriggerId(triggerId);
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId)
        {
            return storageProvider.GetFutureJobRunsByTriggerId(triggerId);
        }

        public long CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc)
        {
            var jobRun = new JobRun
            {
                JobId = job.Id,
                TriggerId = trigger.Id,
                JobParameters = job.Parameters,
                InstanceParameters = trigger.Parameters,
                UniqueId = Guid.NewGuid().ToString(),
                State = JobRunState.Scheduled,
                PlannedStartDateTimeUtc = startDateTimeUtc
            };

            jobRun.Id = storageProvider.AddJobRun(jobRun);

            OnJobRunModification(new JobRunModificationEventArgs
            {
                Job = job,
                Trigger = trigger,
                JobRun = jobRun
            });

            return jobRun.Id;
        }

        public bool CheckParallelExecution(long triggerId)
        {
            FailJobRunIfProcessDoesNotExistAnymore(triggerId);

            return storageProvider.CheckParallelExecution(triggerId);
        }

        private void FailJobRunIfProcessDoesNotExistAnymore(long triggerId)
        {
            var lastJobRun = storageProvider.GetLastJobRunByTriggerId(triggerId);

            if (lastJobRun == null)
            {
                return;
            }

            if (lastJobRun.IsFinished == false && lastJobRun.Pid > 0 && Process.GetProcessesByName(jobRunnerProcessName).All(p => p.Id != lastJobRun.Pid))
            {
                Logger.Warn(string.Format("Setting JobRun (Id: {0}) to failed since Pid {1} could not be found. Old State of JobRun: {2}", lastJobRun.Id, lastJobRun.Pid, lastJobRun.State));
                lastJobRun.State = JobRunState.Failed;
                storageProvider.Update(lastJobRun);
            }
        }

        public void UpdateTrigger(long id, JobTriggerBase trigger)
        {
            if (id == 0)
            {
                throw new ArgumentException("JobId is required", "id");
            }

            var triggerFromDb = storageProvider.GetTriggerById(id);

            var hadChanges = false;

            if (trigger.IsActive != triggerFromDb.IsActive)
            {
                // Activated or deactivated
                triggerFromDb.IsActive = trigger.IsActive;
                hadChanges = true;
            }

            hadChanges = hadChanges || this.ApplyOtherChanges(triggerFromDb as dynamic, trigger as dynamic);

            if (hadChanges)
            {
                if (trigger is InstantTrigger)
                {
                    storageProvider.Update(trigger as InstantTrigger);
                }

                if (trigger is ScheduledTrigger)
                {
                    storageProvider.Update(trigger as ScheduledTrigger);
                }

                if (trigger is RecurringTrigger)
                {
                    storageProvider.Update(trigger as RecurringTrigger);
                }

                OnTriggerUpdate(new JobTriggerEventArgs {Trigger = triggerFromDb});
            }
        }

        protected virtual void OnTriggerUpdate(JobTriggerEventArgs e)
        {
            var handler = TriggerUpdate;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        protected virtual void OnJobRunModification(JobRunModificationEventArgs e)
        {
            var handler = JobRunModification;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private bool ApplyOtherChanges(RecurringTrigger fromDb, RecurringTrigger updatedOne)
        {
            if (fromDb.Definition != updatedOne.Definition)
            {
                fromDb.Definition = updatedOne.Definition;
                return true;
            }

            return false;
        }

        private bool ApplyOtherChanges(ScheduledTrigger fromDb, ScheduledTrigger updatedOne)
        {
            if (fromDb.StartDateTimeUtc != updatedOne.StartDateTimeUtc)
            {
                fromDb.StartDateTimeUtc = updatedOne.StartDateTimeUtc;

                return true;
            }

            return false;
        }

        private bool ApplyOtherChanges(InstantTrigger fromDb, InstantTrigger updatedOne)
        {
            Logger.WarnFormat("Cannot change an instant trigger!");

            return false;
        }

        private bool ApplyOtherChanges(object fromDb, object updatedOne)
        {
            Logger.WarnFormat("Unknown trigger types: From: {1}, To: {2}!", fromDb.GetType(), updatedOne.GetType());

            return false;
        }
    }
}