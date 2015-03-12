using System;

namespace Jobbr.Server.Web.Dto
{
    public class JobDto
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public object Parameters { get; set; }

        public DateTime? CreatedDateTimeUtc { get; set; }

        public DateTime? UpdatedDateTimeUtc { get; set; }
    }

}
