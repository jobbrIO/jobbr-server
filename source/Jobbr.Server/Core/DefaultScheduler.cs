using System;
using System.Threading;

using Jobbr.Server.Common;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The Scheduler creates new scheduled Jobs in the JobRun Table based on the triggers
    /// </summary>
    public class DefaultScheduler : IDisposable
    {
        private readonly IJobService jobService;

        private readonly IJobbrConfiguration configuration;

        private Timer timer;

        public DefaultScheduler(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;

            this.timer = new Timer(this.ScheduleJobRuns, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            this.jobService.TriggerUpdate += this.JobServiceOnTriggerUpdate;

            this.timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));
        }

        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);
            
            this.jobService.TriggerUpdate -= this.JobServiceOnTriggerUpdate;
        }

        public void Dispose()
        {

            this.Stop();
        }

        private void ScheduleJobRuns(object state)
        {
            var alltriggers = this.jobService.GetActiveTriggers();

            foreach (var trigger in alltriggers)
            {
                var job = this.jobService.GetJob(trigger.JobId);

                // Calculate the next occurance for the trigger


                //// Validate if there is already a corresponding JobRun in the future
                /// Case a) Yes and the StartDate is equal to the current calculation -> Nothing Todo
                /// Case b) Yes but the StartDate is not equal and job has not been started yet 
                ///             - and will not start in 10s -> Remove
                ///             - will start or has already started -> ignore
                /// Case c) No -> Create

            }
        }

        private void JobServiceOnTriggerUpdate(object sender, JobTriggerEventArgs jobTriggerEventArgs)
        {
            Console.WriteLine("Got an update for the job " + jobTriggerEventArgs.Trigger.Id);
        }
    }
}