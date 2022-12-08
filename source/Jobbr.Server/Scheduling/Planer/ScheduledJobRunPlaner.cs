using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// Class for planning scheduled job runs.
    /// </summary>
    public class ScheduledJobRunPlaner : IScheduledJobRunPlaner
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledJobRunPlaner"/> class.
        /// </summary>
        public ScheduledJobRunPlaner(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public PlanResult Plan(ScheduledTrigger trigger, bool isNew)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var calculatedNextRun = trigger.StartDateTimeUtc;

            if (calculatedNextRun < _dateTimeProvider.GetUtcNow() && !isNew)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}