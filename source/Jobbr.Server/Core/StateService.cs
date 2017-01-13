using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The job repository.
    /// </summary>
    public class StateService : IStateService
    {
        private static readonly ILog Logger = LogProvider.For<StateService>();

        private readonly string jobRunnerProcessName;

        private readonly JobbrRepository jobbrRepository;

        public StateService(IJobbrConfiguration configuration, JobbrRepository jobbrRepository)
        {
            this.jobbrRepository = jobbrRepository;

            var exeName = new FileInfo(configuration.JobRunnerExeResolver.Invoke()).Name;
            this.jobRunnerProcessName = exeName.Substring(0, exeName.Length - ".exe".Length);

            StateService.Logger.Log(LogLevel.Debug, () => "New instance of a stateService has been created.");
        }

        /// <summary>
        /// The trigger updated.
        /// </summary>
        public event EventHandler<JobTriggerEventArgs> TriggerUpdate;

        /// <summary>
        /// The job run modification.
        /// </summary>
        public event EventHandler<JobRunModificationEventArgs> JobRunModification;

        public void UpdateJobRunState(JobRun jobRun, JobRunState state)
        {
            jobRun.State = state;
            this.jobbrRepository.Update(jobRun);

            StateService.Logger.InfoFormat("[{0}] The JobRun with id: {1} has switched to the '{2}'-State", jobRun.UniqueId, jobRun.Id, state);

            OnJobRunModification(new JobRunModificationEventArgs {JobRun = jobRun});
        }

        public void UpdateJobRunDirectories(JobRun jobRun, string workDir, string tempDir)
        {
            jobRun.WorkingDir = workDir;
            jobRun.TempDir = tempDir;

            this.jobbrRepository.Update(jobRun);
        }

        public void SetPidForJobRun(JobRun jobRun, int id)
        {
            this.jobbrRepository.SetPidForJobRun(jobRun, id);
        }

        public void SetJobRunStartTime(JobRun jobRun, DateTime startDateTimeUtc)
        {
            jobRun.ActualStartDateTimeUtc = startDateTimeUtc;
            this.jobbrRepository.Update(jobRun);

            OnJobRunModification(new JobRunModificationEventArgs {JobRun = jobRun});
        }

        public void SetJobRunEndTime(JobRun jobRun, DateTime endDateTimeUtc)
        {
            var fromDb = this.jobbrRepository.GetJobRunById(jobRun.Id);

            fromDb.ActualEndDateTimeUtc = endDateTimeUtc;
            this.jobbrRepository.Update(jobRun);
        }

        public void UpdateJobRunProgress(long jobRunId, double percent)
        {
            this.jobbrRepository.UpdateJobRunProgress(jobRunId, percent);
        }

        public void UpdatePlannedStartDate(long jobRunId, DateTime plannedStartDateTimeUtc)
        {
            var fromDb = this.jobbrRepository.GetJobRunById(jobRunId);

            var jobFromDb = this.jobbrRepository.GetJob(fromDb.JobId);
                
            this.jobbrRepository.UpdatePlannedStartDateTimeUtc(fromDb.Id, plannedStartDateTimeUtc);

            this.OnJobRunModification(new JobRunModificationEventArgs {Job = jobFromDb, JobRun = fromDb});
        }

        public long CreateJobRun(Job job, JobTriggerBase trigger, DateTime startDateTimeUtc)
        {
            var jobRun = this.jobbrRepository.SaveNewJobRun(job, trigger, startDateTimeUtc);

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

            return this.jobbrRepository.CheckParallelExecution(triggerId);
        }

        private void FailJobRunIfProcessDoesNotExistAnymore(long triggerId)
        {
            var lastJobRun = this.jobbrRepository.GetLastJobRunByTriggerId(triggerId);

            if (lastJobRun == null)
            {
                return;
            }

            if (lastJobRun.IsFinished == false && lastJobRun.Pid > 0 && Process.GetProcessesByName(jobRunnerProcessName).All(p => p.Id != lastJobRun.Pid))
            {
                StateService.Logger.Warn(string.Format("Setting JobRun (Id: {0}) to failed since Pid {1} could not be found. Old State of JobRun: {2}", lastJobRun.Id, lastJobRun.Pid, lastJobRun.State));
                lastJobRun.State = JobRunState.Failed;
                this.jobbrRepository.Update(lastJobRun);
            }
        }


        public void DipathOnTriggerUpdate(JobTriggerBase triggerFromDb)
        {
            this.DispatchTriggerUpdate(new JobTriggerEventArgs { Trigger = triggerFromDb });
        }

        protected virtual void DispatchTriggerUpdate(JobTriggerEventArgs e)
        {
            this.TriggerUpdate?.Invoke(this, e);
        }

        protected virtual void OnJobRunModification(JobRunModificationEventArgs e)
        {
            this.JobRunModification?.Invoke(this, e);
        }
    }
}