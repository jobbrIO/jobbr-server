using System;

namespace Jobbr.Server.Scheduling
{
    internal class ScheduledPlanItem
    {
        public long Id { get; set; }

        public DateTime PlannedStartDateTimeUtc { get; set; }

        public long TriggerId { get; set; }

        public long JobId { get; set; }
    }
}