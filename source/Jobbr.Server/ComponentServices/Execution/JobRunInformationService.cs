using System;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.ComponentModel.Services
{
    internal class JobRunInformationService : IJobRunInformationService
    {
        private readonly IJobbrRepository jobbrRepository;

        public JobRunInformationService(IJobbrRepository jobbrRepository)
        {
            this.jobbrRepository = jobbrRepository;
        }

        public JobRunInfo GetByUniqueId(Guid uniqueId)
        {
            throw new NotImplementedException();
        }

        public JobRunInfo GetByJobRunId(long jobRunId)
        {
            var job = this.jobbrRepository.GetJobRunById(jobRunId);

            if (job == null)
            {
                return null;
            }

            return new JobRunInfo()
            {
                JobId = job.Id,
                // TODO
            };
        }
    }
}