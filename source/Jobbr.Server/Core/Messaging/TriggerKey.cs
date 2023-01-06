namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Trigger key for messaging.
    /// </summary>
    internal class TriggerKey
    {
        /// <summary>
        /// Job ID.
        /// </summary>
        public long JobId { get; set; }

        /// <summary>
        /// Trigger ID.
        /// </summary>
        public long TriggerId { get; set; }
    }
}