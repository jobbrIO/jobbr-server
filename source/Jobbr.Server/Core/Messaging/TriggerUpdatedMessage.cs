using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Trigger updated message.
    /// </summary>
    internal class TriggerUpdatedMessage : GenericTinyMessage<TriggerKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerUpdatedMessage"/> class.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="content">The trigger information for the message.</param>
        public TriggerUpdatedMessage(object sender, TriggerKey content) : base(sender, content)
        {
        }

        /// <summary>
        /// Job ID.
        /// </summary>
        public long JobId => Content.JobId;

        /// <summary>
        /// Trigger ID.
        /// </summary>
        public long TriggerId => Content.TriggerId;
    }
}