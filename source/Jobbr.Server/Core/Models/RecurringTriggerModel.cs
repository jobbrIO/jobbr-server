using System;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Model for recurring trigger.
    /// </summary>
    public class RecurringTriggerModel : TriggerModelBase
    {
        /// <summary>
        /// Trigger definition.
        /// </summary>
        public string Definition { get; set; }

        /// <summary>
        /// Trigger start time.
        /// </summary>
        public DateTime? StartDateTimeUtc { get; set; }

        /// <summary>
        /// Trigger end time.
        /// </summary>
        public DateTime? EndDateTimeUtc { get; set; }

        /// <summary>
        /// If parallel execution is allowed.
        /// </summary>
        public bool NoParallelExecution { get; set; }
    }
}