using System;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
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

            return new PlanResult { Action = PlanAction.Possible, ExpectedStartDateUtc = calculatedNextRun };
        }
    }
}