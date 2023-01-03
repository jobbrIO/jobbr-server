using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    internal class TriggerStateChangedMessage : GenericTinyMessage<TriggerKey>
    {
        public TriggerStateChangedMessage(object sender, TriggerKey content)
            : base(sender, content)
        {
        }

        public long JobId => Content.JobId;

        public long TriggerId => Content.TriggerId;
    }
}