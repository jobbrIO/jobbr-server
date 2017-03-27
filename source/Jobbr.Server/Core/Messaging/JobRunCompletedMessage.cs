using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class JobRunCompletedMessage : TinyMessageBase
    {
        public JobRunCompletedMessage(object sender)
            : base(sender)
        {
        }

        public long Id { get; set; }

        public bool IsSuccessful { get; set; }
    }
}