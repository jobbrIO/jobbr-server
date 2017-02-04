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

        public void PublishStatusUpdate(JobRunInfo jobRunInfo, JobRunStates state)
        {
            // TODO: Enum-Mapping needed and pass eveything to the service?
            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                this.jobRunService.Done(jobRunInfo.UniqueId, state == JobRunStates.Completed);
            }

            // TODO: More generic publish for all the other states
            //TODO: this.stateService.SetJobRunStartTime(jobRun, DateTime.UtcNow);
        }

        public void PublishProgressUpdate(JobRunInfo jobRunInfo, double progress)
        {
        }

        public void PublicArtefact(Guid uniqueId, string fileName, Stream result)
        {
            throw new NotImplementedException();
        }
    }
}