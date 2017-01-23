using System;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.ComponentModel.Services
{
    internal class JobRunInformationService : IJobRunInformationService
    {
        public JobRunInfo GetByUniqueId(Guid uniqueId)
        {
            throw new NotImplementedException();
        }

        public JobRunInfo GetByJobRunId(long jobRunId)
        {
            throw new NotImplementedException();
        }
    }
}