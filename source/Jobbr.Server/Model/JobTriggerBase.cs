using System;
using System.ComponentModel.DataAnnotations;

namespace Jobbr.Server.Model
{
    /// <summary>
    /// The job trigger base.
    /// </summary>
    [Serializable]
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

        public DateTime CreatedDateTimeUtc { get; set; }
    }

    [Serializable]
    public class RecurringTrigger : JobTriggerBase
    {
        public const string TypeName = "Recurring";

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public string Definition { get; set; }

        public bool NoParallelExecution { get; set; }
    }

    [Serializable]
    public class ScheduledTrigger : JobTriggerBase
    {
        public const string TypeName = "Scheduled";

        public DateTime StartDateTimeUtc { get; set; }
    }

    [Serializable]
    public class InstantTrigger : JobTriggerBase
    {
        public const string TypeName = "Instant";

        public int DelayedMinutes { get; set; }
    }
}