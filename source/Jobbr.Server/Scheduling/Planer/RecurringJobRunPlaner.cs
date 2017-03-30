using System;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using NCrontab;

namespace Jobbr.Server.Scheduling.Planer
{
    public class RecurringJobRunPlaner
    {
        private static readonly ILog Logger = LogProvider.For<NewScheduler>();
        private readonly JobbrRepository jobbrRepository;
        private readonly IDateTimeProvider dateTimeProvider;

        public RecurringJobRunPlaner(JobbrRepository repository, IDateTimeProvider dateTimeProvider)
        {
            this.jobbrRepository = repository;
            this.dateTimeProvider = dateTimeProvider;
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

                    Logger.InfoFormat(
                        "No Parallel Execution: prevented planning of new JobRun for Job '{0}' (JobId: {1}). Caused by trigger with id '{2}' (Type: '{3}', userId: '{4}', userName: '{5}')",
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

            // Calculate the next occurance
            if (trigger.StartDateTimeUtc.HasValue && trigger.StartDateTimeUtc.Value > this.dateTimeProvider.GetUtcNow())
            {
                baseTime = trigger.StartDateTimeUtc.Value;
            }
            else
            {
                baseTime = this.dateTimeProvider.GetUtcNow();
            }

            var schedule = CrontabSchedule.Parse(trigger.Definition);

            return new PlanResult
            {
                Action = PlanAction.Possible,
                ExpectedStartDateUtc = schedule.GetNextOccurrence(baseTime)
            };
        }
    }
}