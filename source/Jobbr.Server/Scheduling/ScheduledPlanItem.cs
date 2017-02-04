using System;

namespace Jobbr.Server.Scheduling
{
    internal class ScheduledPlanItem
    {
        public Guid UniqueId { get; set; }

        public DateTime PlannedStartDateTimeUtc { get; set; }

        public long TriggerId { get; set; }
    }
}