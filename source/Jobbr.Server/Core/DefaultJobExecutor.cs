using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Responsible for creating and starting a "virtual" context where the running job is managed within
    /// </summary>
    public class DefaultJobExecutor : IJobExecutor
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogProvider.For<DefaultJobExecutor>();

        private IStateService stateService;

        private IJobbrConfiguration configuration;

        private readonly IJobStorageProvider jobStorageProvider;

        private readonly IJobbrRepository jobbrRepository;

        private Timer timer;

        private List<JobRun> queue = new List<JobRun>();

        private List<JobRunContext> activeContexts = new List<JobRunContext>();

        private object syncRoot = new object();

        private int StartNewJobsEverySeconds = 1;

        public DefaultJobExecutor(IStateService stateService, IJobbrConfiguration configuration, IJobStorageProvider jobStorageProvider, IJobbrRepository jobbrRepository)
        {
            this.stateService = stateService;
            this.configuration = configuration;
            this.jobStorageProvider = jobStorageProvider;
            this.jobbrRepository = jobbrRepository;

            this.timer = new Timer(this.StartReadyJobsFromQueue, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            var dateTime = DateTime.UtcNow;

            var scheduledRuns = this.jobbrRepository.GetJobRunsByState(JobRunState.Scheduled);
            var processingRuns = this.jobbrRepository.GetJobRunsByState(JobRunState.Processing);

            var pastScheduledRuns = new List<JobRun>(scheduledRuns.Where(jr => jr.PlannedStartDateTimeUtc < dateTime).OrderBy(jr => jr.PlannedStartDateTimeUtc));
            var futureScheduledRuns = new List<JobRun>(scheduledRuns.Where(jr => jr.PlannedStartDateTimeUtc >= dateTime).OrderBy(jr => jr.PlannedStartDateTimeUtc));

            // Handle current activeContexts Jobs
            if (processingRuns.Any())
            {
                Logger.WarnFormat("There are {0} JobRuns which in Processing-State. Trying to reestablish a JobRunContext and capturing their process.", processingRuns.Count);
                
                // TODO: Recover still activeContexts jobs
                Logger.Error("Recovering of running jobs after server restart is not implemented!");
            }
            
            // Expire Scheduled Runs from the past
            if (pastScheduledRuns.Any())
            {
                Logger.WarnFormat("There are {0} scheduled JobRuns with a startdate in the past. They are all expired, because it's unlikely that JobRuns get relevant in the near future.", pastScheduledRuns.Count);
                
                // TODO: Trigger past scheduled jobs with no runs?
            }
            
            // Load all existing Schedules into the local memory
            if (futureScheduledRuns.Any())
            {
                Logger.InfoFormat("Adding {0} scheduled JobRuns with an upcoming startdate", futureScheduledRuns.Count);
                this.queue.AddRange(futureScheduledRuns);
            }
            
            // Wire Events
            this.stateService.JobRunModification += this.StateServiceOnStateRunModification;
            this.stateService.TriggerUpdate += this.StateServiceOnTriggerUpdate;

            Logger.InfoFormat("Enabling periodic check for JobRuns to start every {0}s", this.StartNewJobsEverySeconds);
            this.timer.Change(TimeSpan.FromSeconds(this.StartNewJobsEverySeconds), TimeSpan.FromSeconds(this.StartNewJobsEverySeconds));

            Logger.InfoFormat("The DefaultJobExecutor has been started at {0} with a queue size of '{1}'", dateTime, this.queue.Count);
        }

        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            this.stateService.JobRunModification -= this.StateServiceOnStateRunModification;
            this.stateService.TriggerUpdate -= this.StateServiceOnTriggerUpdate;
        }

        private void StateServiceOnStateRunModification(object sender, JobRunModificationEventArgs args)
        {
            lock (this.syncRoot)
            {
                var changedJobRun = args.JobRun;

                // a) Not yet in Queue
                if (this.queue.All(jr => jr.Id != changedJobRun.Id))
                {
                    // Only add scheduled jobruns
                    if (changedJobRun.State == JobRunState.Scheduled)
                    {
                        var job = args.Job;

                        Logger.InfoFormat("Adding JobRun for Job '{0}' (Id: {1}, Type: '{2}') with JobRunId {3} to the queue", job.UniqueName, job.Id, job.Type, changedJobRun.Id);
                        this.queue.Add(changedJobRun);
                    }
                }
                // b) Change information
                else
                {
                    var queuedJobRun = this.queue.FirstOrDefault(x => x.Id == changedJobRun.Id);

                    if (queuedJobRun.State == JobRunState.Scheduled)
                    {
                        if (queuedJobRun.PlannedStartDateTimeUtc != changedJobRun.PlannedStartDateTimeUtc)
                        {
                            Logger.InfoFormat("The planned startdate for a prepared jobRun (uniqueId: {0}) has changed from {1} to {2}", queuedJobRun.UniqueId, queuedJobRun.PlannedStartDateTimeUtc, changedJobRun.PlannedStartDateTimeUtc);
                            queuedJobRun.PlannedStartDateTimeUtc = changedJobRun.PlannedStartDateTimeUtc;
                        }
                    }
                    else
                    {
                        Logger.WarnFormat("There was a change to the already started JobRun (uniqueId: {0}) wich cannot be handled.", queuedJobRun.UniqueId);
                    }
                }
            }
        }

        private void StateServiceOnTriggerUpdate(object sender, JobTriggerEventArgs jobTriggerEventArgs)
        {
            var trigger = jobTriggerEventArgs.Trigger;

            if (!trigger.IsActive && !(trigger is InstantTrigger))
            {
                lock (this.syncRoot)
                {
                    var jobRunsFromQueueByThisTrigger = this.queue.Where(r => r.TriggerId == trigger.Id).ToList();

                    if (jobRunsFromQueueByThisTrigger.Count > 0)
                    {
                        Logger.InfoFormat("Trying to remove {0} ready JobRuns from Queue because related trigger (id: {1}) was deactivated!", jobRunsFromQueueByThisTrigger.Count, trigger.Id);

                        foreach (var jobRun in jobRunsFromQueueByThisTrigger)
                        {
                            if (jobRun.State == JobRunState.Scheduled)
                            {
                                Logger.WarnFormat("Removed prepared JobRun with id {0} because trigger (id: {1}) was deactivated", jobRun.UniqueId, trigger.Id);
                                this.queue.Remove(jobRun);
                            }
                            else
                            {
                                Logger.ErrorFormat("Cannot JobRun with id {0} it has already been started", jobRun.UniqueId);
                            }
                        }
                    }
                }
            }
        }

        private void StartReadyJobsFromQueue(object state)
        {
            lock (this.syncRoot)
            {
                var possibleJobsToStart = this.configuration.MaxConcurrentJobs - this.activeContexts.Count;
                var readyJobs = this.queue.Where(jr => jr.PlannedStartDateTimeUtc <= DateTime.UtcNow).OrderBy(jr => jr.PlannedStartDateTimeUtc).ToList();

                var jobsToStart = readyJobs.Take(possibleJobsToStart).ToList();

                var queueCannotStartAll = readyJobs.Count > possibleJobsToStart;
                var showStatusInformationNow = (DateTime.Now.Second % 5) == 0;
                var canStartAllReadyJobs = jobsToStart.Count > 0 && jobsToStart.Count <= possibleJobsToStart;

                if ((queueCannotStartAll && showStatusInformationNow) || canStartAllReadyJobs)
                {
                    Logger.InfoFormat("There are {0} ready jobs in the queue and currently {1} running jobs. Number of possible jobs to start: {2}", readyJobs.Count, this.activeContexts.Count, possibleJobsToStart);
                }

                foreach (var jobRun in jobsToStart)
                {
                    try
                    {
                        this.StartNewJob(jobRun);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(string.Format("Exception was thrown while starting a new JobRun with Id: {0}. Retry to start next time. (TriggerId: {1}, JobId: {2})", jobRun.Id, jobRun.TriggerId, jobRun.JobId), e);
                    }
                }
            }
        }

        private void StartNewJob(JobRun jobRun)
        {
            var run = jobRun;

            Logger.InfoFormat("Creating new context for JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);

            this.stateService.UpdateJobRunState(jobRun, JobRunState.Preparing);
            this.queue.Remove(jobRun);

            var context = new JobRunContext(this.stateService, this.configuration, this.jobStorageProvider);
            context.Ended += this.ContextOnEnded;

            this.activeContexts.Add(context);

            Logger.InfoFormat("Starting JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);
            new TaskFactory().StartNew(() => context.Start(run));
        }

        private void ContextOnEnded(object sender, JobRunEndedEventArgs args)
        {
            lock (this.syncRoot)
            {
                var jobRunContext = sender as JobRunContext;
                var run = args.JobRun;

                jobRunContext.Ended -= this.ContextOnEnded;

                try
                {
                    this.stateService.SetJobRunEndTime(args.JobRun, DateTime.UtcNow);
                }
                catch (Exception e)
                {
                    Logger.ErrorException(string.Format("Exception while setting the end-time of the jobRun with id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId), e);
                }

                if (args.ExitCode != 0)
                {
                    Logger.WarnFormat("The process within the context JobRun has exited with a non-zero exit code. JobRunId: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);

                    try
                    {
                        this.stateService.UpdateJobRunState(args.JobRun, JobRunState.Failed);
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(string.Format("Exception while setting the 'Failed'-State to the jobRun with id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId), e);
                    }
                }
                else
                {
                    if (args.JobRun.Progress > 0)
                    {
                        try
                        {
                            this.stateService.UpdateJobRunProgress(args.JobRun.Id, 100);
                        }
                        catch (Exception e)
                        {
                            Logger.ErrorException(string.Format("Exception while setting progress to 100% after completion of the jobRun with id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId), e);
                        }
                    }
                }

                Logger.InfoFormat("Removing context for JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);

                this.activeContexts.Remove(jobRunContext);
            }
        }
    }
}
