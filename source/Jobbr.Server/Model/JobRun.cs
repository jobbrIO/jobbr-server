using System;
using System.ComponentModel.DataAnnotations;
using Jobbr.Common.Model;

namespace Jobbr.Server.Model
{
    /// <summary>
    /// The actual run of the job
    /// </summary>
    [Serializable]
    public class JobRun
    {
        [Key]
        public long Id { get; set; }

        public long JobId { get; set; }

        public long TriggerId { get; set; }

        public string UniqueId { get; set; }

        public string JobParameters { get; set; }

        public string InstanceParameters { get; set; }

        public double? Progress { get; set; }

        public JobRunState State { get; set; }

        public bool IsFinished
        {
            get { return this.State == JobRunState.Failed || this.State == JobRunState.Completed || this.State == JobRunState.Deleted || this.State == JobRunState.Omitted; }
        }

        public DateTime PlannedStartDateTimeUtc { get; set; }

        public DateTime? ActualStartDateTimeUtc { get; set; }

        public DateTime? EstimatedEndDateTimeUtc { get; set; }

        public DateTime? ActualEndDateTimeUtc { get; set; }

        public int Pid { get; set; }

        public string WorkingDir { get; set; }

        public string TempDir { get; set; }
    }
}