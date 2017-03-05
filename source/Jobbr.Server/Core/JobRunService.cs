using System;
using AutoMapper;
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
        }

        public void Done(long id, bool isSuccessful)
        {
            this.messengerHub.Publish(new JobRunCompletedMessage(this) { Id = id, IsSuccessful = isSuccessful });
        }
    }

    internal class JobRunCompletedMessage : TinyMessageBase
    {
        public JobRunCompletedMessage(object sender) : base(sender)
        {
        }

        public long Id { get; set; }

        public bool IsSuccessful { get; set; }
    }
}