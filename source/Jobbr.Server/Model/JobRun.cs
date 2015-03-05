using System;
using System.ComponentModel.DataAnnotations;

namespace Jobbr.Server.Model
{
    /// <summary>
    /// The actual run of the job
    /// </summary>
    public class JobRun
    {
        [Key]
        public long Id { get; set; }

        public long JobId { get; set; }

        public long TriggerId { get; set; }

        public Guid Guid { get; set; }

        public int Progress { get; set; }

        public JobRunState State { get; set; }
    }
}