using System;
using System.IO;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.Server.Core;

namespace Jobbr.Server.ComponentServices.Execution
{
    internal class JobRunProgressReceiver : IJobRunProgressChannel
    {
        private readonly JobRunService jobRunService;

        public JobRunProgressReceiver(JobRunService jobRunService)
        {
            this.jobRunService = jobRunService;
        }

        public void PublishStatusUpdate(long jobRunId, JobRunStates state)
        {
            // TODO: Enum-Mapping needed and pass eveything to the service?
            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                this.jobRunService.Done(jobRunId, state == JobRunStates.Completed);
            }

            // TODO: More generic publish for all the other states
            // TODO: this.stateService.SetJobRunStartTime(jobRun, DateTime.UtcNow);
        }

        public void PublishProgressUpdate(long jobRunId, double progress)
        {
            this.jobRunService.UpdateProgress(jobRunId, progress);
        }

        public void PublishArtefact(long id, string fileName, Stream result)
        {
            throw new NotImplementedException();
        }
    }
}