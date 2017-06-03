using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Tests.Infrastructure
{
    public class PseudoExecutor : IJobExecutor
    {
        public void Dispose()
        {
        }

        public void Start()
        {
        }

        public void OnPlanChanged(List<PlannedJobRun> newPlan)
        {
        }

        public bool OnJobRunCanceled(long id)
        {
            return false;
        }

        public void Stop()
        {
        }
    }
}