using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// Interface for instant job run planners.
    /// </summary>
    public interface IInstantJobRunPlaner
    {
        /// <summary>
        /// Plans an instant job run.
        /// </summary>
        /// <param name="trigger">Job run trigger.</param>
        /// <param name="isNew">If the plan is new.</param>
        /// <returns>The created <see cref="PlanResult"/>.</returns>
        PlanResult Plan(InstantTrigger trigger, bool isNew = false);
    }
}