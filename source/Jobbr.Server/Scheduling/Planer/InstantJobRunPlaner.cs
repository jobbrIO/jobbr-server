using System;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
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

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}