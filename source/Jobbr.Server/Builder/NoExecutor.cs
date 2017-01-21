using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.Builder
{
    class NoExecutor : IJobExecutor
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
            throw new NotImplementedException();
        }

        public bool OnJobRunCanceled(Guid uniqueId)
        {
            throw new NotImplementedException();
        }
    }
}
