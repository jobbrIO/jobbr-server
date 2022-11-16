using System.IO;
using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Core;

namespace Jobbr.Server.ComponentServices.Execution
{
    /// <summary>
    /// Receives events regarding job runs and sends them to service classes.
    /// </summary>
    internal class JobRunProgressReceiver : IJobRunProgressChannel
    {
        private readonly JobRunService _jobRunService;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunProgressReceiver"/> class.
        /// </summary>
        /// <param name="jobRunService">Job run service.</param>
        /// <param name="mapper">The mapper.</param>
        public JobRunProgressReceiver(JobRunService jobRunService, IMapper mapper)
        {
            _jobRunService = jobRunService;
            _mapper = mapper;
        }

        /// <summary>
        /// Publishes a status update for the job run.
        /// </summary>
        /// <param name="jobRunId">The ID of the job run.</param>
        /// <param name="state">The state for the update.</param>
        public void PublishStatusUpdate(long jobRunId, JobRunStates state)
        {
            var coreState = _mapper.Map<Core.Models.JobRunStates>(state);

            _jobRunService.UpdateState(jobRunId, coreState);
        }

        /// <summary>
        /// Publishes a progress update for the job run.
        /// </summary>
        /// <param name="jobRunId">The ID for the job run.</param>
        /// <param name="progress">The progress that is being published.</param>
        public void PublishProgressUpdate(long jobRunId, double progress)
        {
            _jobRunService.UpdateProgress(jobRunId, progress);
        }

        /// <summary>
        /// Publishes a process ID for the job run.
        /// </summary>
        /// <param name="jobRunId">The ID of the job run.</param>
        /// <param name="pid">The process ID.</param>
        /// <param name="host">The host.</param>
        public void PublishPid(long jobRunId, int pid, string host)
        {
            _jobRunService.UpdatePid(jobRunId, pid);
        }

        /// <summary>
        /// Publishes an artifact for the job run.
        /// </summary>
        /// <param name="id">The ID for the job run.</param>
        /// <param name="fileName">Artifact filename.</param>
        /// <param name="result">Result <see cref="Stream"/>.</param>
        public void PublishArtefact(long id, string fileName, Stream result)
        {
            _jobRunService.AddArtefact(id, fileName, result);
        }
    }
}