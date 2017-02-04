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

        public long? UserId { get; set; }

        public string UserName { get; set; }

        public long Id { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

    }
}