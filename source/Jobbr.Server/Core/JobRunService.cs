using Jobbr.Server.Storage;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    internal class JobRunService
    {
        private readonly ITinyMessengerHub messengerHub;
        private readonly IJobbrRepository repository;

        public JobRunService(ITinyMessengerHub messengerHub, IJobbrRepository repository)
        {
            this.messengerHub = messengerHub;
            this.repository = repository;
        }

        public void UpdateProgress(long jobRunId, double progress)
        {
            this.repository.UpdateJobRunProgress(jobRunId, progress);
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