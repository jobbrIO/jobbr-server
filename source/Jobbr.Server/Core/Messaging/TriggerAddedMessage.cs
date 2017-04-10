using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    internal class TriggerAddedMessage : GenericTinyMessage<TriggerKey>
    {
        public TriggerAddedMessage(object sender, TriggerKey content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content.TriggerId;

        public long JobId => this.Content.JobId;
    }
}