using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;
using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Server.Model;

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

        private IJobService jobService;

        private IJobbrConfiguration configuration;

        private Timer timer;

        private List<JobRun> queue = new List<JobRun>();

        private List<JobRunContext> activeContexts = new List<JobRunContext>();

        private object syncRoot = new object();

        private int StartNewJobsEverySeconds = 1;

        public DefaultJobExecutor(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;

            this.timer = new Timer(this.StartReadyJobsFromQueue, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            var dateTime = DateTime.UtcNow;

            var scheduledRuns = this.jobService.GetJobRuns(JobRunState.Scheduled);
            var processingRuns = this.jobService.GetJobRuns(JobRunState.Processing);

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
                Logger.WarnFormat("There are {0} scheduled JobRuns with a startdate in the past. They all get expired, because Is unlikely that JobRuns get relevant in the near future.", pastScheduledRuns.Count);
                
                // TODO: Trigger past scheduled jobs with no runs?
            }
            
            // Load all existing Schedules into the local memory
            if (futureScheduledRuns.Any())
            {
                Logger.InfoFormat("Adding {0} scheduled JobRuns with an upcoming startdate", futureScheduledRuns.Count);
                this.queue.AddRange(futureScheduledRuns);
            }
            
            // Wire Events
            this.jobService.JobRunModification += this.JobServiceOnJobRunModification;
            this.jobService.TriggerUpdate += this.JobServiceOnTriggerUpdate;

            Logger.InfoFormat("Enabling periodic check for JobRuns to start every {0}s", this.StartNewJobsEverySeconds);
            this.timer.Change(TimeSpan.FromSeconds(this.StartNewJobsEverySeconds), TimeSpan.FromSeconds(this.StartNewJobsEverySeconds));

            Logger.InfoFormat("The DefaultJobExecutor has been started at {0} with a queue size of '{1}'", dateTime, this.queue.Count);
        }

        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            this.jobService.JobRunModification -= this.JobServiceOnJobRunModification;
            this.jobService.TriggerUpdate -= this.JobServiceOnTriggerUpdate;
        }

        private void JobServiceOnJobRunModification(object sender, JobRunModificationEventArgs args)
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
                // b) TODO: Change information
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

        private void JobServiceOnTriggerUpdate(object sender, JobTriggerEventArgs jobTriggerEventArgs)
        {
            var trigger = jobTriggerEventArgs.Trigger;

            if (!trigger.IsActive)
            {
                lock (this.syncRoot)
                {
                    var jobRunsFromQueueByThisTrigger = this.queue.Where(r => r.TriggerId == trigger.Id).ToList();

                    if (jobRunsFromQueueByThisTrigger.Count > 0)
                    {
                        Logger.InfoFormat("Removing {0} ready JobRuns from Queue because related trigger (id: {1}) was deactivated!", jobRunsFromQueueByThisTrigger.Count, trigger.Id);

                        foreach (var jobRun in jobRunsFromQueueByThisTrigger)
                        {
                            this.queue.Remove(jobRun);
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
                    var run = jobRun;

                    Logger.InfoFormat("Creating new context for JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);

                    this.jobService.UpdateJobRunState(jobRun, JobRunState.Preparing);
                    this.queue.Remove(jobRun);

                    var context = new JobRunContext(this.jobService, this.configuration);
                    context.Ended += this.ContextOnEnded;

                    this.activeContexts.Add(context);

                    Logger.InfoFormat("Starting JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);
                    new TaskFactory().StartNew(() => context.Start(run));
                }
            }
        }

        private void ContextOnEnded(object sender, JobRunEndedEventArgs args)
        {
            lock (this.syncRoot)
            {
                var jobRunContext = sender as JobRunContext;
                var run = args.JobRun;

                jobRunContext.Ended -= this.ContextOnEnded;

                this.jobService.SetJobRunEndTime(args.JobRun, DateTime.UtcNow);

                if (args.ExitCode != 0)
                {
                    Logger.WarnFormat("The process within the context JobRun has exited with a non-zero exit code. JobRunId: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);
                    this.jobService.UpdateJobRunState(args.JobRun, JobRunState.Failed);
                }
                else
                {
                    if (args.JobRun.Progress > 0)
                    {
                        this.jobService.UpdateJobRunProgress(args.JobRun, 100);
                    }
                }

                Logger.InfoFormat("Removing context for JobRun with Id: {0} (TriggerId: {1}, JobId: {2})", run.Id, run.TriggerId, run.JobId);

                this.activeContexts.Remove(jobRunContext);
            }
        }
    }
}
