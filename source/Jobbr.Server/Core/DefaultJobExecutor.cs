using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Responsible for Starting extenal processes with the job inside
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

        private IList<JobRun> queue;

        private IList<JobRunContext> running = new List<JobRunContext>();

        private object syncRoot = new object();

        public DefaultJobExecutor(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;

            this.timer = new Timer(this.Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Callback(object state)
        {
            var jobsToStart = new List<JobRun>();

            lock (this.syncRoot)
            {
                jobsToStart = this.queue.Where(jr => jr.PlannedStartDateTimeUtc <= DateTime.UtcNow).OrderBy(jr => jr.PlannedStartDateTimeUtc).Take(this.configuration.MaxConcurrentJobs - this.running.Count).ToList();

                foreach (var jobRun in jobsToStart)
                {
                    this.jobService.UpdateJobRunState(jobRun, JobRunState.Preparing);
                    this.queue.Remove(jobRun);
                    
                    var context = new JobRunContext(this.jobService, this.configuration);
                    context.Ended += this.ContextOnEnded;

                    this.running.Add(context);

                    var run = jobRun;
                    new TaskFactory().StartNew(() => context.Start(run));
                }
            }
        }

        private void ContextOnEnded(object sender, JobRunEndedEventArgs args)
        {
            lock (this.syncRoot)
            {
                var jobRunContext = sender as JobRunContext;
                jobRunContext.Ended -= this.ContextOnEnded;

                this.jobService.SetJobRunEndTime(args.JobRun, DateTime.UtcNow);

                if (args.ExitCode != 0)
                {
                    this.jobService.UpdateJobRunState(args.JobRun, JobRunState.Failed);
                }
                else
                {
                    if (args.JobRun.Progress > 0)
                    {
                        this.jobService.UpdateJobRunProgress(args.JobRun, 100);
                    }
                }

                this.running.Remove(jobRunContext);
            }
        }

        public void Start()
        {
            var dateTime = DateTime.UtcNow;

            var scheduledRuns = this.jobService.GetJobRuns(JobRunState.Scheduled);
            var processingRuns = this.jobService.GetJobRuns(JobRunState.Processing);

            var pastScheduledRuns = new List<JobRun>(scheduledRuns.Where(jr => jr.PlannedStartDateTimeUtc > dateTime).OrderBy(jr => jr.PlannedStartDateTimeUtc));
            var futureScheduledRuns = new List<JobRun>(scheduledRuns.Where(jr => jr.PlannedStartDateTimeUtc >= dateTime).OrderBy(jr => jr.PlannedStartDateTimeUtc));

            // Handle current running Jobs
            if (processingRuns.Any())
            {
                // TODO: Recover still running jobs
            }
            
            // Expire Scheduled Runs from the past
            if (pastScheduledRuns.Any())
            {
                // TODO: Trigger past scheduled jobs with no runs?
                
            }
            
            // Load all existing Schedules into the local memory
            if (futureScheduledRuns.Any())
            {
                this.queue = new List<JobRun>(futureScheduledRuns);
            }

            // Wire Events
            this.jobService.JobRunModification += this.JobServiceOnJobRunModification;

            this.timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void JobServiceOnJobRunModification(object sender, JobRunModificationEventArgs args)
        {
            lock (this.syncRoot)
            {
                // a) TODO: Remove from queue if trigger was removed

                // b) Add to queue
                if (this.queue.All(jr => jr.Id != args.JobRun.Id))
                {
                    // Only add scheduled jobruns
                    if (args.JobRun.State == JobRunState.Scheduled)
                    {
                        this.queue.Add(args.JobRun);
                    }
                }
                else
                {

                    // c) TODO: Change information
                }
            }
        }

        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            this.jobService.JobRunModification += JobServiceOnJobRunModification;
        }
    }
}
