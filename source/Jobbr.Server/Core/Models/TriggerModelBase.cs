using System;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Base class for trigger models.
    /// </summary>
    public class TriggerModelBase
    {
        /// <summary>
        /// Comment.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The ID of the job.
        /// </summary>
        public long JobId { get; set; }

        /// <summary>
        /// Trigger parameters.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// If trigger is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// User display name.
        /// </summary>
        public string UserDisplayName { get; set; }

        /// <summary>
        /// User ID.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Trigger ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Creation time in UTC.
        /// </summary>
        public DateTime CreatedDateTimeUtc { get; set; }

        /// <summary>
        /// If trigger has been deleted.
        /// </summary>
        public bool Deleted { get; set; }
    }
}