using System;
using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Scheduling.Planer;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server.Scheduling
{
    public class DefaultScheduler : IJobScheduler
    {
        private readonly ILogger<DefaultScheduler> _logger;
        private readonly IJobbrRepository _jobbrRepository;
        private readonly IJobExecutor _executor;
        private readonly IPeriodicTimer _periodicTimer;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IInstantJobRunPlaner _instantJobRunPlanner;
        private readonly IScheduledJobRunPlaner _scheduledJobRunPlanner;
        private readonly IRecurringJobRunPlaner _recurringJobRunPlanner;
        private readonly DefaultSchedulerConfiguration _schedulerConfiguration;
        private readonly object _evaluateTriggersLock = new ();

        private List<ScheduledPlanItem> _currentPlan = new ();

        public DefaultScheduler(ILoggerFactory loggerFactory, IJobbrRepository jobbrRepository, IJobExecutor executor, IInstantJobRunPlaner instantJobRunPlanner, IScheduledJobRunPlaner scheduledJobRunPlanner,
            IRecurringJobRunPlaner recurringJobRunPlanner, DefaultSchedulerConfiguration schedulerConfiguration, IPeriodicTimer periodicTimer, IDateTimeProvider dateTimeProvider)
        {
            _logger = loggerFactory.CreateLogger<DefaultScheduler>();
            _jobbrRepository = jobbrRepository;
            _executor = executor;
            _periodicTimer = periodicTimer;
            _dateTimeProvider = dateTimeProvider;
            _instantJobRunPlanner = instantJobRunPlanner;
            _scheduledJobRunPlanner = scheduledJobRunPlanner;
            _recurringJobRunPlanner = recurringJobRunPlanner;
            _schedulerConfiguration = schedulerConfiguration;

            _periodicTimer.Setup(EvaluateRecurringTriggers);
        }

        public void Dispose()
        {
        }

        public void Start()
        {
            SetScheduledJobRunsFromPastToOmitted();

            SetRunningJobsToFailed();

            CreateInitialPlan();

            _periodicTimer.Start();
        }

        public void Stop()
        {
            _periodicTimer.Stop();
        }

        public void OnTriggerDefinitionUpdated(long jobId, long triggerId)
        {
            lock (_evaluateTriggersLock)
            {
                _logger.LogInformation("The trigger with id '{triggerId}' has been updated. Reflecting changes to Plan if any.", triggerId);
                var trigger = _jobbrRepository.GetTriggerById(jobId, triggerId);

                PlanResult planResult = GetPlanResult(trigger as dynamic, false);

                if (planResult.Action != PlanAction.Possible)
                {
                    _logger.LogDebug("The trigger was not considered to be relevant to the plan, skipping. PlanResult was '{action}'", planResult.Action);
                    return;
                }

                var dateTime = planResult.ExpectedStartDateUtc;

                if (!dateTime.HasValue)
                {
                    _logger.LogWarning("Unable to gather an expected start date for trigger, skipping.");
                    return;
                }

                // Get the next occurrence from database
                var dependentJobRun = _jobbrRepository.GetNextJobRunByTriggerId(jobId, trigger.Id, _dateTimeProvider.GetUtcNow());

                if (dependentJobRun == null)
                {
                    _logger.LogError("Trigger was updated before job run has been created. Cannot apply update.");
                    return;
                }

                UpdatePlannedJobRun(dependentJobRun, trigger, dateTime.Value);
            }
        }

        public void OnTriggerStateUpdated(long jobId, long triggerId)
        {
            lock (_evaluateTriggersLock)
            {
                _logger.LogInformation("The trigger with id '{triggerId}' has been changed its state. Reflecting changes to Plan if any.", triggerId);

                var trigger = _jobbrRepository.GetTriggerById(jobId, triggerId);

                PlanResult planResult = GetPlanResult(trigger as dynamic, false);

                if (planResult.Action == PlanAction.Obsolete)
                {
                    // Remove from in memory plan to not publish this in future
                    _currentPlan.RemoveAll(e => e.TriggerId == triggerId);

                    // Set the JobRun to deleted if any
                    var dependentJobRun = _jobbrRepository.GetNextJobRunByTriggerId(jobId, trigger.Id, _dateTimeProvider.GetUtcNow());

                    if (dependentJobRun != null)
                    {
                        _jobbrRepository.Delete(dependentJobRun);
                    }

                    PublishCurrentPlan();

                    return;
                }

                if (planResult.Action == PlanAction.Possible)
                {
                    var newItem = CreateNew(planResult, trigger);

                    if (newItem != null)
                    {
                        _currentPlan.Add(newItem);

                        PublishCurrentPlan();
                    }
                }
            }
        }

        public void OnTriggerAdded(long jobId, long triggerId)
        {
            lock (_evaluateTriggersLock)
            {
                _logger.LogInformation($"The trigger with id '{triggerId}' has been added. Reflecting changes to the current plan.", triggerId);

                var trigger = _jobbrRepository.GetTriggerById(jobId, triggerId);

                PlanResult planResult = GetPlanResult(trigger as dynamic, true);

                if (planResult.Action != PlanAction.Possible)
                {
                    _logger.LogDebug("The trigger was not considered to be relevant to the plan, skipping. PlanResult was '{action}'", planResult.Action);
                    return;
                }

                var newItem = CreateNew(planResult, trigger);

                if (newItem == null)
                {
                    _logger.LogError("Unable to create a new Planned Item with a JobRun.");
                    return;
                }

                _currentPlan.Add(newItem);

                PublishCurrentPlan();
            }
        }

        public void OnJobRunEnded(long id)
        {
            lock (_evaluateTriggersLock)
            {
                _logger.LogInformation("A JobRun has ended. Reevaluating triggers that did not yet schedule a run");

                // Remove from in memory plan to not publish this in future
                var numberOfDeletedItems = _currentPlan.RemoveAll(e => e.Id == id);

                var additionalItems = new List<ScheduledPlanItem>();

                // If a trigger was blocked previously, it might be a candidate to schedule now
                var activeTriggers = _jobbrRepository.GetActiveTriggers(pageSize: int.MaxValue).Items;

                foreach (var trigger in activeTriggers)
                {
                    if (_currentPlan.Any(p => p.TriggerId == trigger.Id))
                    {
                        continue;
                    }

                    PlanResult planResult = GetPlanResult(trigger as dynamic, false);

                    if (planResult.Action == PlanAction.Possible)
                    {
                        var scheduledItem = CreateNew(planResult, trigger);

                        additionalItems.Add(scheduledItem);
                    }
                }

                if (additionalItems.Any() || numberOfDeletedItems > 0)
                {
                    _logger.LogInformation("The completion of a previous job caused the addition of {itemCount} and removal of {deletedCount} scheduled items", additionalItems.Count, numberOfDeletedItems);
                    _currentPlan.AddRange(additionalItems);

                    PublishCurrentPlan();
                }
                else
                {
                    _logger.LogDebug("There was no possibility to scheduled new items after the completion of job with it '{id}'.", id);
                }
            }
        }

        private void EvaluateRecurringTriggers()
        {
            lock (_evaluateTriggersLock)
            {
                // Re-evaluate recurring triggers every n seconds
                var activeTriggers = _jobbrRepository.GetActiveTriggers(pageSize: int.MaxValue).Items.Where(t => t.GetType() == typeof(RecurringTrigger));

                var additionalItems = new List<ScheduledPlanItem>();

                foreach (var trigger in activeTriggers.Cast<RecurringTrigger>())
                {
                    var planResult = GetPlanResult(trigger, false);

                    if (planResult.Action == PlanAction.Possible)
                    {
                        // Check if there is already a run planned at this time
                        var nextRunForTrigger = _jobbrRepository.GetNextJobRunByTriggerId(trigger.JobId, trigger.Id, _dateTimeProvider.GetUtcNow());

                        if (nextRunForTrigger == null || !nextRunForTrigger.PlannedStartDateTimeUtc.Equals(planResult.ExpectedStartDateUtc))
                        {
                            var scheduledItem = CreateNew(planResult, trigger);
                            additionalItems.Add(scheduledItem);
                        }
                    }
                }

                if (additionalItems.Any())
                {
                    _logger.LogInformation("The re-evaluation of recurring triggers caused the addition of {itemCount} scheduled items", additionalItems.Count);
                    _currentPlan.AddRange(additionalItems);

                    PublishCurrentPlan();
                }
            }
        }

        private ScheduledPlanItem CreateNew(PlanResult planResult, JobTriggerBase trigger)
        {
            var dateTime = planResult.ExpectedStartDateUtc;

            if (!dateTime.HasValue)
            {
                _logger.LogWarning("Unable to gather an expected start date for trigger with id {id}, (JobId: {jobId}), skipping.", trigger.Id, trigger.JobId);

                return null;
            }

            // Create the next occurrence from database
            var newJobRun = CreateNewJobRun(trigger, dateTime.Value);

            // Add to the initial plan
            var newItem = new ScheduledPlanItem
            {
                TriggerId = trigger.Id,
                Id = newJobRun.Id,
                JobId = trigger.JobId,
                PlannedStartDateTimeUtc = newJobRun.PlannedStartDateTimeUtc,
            };

            return newItem;
        }

        private void SetScheduledJobRunsFromPastToOmitted()
        {
            var dateTime = _dateTimeProvider.GetUtcNow();
            var scheduledJobRuns = _jobbrRepository.GetJobRunsByState(JobRunStates.Scheduled, pageSize: int.MaxValue).Items.Where(p => p.PlannedStartDateTimeUtc < dateTime).ToList();

            if (!scheduledJobRuns.Any())
            {
                _logger.LogDebug("There were no jobs found that had been planned before {dateTime}", dateTime);
                return;
            }

            _logger.LogInformation("There were {jobRunCount} job runs that should have been started while the JobServer was not running. Need to omit them...", scheduledJobRuns.Count);
            foreach (var jobRun in scheduledJobRuns)
            {
                jobRun.State = JobRunStates.Omitted;
                _jobbrRepository.Update(jobRun);
                _logger.LogDebug("Omitted JobRun with id {jobRunId} for job {jobId} that has been planned for {plannedStartTime}", jobRun.Id, jobRun.Job.Id, jobRun.PlannedStartDateTimeUtc);
            }
        }

        private void SetRunningJobsToFailed()
        {
            var runningJobRuns = _jobbrRepository.GetRunningJobs().ToList();

            if (!runningJobRuns.Any())
            {
                _logger.LogDebug("There were no uncompleted JobRuns while starting. The last shutdown seems to be healthy.");
                return;
            }

            _logger.LogWarning("{runningCount} JobRuns are still in the 'Running'-State. They which may have crashed after an unhealthy shutdown.", runningJobRuns.Count);
            _logger.LogInformation("Need to manually set {runningCount} JobRuns to the state 'Failed'...", runningJobRuns.Count);

            foreach (var jobRun in runningJobRuns)
            {
                jobRun.State = JobRunStates.Failed;
                _jobbrRepository.Update(jobRun);

                _logger.LogDebug("Set JobRun with id {jobRunId} for job {jobId} to the state 'Failed'. Possible Reason: Unclean shutdown.", jobRun.Id, jobRun.Job.Id);
            }
        }

        private void CreateInitialPlan()
        {
            var activeTriggers = _jobbrRepository.GetActiveTriggers(pageSize: int.MaxValue).Items;

            var newPlan = new List<ScheduledPlanItem>();

            foreach (var trigger in activeTriggers)
            {
                PlanResult planResult = GetPlanResult(trigger as dynamic, false);

                if (planResult.Action == PlanAction.Obsolete)
                {
                    _logger.LogWarning("Disabling trigger with id '{id}', because start date is in the past. (Type: '{typeName}', userId: '{userId}', userName: '{name}')", trigger.Id, trigger.GetType().Name, trigger.UserId, trigger.UserDisplayName);

                    _jobbrRepository.DisableTrigger(trigger.JobId, trigger.Id);
                    continue;
                }

                if (planResult.Action == PlanAction.Blocked)
                {
                    // Cannot schedule job run, one reason could be that this job is not allowed to run because another job run is active
                    continue;
                }

                if (planResult.Action == PlanAction.Possible)
                {
                    if (planResult.ExpectedStartDateUtc == null)
                    {
                        // Move to ctor of PlanResult
                        throw new ArgumentNullException(nameof(planResult.ExpectedStartDateUtc));
                    }

                    var dateTime = planResult.ExpectedStartDateUtc;

                    // Get the next occurrence from database
                    var dependentJobRun = _jobbrRepository.GetNextJobRunByTriggerId(trigger.JobId, trigger.Id, _dateTimeProvider.GetUtcNow());

                    if (dependentJobRun != null)
                    {
                        UpdatePlannedJobRun(dependentJobRun, trigger, dateTime.Value);
                    }
                    else
                    {
                        dependentJobRun = CreateNewJobRun(trigger, dateTime.Value);
                    }

                    // Add to the initial plan
                    newPlan.Add(new ScheduledPlanItem
                    {
                        TriggerId = trigger.Id,
                        Id = dependentJobRun.Id,
                        PlannedStartDateTimeUtc = dependentJobRun.PlannedStartDateTimeUtc,
                        JobId = trigger.JobId,
                    });
                }
            }

            // Set current plan
            _currentPlan = newPlan;

            // Publish the initial plan top the Executor
            PublishCurrentPlan();
        }

        private void PublishCurrentPlan()
        {
            _logger.LogInformation("Getting new plan for upcoming scheduled jobs to the _executor. Number of Items: {planCount}", _currentPlan.Count);

            var possibleJobRuns = MaxConcurrentJobRunPlaner.GetPossiblePlannedJobRuns(_currentPlan, _jobbrRepository);

            _logger.LogInformation("Publishing new plan for upcoming planned jobs to the _executor. Number of Items: {jobRunCount}", possibleJobRuns.Count);

            try
            {
                _executor.OnPlanChanged(possibleJobRuns);
            }
            catch (Exception e)
            {
                _logger.LogWarning(e, "Unable to publish current plan to Executor.");
            }
        }

        private JobRun CreateNewJobRun(JobTriggerBase trigger, DateTime dateTime)
        {
            var job = _jobbrRepository.GetJob(trigger.JobId);

            var jobRun = _jobbrRepository.SaveNewJobRun(job, trigger, dateTime);

            return jobRun;
        }

        private void UpdatePlannedJobRun(JobRun plannedNextRun, JobTriggerBase trigger, DateTime calculatedNextRun)
        {
            // Is this value in sync with the schedule table?
            if (plannedNextRun.PlannedStartDateTimeUtc == calculatedNextRun)
            {
                _logger.LogDebug(
                    "The previously planned start date '{nextRunStart}' is still correct for JobRun (id: {nextRunId}) triggered by trigger with id '{triggerId}' (Type: '{triggerType}', userId: '{triggerUserId}', userName: '{triggerUserName}')",
                    calculatedNextRun,
                    plannedNextRun.Id,
                    trigger.Id,
                    trigger.GetType().Name,
                    trigger.UserId,
                    trigger.UserDisplayName);

                return;
            }

            // Was the change too close before the execution date?
            var utcNow = _dateTimeProvider.GetUtcNow();

            if (utcNow.AddSeconds(_schedulerConfiguration.AllowChangesBeforeStartInSec) >= calculatedNextRun)
            {
                _logger.LogWarning(
                    "The planned start date '{startTime}' has changed to '{nextRunStart}'. This change was done too close (less than {changeWindowSecs} seconds) to the next planned run and cannot be adjusted",
                    plannedNextRun.PlannedStartDateTimeUtc,
                    calculatedNextRun,
                    _schedulerConfiguration.AllowChangesBeforeStartInSec);

                return;
            }

            _logger.LogWarning("The calculated start date '{startTime}' has changed to '{nextRunStart}', the planned job run needs to be updated as next step", plannedNextRun.PlannedStartDateTimeUtc, calculatedNextRun);

            plannedNextRun.PlannedStartDateTimeUtc = calculatedNextRun;
            _jobbrRepository.Update(plannedNextRun);
        }

        private PlanResult GetPlanResult(InstantTrigger trigger, bool isNew = false) => _instantJobRunPlanner.Plan(trigger, isNew);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(ScheduledTrigger trigger, bool isNew = false) => _scheduledJobRunPlanner.Plan(trigger, isNew);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(RecurringTrigger trigger, bool isNew = false) => _recurringJobRunPlanner.Plan(trigger);

        // ReSharper disable once UnusedParameter.Local
        // Reason: Parameter is used by dynamic overload
        private PlanResult GetPlanResult(object trigger, bool isNew)
        {
            throw new NotImplementedException($"Unable to dynamic dispatch trigger of type '{trigger?.GetType().Name}'");
        }
    }
}
