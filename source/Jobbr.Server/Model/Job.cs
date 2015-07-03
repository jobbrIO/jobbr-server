using System;
using System.ComponentModel.DataAnnotations;

namespace Jobbr.Server.Model
{
    [Serializable]
    public class Job
    {
        [Key]
        public long Id { get; set; }

        public string UniqueName { get; set; }

        public string Title { get; set; }

        public string Type { get; set; }

        public string Parameters { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public DateTime? UpdatedDateTimeUtc { get; set; }
    }
}