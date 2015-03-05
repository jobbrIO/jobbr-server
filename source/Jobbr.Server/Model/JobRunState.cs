namespace Jobbr.Server.Model
{
    /// <summary>
    /// The job run state.
    /// </summary>
    public enum JobRunState
    {
        /// <summary>
        /// The JoBRun is queued unless a JobStarter starts a new executable
        /// </summary>
        Scheduled,

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