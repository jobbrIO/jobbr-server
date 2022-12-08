using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Scheduling.Planer
{
    /// <summary>
    /// Interface for recurring job run planners.
    /// </summary>
    public interface IRecurringJobRunPlaner
    {
        /// <summary>
        /// Plans a recurring job run.
        /// </summary>
        /// <param name="trigger">Job run trigger.</param>
        /// <returns>The created <see cref="PlanResult"/>.</returns>
        PlanResult Plan(RecurringTrigger trigger);
    }
}