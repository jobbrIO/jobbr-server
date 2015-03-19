using System;
using System.Collections.Generic;

namespace Jobbr.Server.Web.Dto
{
    public class JobDto
    {
        public long Id { get; set; }

        public string UniqueName { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public object Parameters { get; set; }

        public DateTime? CreatedDateTimeUtc { get; set; }

        public DateTime? UpdatedDateTimeUtc { get; set; }

        public List<JobTriggerDtoBase> Trigger { get; set; } 
    }
}
