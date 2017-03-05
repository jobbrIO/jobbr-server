using System;
using AutoMapper;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    public class JobRunService
    {
        private static readonly ILog Logger = LogProvider.For<JobRunService>();

        private readonly ITinyMessengerHub messengerHub;
        private readonly IJobbrRepository repository;
        private readonly IMapper mapper;

        public JobRunService(ITinyMessengerHub messengerHub, IJobbrRepository repository, IMapper mapper)
        {
            this.messengerHub = messengerHub;
            this.repository = repository;
            this.mapper = mapper;
        }

        public void UpdateProgress(long jobRunId, double progress)
        {
            this.repository.UpdateJobRunProgress(jobRunId, progress);
        }

        public void UpdateState(long jobRunId, JobRunStates state)
        {
            Logger.InfoFormat("[{0}] The JobRun with id: {0} has switched to the '{1}'-State", jobRunId, state);

            var jobRun = this.repository.GetJobRunById(jobRunId);
            jobRun.State = this.mapper.Map<ComponentModel.JobStorage.Model.JobRunStates>(state);

            if (state == JobRunStates.Started)
            {
                jobRun.ActualStartDateTimeUtc = DateTime.UtcNow;
            }

            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                jobRun.ActualEndDateTimeUtc = DateTime.UtcNow;
            }

            this.repository.Update(jobRun);

            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                this.messengerHub.Publish(new JobRunCompletedMessage(this) { Id = jobRunId, IsSuccessful = state == JobRunStates.Completed });
            }
        }
    }
}