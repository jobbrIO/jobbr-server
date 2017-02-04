using System;

namespace Jobbr.Server.Core.Models
{
    internal class ScheduledTriggerModel : TriggerModelBase
    {
        public DateTime StartDateTimeUtc { get; set; }
    }
}