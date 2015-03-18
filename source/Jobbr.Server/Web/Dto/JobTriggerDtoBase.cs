using System;

namespace Jobbr.Server.Web.Dto
{
    /// <summary>
    /// The job trigger base.
    /// </summary>
    public abstract class JobTriggerDtoBase
    {
        public long Id { get; set; }

        public string TriggerType { get; set; }

        public bool IsActive { get; set; }

        public object Parameters { get; set; }

        public string Comment { get; set; }

        public long? UserId { get; set; }

        public string UserName { get; set; }

        public string UserDisplayName { get; set; }
    }

    public class RecurringTriggerDto : JobTriggerDtoBase
    {
        public const string TypeName = "Recurring";

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public string Definition { get; set; }
    }

    public class ScheduledTriggerDto : JobTriggerDtoBase
    {
        public const string TypeName = "Scheduled";

        public DateTime StartDateTimeUtc { get; set; }
    }

    public class InstantTriggerDto : JobTriggerDtoBase
    {
        public const string TypeName = "Instant";

        public int DelayedMinutes { get; set; }
    }
}
