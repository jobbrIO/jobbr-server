using System;

namespace Jobbr.Server.Scheduling.Planer
{
    internal struct PlanResult
    {
        internal PlanAction Action;
        internal DateTime? ExpectedStartDateUtc;

        internal static PlanResult FromAction(PlanAction action)
        {
            return new PlanResult() { Action = action };
        }
    }
}