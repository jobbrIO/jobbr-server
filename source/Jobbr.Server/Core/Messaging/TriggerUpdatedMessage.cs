using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class TriggerUpdatedMessage : GenericTinyMessage<long>
    {
        public TriggerUpdatedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }
}