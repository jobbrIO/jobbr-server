using System;
using System.Threading;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

using NCrontab;

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
                this.CreateSchedule(trigger);
            }
        }

        private void CreateSchedule(JobTriggerBase trigger)
        {
            var job = this.jobService.GetJob(trigger.JobId);
            DateTime? calculatedNextRun = null;

            // Get the next occurence from database
            var nextScheduledJobRun = this.jobService.GetNextJobRunByTriggerId(trigger.Id);

            // Calculate the next occurance for the trigger
            calculatedNextRun = this.GetNextTriggerDateTime(trigger as dynamic);

            if (calculatedNextRun != null)
            {
                if (nextScheduledJobRun == null)
                {
                    this.jobService.CreateJobRun(job, trigger, calculatedNextRun.Value);
                }
                else
                {
                    // Is this value in sync with the schedule table?
                    if (nextScheduledJobRun.PlannedStartDateTimeUtc == calculatedNextRun)
                    {
                        // Ok, all in sync --> Nothing to do
                    }
                    else
                    {
                        if (nextScheduledJobRun.PlannedStartDateTimeUtc.AddSeconds(this.configuration.AllowChangesBeforeStartInSec) < calculatedNextRun)
                        {
                            // TODO: Change the trigger
                        }
                        else
                        {
                            // TODO: Its too late --> Log
                        }
                    }
                }
            }
            else
            {
                // TODO: What happens now? --> Log that no valid future execution data was found
            }
        }

        private DateTime? GetNextTriggerDateTime(ScheduledTrigger scheduledTrigger)
        {
            // TODO: Implement
            return null;
        }

        private DateTime? GetNextTriggerDateTime(InstantTrigger instantTrigger)
        {
            var baseDateTimeUtc = instantTrigger.CreateDateTimeUtc;

            var startDate = baseDateTimeUtc.AddMinutes(instantTrigger.DelayedMinutes);

            this.jobService.DisableTrigger(instantTrigger.Id, false);

            return startDate;
        }

        private DateTime? GetNextTriggerDateTime(RecurringTrigger recurringTrigger)
        {
            if (recurringTrigger != null)
            {
                DateTime lastTime;

                // Calculate the next occurance
                if (recurringTrigger.StartDateTimeUtc.HasValue && recurringTrigger.StartDateTimeUtc.Value > DateTime.UtcNow)
                {
                    lastTime = recurringTrigger.StartDateTimeUtc.Value;
                }
                else
                {
                    lastTime = DateTime.UtcNow;
                }

                var schedule = CrontabSchedule.Parse(recurringTrigger.Definition);
                return schedule.GetNextOccurrence(lastTime);
            }

            return null;
        }

        private void JobServiceOnTriggerUpdate(object sender, JobTriggerEventArgs args)
        {
            Console.WriteLine("Got an update for the job " + args.Trigger.Id);

            if (args.Trigger.IsActive)
            {
                this.CreateSchedule(args.Trigger);
            }
            else
            {
                this.RemoveSchedule(args.Trigger);
            }
        }

        private void RemoveSchedule(JobTriggerBase trigger)
        {
            throw new NotImplementedException();
        }
    }
}