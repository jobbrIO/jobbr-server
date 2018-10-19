using System;

namespace Jobbr.Server.Core.Models
{
    internal class TriggerModelBase
    {
        public string Comment { get; set; }

        public long JobId { get; set; }

        public string Parameters { get; set; }

        public bool IsActive { get; set; }

        public string UserDisplayName { get; set; }

        public string UserId { get; set; }

        public long Id { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public bool Deleted { get; set; }
    }
}