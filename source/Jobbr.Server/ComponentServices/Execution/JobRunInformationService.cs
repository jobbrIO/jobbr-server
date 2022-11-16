using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server.ComponentServices.Execution
{
    /// <summary>
    /// Service for retrieving information on job runs.
    /// </summary>
    internal class JobRunInformationService : IJobRunInformationService
    {
        private readonly ILogger<JobRunInformationService> _logger;
        private readonly IJobbrRepository _jobbrRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunInformationService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="jobbrRepository">Repository that contains the jobs.</param>
        /// <param name="mapper">The mapper.</param>
        public JobRunInformationService(ILogger<JobRunInformationService> logger, IJobbrRepository jobbrRepository, IMapper mapper)
        {
            _logger = logger;
            _jobbrRepository = jobbrRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets a job by using the run ID.
        /// </summary>
        /// <param name="jobRunId">The run ID.</param>
        /// <returns>A <see cref="JobRunInfo"/>.</returns>
        public JobRunInfo GetByJobRunId(long jobRunId)
        {
            _logger.LogDebug("Retrieving information regarding job run with ID '{jobRunId}'", jobRunId);

            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);

            if (jobRun == null)
            {
                return null;
            }

            var trigger = _jobbrRepository.GetTriggerById(jobRun.Job.Id, jobRun.Trigger.Id);
            var job = _jobbrRepository.GetJob(jobRun.Job.Id);

            var info = new JobRunInfo();

            _mapper.Map(job, info);
            _mapper.Map(trigger, info);
            _mapper.Map(jobRun, info);

            return info;
        }
    }
}