using System;
using System.IO;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;

namespace Jobbr.Server.Builder
{
    internal class JobRunProgressReceiver : IJobRunProgressChannel
    {
        public void PublishStatusUpdate(JobRunInfo jobRunInfo, JobRunStates state)
        {
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