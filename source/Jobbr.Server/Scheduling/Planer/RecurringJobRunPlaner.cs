using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using NCrontab;

namespace Jobbr.Server.Scheduling.Planer
{
    public class RecurringJobRunPlaner
    {
        private readonly ILogger<RecurringJobRunPlaner> _logger;
        private readonly JobbrRepository _jobbrRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RecurringJobRunPlaner(ILogger<RecurringJobRunPlaner> logger, JobbrRepository jobbrRepository, IDateTimeProvider dateTimeProvider)
        {
            _logger = logger;
            _jobbrRepository = jobbrRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        internal PlanResult Plan(RecurringTrigger trigger)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            if (trigger.EndDateTimeUtc.HasValue && trigger.EndDateTimeUtc.Value < _dateTimeProvider.GetUtcNow())
            {
                // This job run is not valid anymore
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            if (trigger.NoParallelExecution)
            {
                // find jobs that are running right now based on this trigger
                var hasRunningJob = _jobbrRepository.GetRunningJobs(trigger.JobId, trigger.Id).Any();

                if (hasRunningJob)
                {
                    var job = _jobbrRepository.GetJob(trigger.JobId);

                    _logger.LogInformation(
                     "No Parallel Execution: prevented planning of new job run for Job '{jobName}' (JobId: {jobId}). Caused by trigger with id '{triggerId}' (Type: '{triggerType}', userId: '{userId}', userName: '{userName}')",
                     job.UniqueName,
                     job.Id,
                     trigger.Id,
                     trigger.GetType().Name,
                     trigger.UserId,
                     trigger.UserDisplayName);

                    return PlanResult.FromAction(PlanAction.Blocked);
                }
            }

            DateTime baseTime;

            // Calculate the next occurrence based on current time or possible future startdate
            if (trigger.StartDateTimeUtc.HasValue && trigger.StartDateTimeUtc.Value > _dateTimeProvider.GetUtcNow())
            {
                baseTime = trigger.StartDateTimeUtc.Value;
            }
            else
            {
                baseTime = _dateTimeProvider.GetUtcNow();
            }

            var schedule = CrontabSchedule.Parse(trigger.Definition);

            return new PlanResult
            {
                Action = PlanAction.Possible,
                ExpectedStartDateUtc = schedule.GetNextOccurrence(baseTime),
            };
        }
    }
}