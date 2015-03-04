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

    /// <summary>
    /// The job run state.
    /// </summary>
    public enum JobRunState
    {
        /// <summary>
        /// The JoBRun is queued unless a JobStarter starts a new executable
        /// </summary>
        Queued,

        /// <summary>
        /// The JobStarted has created a enviornment for the Job and copies a related files/data to the working directory
        /// </summary>
        Preparing,

        /// <summary>
        /// The JobStarted has created a new environment and the executable has been started
        /// </summary>
        Started,

        /// <summary>
        /// The Executable has started and connected to the jobserver
        /// </summary>
        Initializing,

        /// <summary>
        /// The logic has started to run
        /// </summary>
        Running,

        /// <summary>
        /// The external code was run
        /// </summary>
        Finishing,
        
        /// <summary>
        /// The job as executed sucessfully and the executer has cleaned up and terminated
        /// </summary>
        Completed,

        /// <summary>
        /// The job failed.
        /// </summary>
        Failed
    }
}