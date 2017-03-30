namespace Jobbr.Server.Core.Messaging
{
    internal class TriggerKey
    {
        public long JobId { get; set; }

        public long TriggerId { get; set; }
    }
}