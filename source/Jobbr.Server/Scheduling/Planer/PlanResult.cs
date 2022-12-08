using System;

namespace Jobbr.Server.Scheduling.Planer
{
    public struct PlanResult
    {
        internal PlanAction Action;
        internal DateTime? ExpectedStartDateUtc;

        internal static PlanResult FromAction(PlanAction action)
        {
            return new PlanResult { Action = action };
        }
    }
}