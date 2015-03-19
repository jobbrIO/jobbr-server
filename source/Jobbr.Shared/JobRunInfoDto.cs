using System;

namespace Jobbr.Shared
{
    /// <summary>
    /// The job run configuration.
    /// </summary>
    public class JobRunInfoDto
    {
        public long JobId { get; set; }

        public long TriggerId { get; set; }

        public long JobRunId { get; set; }

        public Guid UniqueId { get; set; }

        public object JobParameter { get; set; }

        public object InstanceParameter { get; set; }

        public string JobName { get; set; }

        public string JobType { get; set; }

        public string WorkingDir { get; set; }

        public string TempDir { get; set; }
    }
}
