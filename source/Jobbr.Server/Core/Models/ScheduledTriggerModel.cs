using System;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Model for a scheduled trigger.
    /// </summary>
    public class ScheduledTriggerModel : TriggerModelBase
    {
        /// <summary>
        /// Start time.
        /// </summary>
        public DateTime StartDateTimeUtc { get; set; }
    }
}