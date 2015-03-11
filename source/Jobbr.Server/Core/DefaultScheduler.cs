using System;
using System.Runtime.CompilerServices;
using System.Threading;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

using NCrontab;

namespace Jobbr.Server.Core
{
    using Jobbr.Server.Logging;

    /// <summary>
    /// The Scheduler creates new scheduled Jobs in the JobRun Table based on the triggers
    /// </summary>
    public class DefaultScheduler : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<DefaultScheduler>();

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
            var plannedNextRun = this.jobService.GetNextJobRunByTriggerId(trigger.Id);

            // Calculate the next occurance for the trigger
            calculatedNextRun = this.GetNextTriggerDateTime(trigger as dynamic);

            if (calculatedNextRun < DateTime.UtcNow && trigger.IsActive)
            {
                Logger.WarnFormat("Active Disabling trigger for startdate '{0}', because historical startdate is not supported.  Id '{1}' (Type: '{2}', userId: '{3}', userName: '{4}')", calculatedNextRun, trigger.Id, trigger.TriggerType, trigger.UserId, trigger.UserName);
                this.jobService.DisableTrigger(trigger.Id, false);
            }
            else if (calculatedNextRun != null)
            {
                if (plannedNextRun == null)
                {
                    Logger.InfoFormat(
                        "Planning new JobRun for Job '{0}' (JobId: {1}) to start @ '{2}'. Caused by trigger with id '{3}' (Type: '{4}', userId: '{5}', userName: '{6}')",
                        job.Name,
                        job.Id,
                        calculatedNextRun.Value,
                        trigger.Id,
                        trigger.TriggerType,
                        trigger.UserId,
                        trigger.UserName);

                    this.jobService.CreateJobRun(job, trigger, calculatedNextRun.Value);
                }
                else
                {
                    // Is this value in sync with the schedule table?
                    if (plannedNextRun.PlannedStartDateTimeUtc == calculatedNextRun)
                    {
                        Logger.DebugFormat(
                            "The planned startdate '{0}' is still correct for JobRun (id: {1}) triggered by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
                            calculatedNextRun.Value,
                            plannedNextRun.Id,
                            trigger.Id,
                            trigger.TriggerType,
                            trigger.UserId,
                            trigger.UserName);
                    }
                    else
                    {
                        if (plannedNextRun.PlannedStartDateTimeUtc.AddSeconds(this.configuration.AllowChangesBeforeStartInSec) < calculatedNextRun)
                        {
                            Logger.WarnFormat(
                                "The planned startdate '{0}' has changed to '{1}'. This trigger should be updated in database and the DefaultJobExecutor should be notified",
                                plannedNextRun.PlannedStartDateTimeUtc,
                                calculatedNextRun.Value);

                            // TODO: Change the trigger and make shure thath the schedule and queue of the jobexecutor is adjusted accordingly.
                        }
                        else
                        {
                            Logger.WarnFormat(
                                "The planned startdate '{0}' has changed to '{1}'. This change was done too close (less than {2} seconds) to the next planned run and cannnot be reflected",
                                plannedNextRun.PlannedStartDateTimeUtc,
                                calculatedNextRun.Value,
                                this.configuration.AllowChangesBeforeStartInSec);
                        }
                    }
                }
            }
            else
            {
                Logger.WarnFormat(
                    "Cannot calculate next run for Job '{0}' (JobId: {1}). Caused by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
                    job.Name,
                    job.Id,
                    trigger.Id,
                    trigger.TriggerType,
                    trigger.UserId,
                    trigger.UserName);
            }
        }

        private DateTime? GetNextTriggerDateTime(ScheduledTrigger scheduledTrigger)
        {
            var startDate = scheduledTrigger.DateTimeUtc;

            return startDate;
        }

        private DateTime? GetNextTriggerDateTime(InstantTrigger instantTrigger)
        {
            var baseDateTimeUtc = instantTrigger.CreateDateTimeUtc;
            var startDate = baseDateTimeUtc.AddMinutes(instantTrigger.DelayedMinutes);

            this.jobService.DisableTrigger(instantTrigger.Id, false);
            instantTrigger.IsActive = false;
            
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
            Logger.Log(
                LogLevel.Info,
                () =>
                    {
                        var job = this.jobService.GetJob(args.Trigger.JobId);
                        return string.Format("Got new or updated trigger (Type: '{0}'. Id: '{1}', UserId: '{2}', UserName: '{3}' for job '{4}' (JobId: {5})", args.Trigger.TriggerType, args.Trigger.Id, args.Trigger.UserId, args.Trigger.UserName, job.Name, job.Id);
                    });

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