using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// TinyMessenger message class that is used when a job trigger is added.
    /// </summary>
    internal class TriggerAddedMessage : GenericTinyMessage<TriggerKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerAddedMessage"/> class.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        /// <param name="content">Message content.</param>
        public TriggerAddedMessage(object sender, TriggerKey content) : base(sender, content)
        {
        }

        /// <summary>
        /// Job trigger ID.
        /// </summary>
        public long TriggerId => Content.TriggerId;

        /// <summary>
        /// Job ID.
        /// </summary>
        public long JobId => Content.JobId;
    }
}