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
            DateTime? nextTriggerDateTimeUtc = null;

            // Calculate the next occurance for the trigger
            nextTriggerDateTimeUtc = nextTriggerDateTimeUtc ?? this.GetNextTriggerDateTime(trigger as CronTrigger);
            nextTriggerDateTimeUtc = nextTriggerDateTimeUtc ?? this.GetNextTriggerDateTime(trigger as InstantTrigger);
            nextTriggerDateTimeUtc = nextTriggerDateTimeUtc ?? this.GetNextTriggerDateTime(trigger as StartDateTimeUtcTrigger);

            if (nextTriggerDateTimeUtc != null)
            {
                // Is this value in sync with the schedule table?
            }
            else
            {
                // TODO: What happens now?
            }

            //// Validate if there is already a corresponding JobRun in the future
            /// Case a) Yes and the StartDate is equal to the current calculation -> Nothing Todo
            /// Case b) Yes but the StartDate is not equal and job has not been started yet 
            ///             - and will not start in 10s -> Remove
            ///             - will start or has already started -> ignore
            /// Case c) No -> Create
        }

        private DateTime? GetNextTriggerDateTime(StartDateTimeUtcTrigger startDateTimeUtcTrigger)
        {
            return null;
        }

        private DateTime? GetNextTriggerDateTime(InstantTrigger instantTrigger)
        {
            return null;
        }

        private DateTime? GetNextTriggerDateTime(CronTrigger cronTrigger)
        {
            if (cronTrigger != null)
            {
                DateTime lastTime;

                // Calculate the next occurance
                var lastJobRun = this.jobService.GetLastJobRunByTriggerId(cronTrigger.Id);

                if (lastJobRun != null)
                {
                    lastTime = lastJobRun.PlannedStartDateTimeUtc;
                }
                else if (cronTrigger.DateTimeUtc.HasValue)
                {
                    lastTime = cronTrigger.DateTimeUtc.Value;
                }
                else
                {
                    lastTime = DateTime.UtcNow;
                }

                var schedule = CrontabSchedule.Parse(cronTrigger.Definition);
                return schedule.GetNextOccurrence(lastTime);
            }

            return null;
        }

        private void JobServiceOnTriggerUpdate(object sender, JobTriggerEventArgs args)
        {
            Console.WriteLine("Got an update for the job " + args.Trigger.Id);

            // Handle IsActive == false --> Remove Schedules
            // Handle IsActive == true --> Add New ones

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