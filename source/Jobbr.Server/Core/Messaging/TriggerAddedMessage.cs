using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    internal class TriggerAddedMessage : GenericTinyMessage<long>
    {
        public TriggerAddedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }
}