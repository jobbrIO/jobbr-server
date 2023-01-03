using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    internal class TriggerUpdatedMessage : GenericTinyMessage<TriggerKey>
    {
        public TriggerUpdatedMessage(object sender, TriggerKey content)
            : base(sender, content)
        {
        }

        public long JobId => Content.JobId;

        public long TriggerId => Content.TriggerId;
    }
}