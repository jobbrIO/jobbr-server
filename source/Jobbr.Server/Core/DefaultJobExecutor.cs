using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Responsible for Starting extenal processes with the job inside
    /// </summary>
    public class DefaultJobExecutor : IJobExecutor
    {
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
                    this.running.Add(context);

                    var run = jobRun;
                    new TaskFactory().StartNew(() => context.Start(run));

                    context.Ended += this.ContextOnEnded;
                }
            }
        }

        private void ContextOnEnded(object sender, JobRunEndedEventArgs jobRunEndedEventArgs)
        {
            var jobRunContext = sender as JobRunContext;
            jobRunContext.Ended -= this.ContextOnEnded;

            if (jobRunEndedEventArgs.ExitCode != 0)
            {
                this.jobService.UpdateJobRunState(jobRunEndedEventArgs.JobRun, JobRunState.Failed);
            }

            this.running.Remove(jobRunContext);
        }

        public void Start()
        {
            // Load all existing Schedules into the local memory
            var scheduledRuns = this.jobService.GetJobRuns(JobRunState.Scheduled);

            var futureRuns  = new List<JobRun>(scheduledRuns.Where(jr => jr.PlannedStartDateTimeUtc >= DateTime.UtcNow).OrderBy(jr => jr.PlannedStartDateTimeUtc));

            // TODO: Recover still running jobs
            // TODO: Trigger past scheduled jobs with no runs?

            this.queue = new List<JobRun>(futureRuns);

            this.jobService.JobRunModification += this.JobServiceOnJobRunModification;

            this.timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void JobServiceOnJobRunModification(object sender, JobRunModificationEventArgs args)
        {
            lock (this.syncRoot)
            {
                // a) TODO: Remove from queue

                if (this.queue.All(jr => jr.Id != args.JobRun.Id))
                {
                    // Only add scheduled jobruns
                    if (args.JobRun.State == JobRunState.Scheduled)
                    {
                        // b) Add to queue
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
