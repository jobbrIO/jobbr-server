using System;
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

        public void Done(Guid uniqueId, bool isSuccessful)
        {
            this.messengerHub.Publish(new JobRunCompletedMessage(this) { UniqueId = uniqueId, IsSuccessful = isSuccessful });
        }
    }

    internal class JobRunCompletedMessage : TinyMessageBase
    {
        public JobRunCompletedMessage(object sender) : base(sender)
        {
        }

        public Guid UniqueId { get; set; }

        public bool IsSuccessful { get; set; }
    }
}