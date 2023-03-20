using System;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Model class for jobs.
    /// </summary>
    public class JobModel
    {
        /// <summary>
        /// Job ID.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Unique name for the job.
        /// </summary>
        public string UniqueName { get; set; }

        /// <summary>
        /// Title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Parameters.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Type.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The last time the job information was updated in UTC.
        /// </summary>
        public DateTime? UpdatedDateTimeUtc { get; set; }

        /// <summary>
        /// Creation time in UTC.
        /// </summary>
        public DateTime? CreatedDateTimeUtc { get; set; }

        /// <summary>
        /// If job has been deleted.
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Maximum amount of concurrent job runs allowed.
        /// </summary>
        public int MaxConcurrentJobRuns { get; set; }
    }
}