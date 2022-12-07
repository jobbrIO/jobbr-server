using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// Interface for scheduled job run planners.
    /// </summary>
    public interface IScheduledJobRunPlaner
    {
        /// <summary>
        /// Plans a scheduled job run.
        /// </summary>
        /// <param name="trigger">Job run trigger.</param>
        /// <param name="isNew">If the plan is new.</param>
        /// <returns>The created <see cref="PlanResult"/>.</returns>
        PlanResult Plan(ScheduledTrigger trigger, bool isNew);
    }
}