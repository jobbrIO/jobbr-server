using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class TriggerStateChangedMessage : GenericTinyMessage<long>
    {
        public TriggerStateChangedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }
}