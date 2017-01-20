using System;

using System.Threading;

using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;

using NCrontab;

namespace Jobbr.Server.Core
{

    /// <summary>
    /// The Scheduler creates new scheduled Jobs in the JobRun Table based on the triggers
    /// </summary>
    public class DefaultScheduler : IDisposable
    {
        private static readonly ILog Logger = LogProvider.For<DefaultScheduler>();

        private readonly IStateService stateService;

        private readonly IJobbrConfiguration configuration;

        private readonly IJobbrRepository jobbrRepository;

        private readonly IJobManagementService jobManagementService;

        private Timer timer;

        public DefaultScheduler(IStateService stateService, IJobbrConfiguration configuration, IJobbrRepository jobbrRepository, IJobManagementService jobManagementService)
        {
            this.stateService = stateService;
            this.configuration = configuration;
            this.jobbrRepository = jobbrRepository;
            this.jobManagementService = jobManagementService;

            this.timer = new Timer(this.ScheduleJobRuns, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start()
        {
            // TODO: Wire again
            //this.stateService.TriggerUpdate += this.StateServiceOnTriggerUpdate;

            this.timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));
        }

        public void Stop()
        {
            this.timer.Change(Timeout.Infinite, Timeout.Infinite);

            // TODO Unwire again
            //this.stateService.TriggerUpdate -= this.StateServiceOnTriggerUpdate;
        }

        public void Dispose()
        {
            this.Stop();
        }

        private void ScheduleJobRuns(object state)
        {
            var alltriggers = this.jobbrRepository.GetActiveTriggers();

            foreach (var trigger in alltriggers)
            {
                try
                {
                    this.CreateSchedule(trigger);
                }
                catch (Exception e)
                {
                    Logger.ErrorException(string.Format("Exception thrown while creating schedule for trigger with Id '{0}' (Type: '{1}', userId: '{2}', userName: '{3}')", trigger.Id, trigger.GetType().Name, trigger.UserId, trigger.UserName), e);
                }
            }
        }

        private void CreateSchedule(JobTriggerBase trigger)
        {
            var job = this.jobbrRepository.GetJob(trigger.JobId);
            DateTime? calculatedNextRun = null;

            // Get the next occurence from database
            var plannedNextRun = this.jobbrRepository.GetNextJobRunByTriggerId(trigger.Id);

            // Calculate the next occurance for the trigger
            calculatedNextRun = this.GetNextTriggerDateTime(trigger as dynamic);

            if (calculatedNextRun < DateTime.UtcNow && trigger.IsActive)
            {
                Logger.WarnFormat("Active Disabling trigger for startdate '{0}', because historical startdate is not supported. Id '{1}' (Type: '{2}', userId: '{3}', userName: '{4}')", calculatedNextRun, trigger.Id, trigger.GetType().Name, trigger.UserId, trigger.UserName);

                // TODO: ReDo
                // this.jobManagementService.DisableTrigger(trigger.Id, false);
            }
            else if (calculatedNextRun != null)
            {
                if (plannedNextRun == null)
                {
                    var recurringTrigger = trigger as RecurringTrigger;

                    if (recurringTrigger != null && recurringTrigger.NoParallelExecution)
                    {
                        if (this.jobbrRepository.CheckParallelExecution(recurringTrigger.Id) == false)
                        {
                            Logger.InfoFormat(
                                "No Parallel Execution: prevented planning of new JobRun for Job '{0}' (JobId: {1}). Caused by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
                                job.UniqueName,
                                job.Id,
                                trigger.Id,
                                trigger.GetType().Name,
                                trigger.UserId,
                                trigger.UserName);

                            return;
                        }
                    }

                    Logger.InfoFormat(
                        "Planning new JobRun for Job '{0}' (JobId: {1}) to start @ '{2}'. Caused by trigger with id '{3}' (Type: '{4}', userId: '{5}', userName: '{6}')",
                        job.UniqueName,
                        job.Id,
                        calculatedNextRun.Value,
                        trigger.Id,
                        trigger.GetType().Name,
                        trigger.UserId,
                        trigger.UserName);

                    // TODO: ReDo
                    //this.stateService.CreateJobRun(job, trigger, calculatedNextRun.Value);
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
                            trigger.GetType().Name,
                            trigger.UserId,
                            trigger.UserName);
                    }
                    else
                    {
                        if (DateTime.UtcNow.AddSeconds(this.configuration.AllowChangesBeforeStartInSec) < calculatedNextRun)
                        {
                            Logger.WarnFormat(
                                "The calculated startdate '{0}' has changed to '{1}', the planned jobRun needs to be updated as next step",
                                plannedNextRun.PlannedStartDateTimeUtc,
                                calculatedNextRun.Value);

                            plannedNextRun.PlannedStartDateTimeUtc = calculatedNextRun.Value;

                            // TODO: ReDo
                            // this.stateService.UpdatePlannedStartDate(plannedNextRun.Id, plannedNextRun.PlannedStartDateTimeUtc);
                        }
                        else
                        {
                            Logger.WarnFormat(
                                "The planned startdate '{0}' has changed to '{1}'. This change was done too close (less than {2} seconds) to the next planned run and cannot be adjusted",
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
                    job.UniqueName,
                    job.Id,
                    trigger.Id,
                    trigger.GetType().Name,
                    trigger.UserId,
                    trigger.UserName);
            }
        }

        private DateTime? GetNextTriggerDateTime(ScheduledTrigger scheduledTrigger)
        {
            var startDate = scheduledTrigger.StartDateTimeUtc;

            return startDate;
        }

        private DateTime? GetNextTriggerDateTime(InstantTrigger instantTrigger)
        {
            // TODO: was based on the CreatedDateTimeUtc
            // var baseDateTimeUtc = instantTrigger.CreatedDateTimeUtc;
            var baseDateTimeUtc = DateTime.UtcNow;
            var startDate = baseDateTimeUtc.AddMinutes(instantTrigger.DelayedMinutes);

            // TODO: Review if this is still needed
            //this.jobManagementService.DisableTrigger(instantTrigger.Id, false);
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

        //private void StateServiceOnTriggerUpdate(object sender, JobTriggerEventArgs args)
        //{
        //    Logger.Log(
        //        LogLevel.Info,
        //        () =>
        //            {
        //                var job = this.jobbrRepository.GetJob(args.Trigger.JobId);
        //                return string.Format("Got new or updated trigger (Type: '{0}'. Id: '{1}', UserId: '{2}', UserName: '{3}' for job '{4}' (JobId: {5})", args.Trigger.TriggerType, args.Trigger.Id, args.Trigger.UserId, args.Trigger.UserName, job.UniqueName, job.Id);
        //            });

        //    if (args.Trigger.IsActive)
        //    {
        //        this.CreateSchedule(args.Trigger);
        //    }
        //    else
        //    {
        //        this.RemoveSchedule(args.Trigger);
        //    }
        //}

        //private void RemoveSchedule(JobTriggerBase trigger)
        //{
        //    var jobRun = this.jobbrRepository.GetNextJobRunByTriggerId(trigger.Id);
        //    this.stateService.UpdateJobRunState(jobRun, JobRunState.Deleted);
        //}
    }
}