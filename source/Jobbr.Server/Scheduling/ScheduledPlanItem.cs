using System;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Scheduled plan item.
    /// </summary>
    internal class ScheduledPlanItem
    {
        /// <summary>
        /// Item ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Planned start timestamp.
        /// </summary>
        public DateTime PlannedStartDateTimeUtc { get; set; }

        /// <summary>
        /// Trigger ID.
        /// </summary>
        public long TriggerId { get; set; }

        /// <summary>
        /// Job ID.
        /// </summary>
        public long JobId { get; set; }
    }
}