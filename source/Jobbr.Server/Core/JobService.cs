using System;

namespace Jobbr.Server.Core
{
    public class JobService
    {
        public JobModel Add(JobModel model)
        {
            return null;
        }
    }

    public class JobModel
    {
        public long Id { get; set; }
        public string UniqueName { get; set; }
        public string Title { get; set; }
        public string Parameters { get; set; }
        public string Type { get; set; }
        public DateTime? UpdatedDateTimeUtc { get; set; }
        public DateTime? CreatedDateTimeUtc { get; set; }

    }
}