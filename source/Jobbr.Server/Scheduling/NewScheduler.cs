using System;
using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using NCrontab;

namespace Jobbr.Server.Scheduling
{
    public class InstantJobRunPlaner
    {
        internal PlanResult Plan(InstantTrigger trigger, bool isNew = false)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var baseDateTimeUtc = trigger.CreatedAtUtc;
            var calculatedNextRun = baseDateTimeUtc.AddMinutes(trigger.DelayedMinutes);

            if (calculatedNextRun < DateTime.UtcNow && !isNew)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return PlanResult.FromAction(PlanAction.Possible);
        }
    }

    public class ScheduledJobRunPlaner
    {
        internal PlanResult Plan(ScheduledTrigger trigger)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var calculatedNextRun = trigger.StartDateTimeUtc;

            if (calculatedNextRun < DateTime.UtcNow)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return PlanResult.FromAction(PlanAction.Possible);
        }
    }

    public class RecurringJobRunPlaner
    {
        private static readonly ILog Logger = LogProvider.For<NewScheduler>();

        private readonly JobbrRepository jobbrRepository;

        public RecurringJobRunPlaner(JobbrRepository jobbrRepository)
        {
            this.jobbrRepository = jobbrRepository;
        }

        internal PlanResult Plan(RecurringTrigger trigger)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            if (trigger.NoParallelExecution)
            {
                if (this.jobbrRepository.CheckParallelExecution(trigger.Id) == false)
                {
                    var job = this.jobbrRepository.GetJob(trigger.JobId);

                    Logger.InfoFormat("No Parallel Execution: prevented planning of new JobRun for Job '{0}' (JobId: {1}). Caused by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
                        job.UniqueName,
                        job.Id,
                        trigger.Id,
                        trigger.GetType().Name,
                        trigger.UserId,
                        trigger.UserName);

                    return PlanResult.FromAction(PlanAction.Blocked);
                }
            }

            DateTime baseTime;

            // Calculate the next occurance
            if (trigger.StartDateTimeUtc.HasValue && trigger.StartDateTimeUtc.Value > DateTime.UtcNow)
            {
                baseTime = trigger.StartDateTimeUtc.Value;
            }
            else
            {
                baseTime = DateTime.UtcNow;
            }

            var schedule = CrontabSchedule.Parse(trigger.Definition);

            return new PlanResult
            {
                Action = PlanAction.Possible,
                ExpectedStartDateUtc = schedule.GetNextOccurrence(baseTime)
            };
        }
    }

    internal struct PlanResult
    {
        internal PlanAction Action;
        internal DateTime? ExpectedStartDateUtc;

        internal static PlanResult FromAction(PlanAction action)
        {
            return new PlanResult() { Action = action };
        }
    }

    internal enum PlanAction
    {
        Obsolete,
        Blocked,
        Possible
    }

    public class NewScheduler : IJobScheduler
    {
        private static readonly ILog Logger = LogProvider.For<NewScheduler>();

        private readonly IJobbrRepository repository;
        private readonly IJobExecutor executor;
        private List<TriggerPlannedJobRunCombination> currentPlan = new List<TriggerPlannedJobRunCombination>();

        private InstantJobRunPlaner instantJobRunPlaner;
        private ScheduledJobRunPlaner scheduledJobRunPlaner;
        private RecurringJobRunPlaner recurringJobRunPlaner;
        private readonly DefaultSchedulerConfiguration configuration;

        public NewScheduler(IJobbrRepository repository, IJobExecutor executor, InstantJobRunPlaner instantJobRunPlaner, ScheduledJobRunPlaner scheduledJobRunPlaner, RecurringJobRunPlaner recurringJobRunPlaner, DefaultSchedulerConfiguration configuration)
        {
            this.repository = repository;
            this.executor = executor;

            this.instantJobRunPlaner = instantJobRunPlaner;
            this.scheduledJobRunPlaner = scheduledJobRunPlaner;
            this.recurringJobRunPlaner = recurringJobRunPlaner;

            this.configuration = configuration;
        }

        public void Dispose()
        {
        }

        public void Start()
        {
            this.CreateInitialPlan();
        }

        public void Stop()
        {
        }

        private void CreateInitialPlan()
        {
            var activeTriggers = this.repository.GetActiveTriggers();

            var newPlan = new List<TriggerPlannedJobRunCombination>();
            foreach (var trigger in activeTriggers)
            {
                PlanResult planResult = this.GetPlanResult(trigger as dynamic, false);

                if (planResult.Action == PlanAction.Obsolete)
                {
                    Logger.WarnFormat($"Disabling trigger with id '{trigger.Id}', because startdate is in the past. (Type: '{trigger.GetType().Name}', userId: '{trigger.UserId}', userName: '{trigger.UserName}')");

                    this.repository.DisableTrigger(trigger.Id);
                    continue;
                }

                if (planResult.Action == PlanAction.Blocked)
                {
                    // Cannot schedule jobrun, one reason could be that this job is not allowed to run because another jobrun is active
                    continue;
                }

                if (planResult.Action == PlanAction.Possible)
                {
                    if (planResult.ExpectedStartDateUtc == null)
                    {
                        // Move to ctor of PlanResult
                        throw new ArgumentNullException("ExpectedStartDateUtc");
                    }

                    var dateTime = planResult.ExpectedStartDateUtc;

                    // Get the next occurence from database
                    var dependentJobRun = this.repository.GetNextJobRunByTriggerId(trigger.Id);

                    if (dependentJobRun != null)
                    {
                        this.UpdatePlannedJobRun(dependentJobRun, trigger, dateTime.Value);
                    }
                    else
                    {
                        dependentJobRun = this.CreateNewJobRun(trigger, dateTime.Value);
                    }

                    // Add to the initial plan
                    newPlan.Add(new TriggerPlannedJobRunCombination()
                    {
                        TriggerId = trigger.Id,
                        UniqueId = dependentJobRun.UniqueId,
                        PlannedStartDateTimeUtc = dependentJobRun.PlannedStartDateTimeUtc
                    });
                }
            }

            // Set current plan
            this.currentPlan = newPlan;

            // Publish the initial plan top the Excutor
            this.PublishCurrentPlan();
        }

        public void OnTriggerDefinitionUpdated(long triggerId)
        {
            
        }

        public void OnTriggerAdded(long triggerId)
        {
            
        }

        public void OnJobRunEnded(long jobRunId)
        {
            
        }

        private void PublishCurrentPlan()
        {
            var clone = this.currentPlan.Select(e => new PlannedJobRun() { PlannedStartDateTimeUtc = e.PlannedStartDateTimeUtc, UniqueId = e.UniqueId }).ToList();

            try
            {
                this.executor.OnPlanChanged(clone);
            }
            catch (Exception e)
            {
                Logger.WarnException("Unable to publish current plan to Executor", e);
            }
        }

        private JobRun CreateNewJobRun(JobTriggerBase trigger, DateTime dateTime)
        {
            var job = this.repository.GetJob(trigger.JobId);

            var jobRun = this.repository.SaveNewJobRun(job, trigger, dateTime);

            return jobRun;
        }

        private void UpdatePlannedJobRun(JobRun plannedNextRun, JobTriggerBase trigger, DateTime calculatedNextRun)
        {
            // Is this value in sync with the schedule table?
            if (plannedNextRun.PlannedStartDateTimeUtc == calculatedNextRun)
            {
                Logger.DebugFormat(
                    "The previously planned startdate '{0}' is still correct for JobRun (id: {1}) triggered by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
                    calculatedNextRun,
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
                    Logger.WarnFormat("The calculated startdate '{0}' has changed to '{1}', the planned jobRun needs to be updated as next step", plannedNextRun.PlannedStartDateTimeUtc, calculatedNextRun);

                    plannedNextRun.PlannedStartDateTimeUtc = calculatedNextRun;
                    this.repository.Update(plannedNextRun);
                }
                else
                {
                    Logger.WarnFormat(
                        "The planned startdate '{0}' has changed to '{1}'. This change was done too close (less than {2} seconds) to the next planned run and cannot be adjusted",
                        plannedNextRun.PlannedStartDateTimeUtc,
                        calculatedNextRun,
                        this.configuration.AllowChangesBeforeStartInSec);
                }
            }
        }

        private PlanResult GetPlanResult(InstantTrigger trigger, bool isNew = false) => this.instantJobRunPlaner.Plan(trigger, isNew);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(ScheduledTrigger trigger, bool isNew = false) => this.scheduledJobRunPlaner.Plan(trigger);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(RecurringTrigger trigger, bool isNew = false) => this.recurringJobRunPlaner.Plan(trigger);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(object trigger, bool isNew)
        {
            throw new NotImplementedException($"Unable to dynamic dispatch trigger of type '{trigger?.GetType().Name}'");
        }
    }

    internal class TriggerPlannedJobRunCombination
    {
        public Guid UniqueId { get; set; }

        public DateTime PlannedStartDateTimeUtc { get; set; }

        public long TriggerId { get; set; }
    }
}
