using TinyMessenger;

namespace Jobbr.Server.Core
{
    internal class JobRunService
    {
        private readonly ITinyMessengerHub messengerHub;

        public JobRunService(ITinyMessengerHub messengerHub)
        {
            this.messengerHub = messengerHub;
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