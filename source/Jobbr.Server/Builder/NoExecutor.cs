using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.Builder
{
    public class NoExecutor : IJobExecutor
    {
        public void Dispose()
        {
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void OnPlanChanged(List<PlannedJobRun> newPlan)
        {
        }

        public bool OnJobRunCanceled(long id)
        {
            return false;
        }
    }
}
