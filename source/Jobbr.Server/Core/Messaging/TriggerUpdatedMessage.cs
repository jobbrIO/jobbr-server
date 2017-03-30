using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class TriggerUpdatedMessage : GenericTinyMessage<TriggerKey>
    {
        public TriggerUpdatedMessage(object sender, TriggerKey content)
            : base(sender, content)
        {
        }

        public long JobId => this.Content.JobId;

        public long TriggerId => this.Content.TriggerId;
    }
}