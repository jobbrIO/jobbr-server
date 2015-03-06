using System;
using System.ComponentModel.DataAnnotations;

namespace Jobbr.Server.Model
{
    /// <summary>
    /// The job trigger base.
    /// </summary>
    public abstract class JobTriggerBase
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        [Key]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the job.
        /// </summary>
        public long JobId { get; set; }

        public string TriggerType { get; set; }

        public bool IsActive { get; set; }

        public string Parameters { get; set; }

        public string Comment { get; set; }

        public long? UserId { get; set; }

        public string UserName { get; set; }

        public string UserDisplayName { get; set; }
    }

    public class CronTrigger : JobTriggerBase
    {
        public DateTime? DateTimeUtc { get; set; }

        public string Definition { get; set; }
    }

    public class StartDateTimeUtcTrigger : JobTriggerBase
    {
        public DateTime DateTimeUtc { get; set; }
    }

    public class InstantTrigger : JobTriggerBase
    {
        public int DelayedMinutes { get; set; }
    }
}