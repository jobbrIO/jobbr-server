using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// Class for planning instant job runs.
    /// </summary>
    public class InstantJobRunPlaner : IInstantJobRunPlaner
    {
        private readonly IDateTimeProvider _dateTimeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantJobRunPlaner"/> class.
        /// </summary>
        public InstantJobRunPlaner(IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
        }

        /// <inheritdoc/>
        public PlanResult Plan(InstantTrigger trigger, bool isNew = false)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var baseDateTimeUtc = trigger.CreatedDateTimeUtc;
            var calculatedNextRun = baseDateTimeUtc.AddMinutes(trigger.DelayedMinutes);

            if (calculatedNextRun < _dateTimeProvider.GetUtcNow() && !isNew)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}