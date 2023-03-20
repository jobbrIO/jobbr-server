using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Message for a completed job run.
    /// </summary>
    public class JobRunCompletedMessage : TinyMessageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunCompletedMessage"/> class.
        /// </summary>
        /// <param name="sender">Message sender.</param>
        public JobRunCompletedMessage(object sender) : base(sender)
        {
        }

        /// <summary>
        /// Job run ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// If job run was successful.
        /// </summary>
        public bool IsSuccessful { get; set; }
    }
}