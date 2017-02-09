using System;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    public class InstantJobRunPlaner
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public InstantJobRunPlaner(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        internal PlanResult Plan(InstantTrigger trigger, bool isNew = false)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var baseDateTimeUtc = trigger.CreatedAtUtc;
            var calculatedNextRun = baseDateTimeUtc.AddMinutes(trigger.DelayedMinutes);

            if (calculatedNextRun < this.dateTimeProvider.GetUtcNow() && !isNew)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}