using System;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// The result of a plan.
    /// </summary>
    public struct PlanResult
    {
        /// <summary>
        /// Plan action.
        /// </summary>
        internal PlanAction Action;

        /// <summary>
        /// Expected start date of the plan in UTC.
        /// </summary>
        internal DateTime? ExpectedStartDateUtc;

        /// <summary>
        /// Creates a <see cref="PlanResult"/> from a <see cref="PlanAction"/>.
        /// </summary>
        /// <param name="action">Plan action to create the result from.</param>
        /// <returns>A <see cref="PlanResult"/> created from the <see cref="PlanAction"/>.</returns>
        internal static PlanResult FromAction(PlanAction action)
        {
            return new PlanResult
            {
                Action = action
            };
        }
    }
}