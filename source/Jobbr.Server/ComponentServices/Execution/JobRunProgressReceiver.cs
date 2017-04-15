using System.IO;
using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Core;

namespace Jobbr.Server.ComponentServices.Execution
{
    internal class JobRunProgressReceiver : IJobRunProgressChannel
    {
        private readonly JobRunService jobRunService;
        private readonly IMapper mapper;

        public JobRunProgressReceiver(JobRunService jobRunService, IMapper mapper)
        {
            this.jobRunService = jobRunService;
            this.mapper = mapper;
        }

        public void PublishStatusUpdate(long jobRunId, JobRunStates state)
        {
            var coreState = this.mapper.Map<Core.Models.JobRunStates>(state);

            this.jobRunService.UpdateState(jobRunId, coreState);
        }

        public void PublishProgressUpdate(long jobRunId, double progress)
        {
            this.jobRunService.UpdateProgress(jobRunId, progress);
        }

        public void PublishPid(long jobRunId, int pid, string host)
        {
            this.jobRunService.UpdatePid(jobRunId, host, pid);
        }

        public void PublishArtefact(long id, string fileName, Stream result)
        {
            this.jobRunService.AddArtefact(id, fileName, result);
        }
    }
}