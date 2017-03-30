using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class TriggerStateChangedMessage : GenericTinyMessage<TriggerKey>
    {
        public TriggerStateChangedMessage(object sender, TriggerKey content)
            : base(sender, content)
        {
        }

        public long JobId => this.Content.JobId;

        public long TriggerId => this.Content.TriggerId;
    }
}