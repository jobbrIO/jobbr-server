using System;
using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;

namespace Jobbr.Server.ComponentServices.Execution
{
    internal class JobRunInformationService : IJobRunInformationService
    {
        private static ILog Logger = LogProvider.For<JobRunInformationService>();

        private readonly IJobbrRepository jobbrRepository;
        private readonly IMapper mapper;

        public JobRunInformationService(IJobbrRepository jobbrRepository, IMapper mapper)
        {
            this.jobbrRepository = jobbrRepository;
            this.mapper = mapper;
        }

        public JobRunInfo GetByUniqueId(Guid uniqueId)
        {
            throw new NotImplementedException();
        }

        public JobRunInfo GetByJobRunId(long jobRunId)
        {
            Logger.Debug($"Retrieving information regarding jobrun with id '{jobRunId}'");

            var jobRun = this.jobbrRepository.GetJobRunById(jobRunId);

            if (jobRun == null)
            {
                return null;
            }

            var trigger = this.jobbrRepository.GetTriggerById(jobRun.TriggerId);
            var job = this.jobbrRepository.GetJob(jobRun.Id);

            var info = new JobRunInfo();

            this.mapper.Map(job, info);
            this.mapper.Map(trigger, info);
            this.mapper.Map(jobRun, info);

            return info;
        }
    }
}