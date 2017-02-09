using System;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    public class ScheduledJobRunPlaner
    {
        private readonly IDateTimeProvider dateTimeProvider;

        public ScheduledJobRunPlaner(IDateTimeProvider dateTimeProvider)
        {
            this.dateTimeProvider = dateTimeProvider;
        }

        internal PlanResult Plan(ScheduledTrigger trigger, bool isNew)
        {
            if (!trigger.IsActive)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            var calculatedNextRun = trigger.StartDateTimeUtc;

            if (calculatedNextRun < this.dateTimeProvider.GetUtcNow() && !isNew)
            {
                return PlanResult.FromAction(PlanAction.Obsolete);
            }

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}