using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Trigger state changed message.
    /// </summary>
    internal class TriggerStateChangedMessage : GenericTinyMessage<TriggerKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerStateChangedMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="content">The trigger information for the message.</param>
        public TriggerStateChangedMessage(object sender, TriggerKey content) : base(sender, content)
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