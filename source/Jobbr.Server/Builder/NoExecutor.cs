using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// Executor that does nothing.
    /// </summary>
    public class NoExecutor : IJobExecutor
    {
        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Start()
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void Stop()
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        public void OnPlanChanged(List<PlannedJobRun> newPlan)
        {
        }

        /// <summary>
        /// Does nothing.
        /// </summary>
        /// <param name="id">Job ID.</param>
        /// <returns>Always return false.</returns>
        public bool OnJobRunCanceled(long id)
        {
            return false;
        }
    }
}
