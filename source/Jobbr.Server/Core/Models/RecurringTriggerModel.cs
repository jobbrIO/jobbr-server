using System;

namespace Jobbr.Server.Core.Models
{
    internal class RecurringTriggerModel : TriggerModelBase
    {
        public string Definition { get; set; }

        public DateTime? StartDateTimeUtc { get; set; }

        public DateTime? EndDateTimeUtc { get; set; }

        public bool NoParallelExecution { get; set; }
    }
}