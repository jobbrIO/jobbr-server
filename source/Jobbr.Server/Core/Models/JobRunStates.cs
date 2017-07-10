namespace Jobbr.Server.Core.Models
{
    public enum JobRunStates
    {
        /// <summary>
        /// Undefined State
        /// </summary>
        Null = 0,

        /// <summary>
        /// The JoBRun is queued unless a JobStarter starts a new executable
        /// </summary>
        Scheduled = 1,

        /// <summary>
        /// The JobStarter has created a enviornment for the Job and copies a related files/data to the working directory
        /// </summary>
        Preparing = 2,

        /// <summary>
        /// The JobStarted has started a new executable 
        /// </summary>
        Starting = 3,

        /// <summary>
        /// The JobStarted has created a new environment and the executable has been started
        /// </summary>
        Started = 4,

        /// <summary>
        /// The Executable itself has connected to the jobServer
        /// </summary>
        Connected = 5,

        /// <summary>
        /// The Executable is running and connected to the jobserver
        /// </summary>
        Initializing = 6,

        /// <summary>
        /// The logic has started to run
        /// </summary>
        Processing = 7,

        /// <summary>
        /// The external code was run
        /// </summary>
        Finishing = 8,

        /// <summary>
        /// Collecting the files
        /// </summary>
        Collecting = 9,

        /// <summary>
        /// The job as executed sucessfully and the executer has cleaned up and terminated
        /// </summary>
        Completed = 10,

        /// <summary>
        /// The job failed.
        /// </summary>
        Failed = 11,

        /// <summary>
        /// The JobRun has ben deleted in advance
        /// </summary>
        Deleted = 12,

        /// <summary>
        /// The JobRun has been omitted. Eg job has been scheduled, Jobserver stopped (before the jobrun is executed) and After PlannedStartDateTime started again -> JobRun won't be started in that case but set to Omitted.
        /// </summary>
        Omitted = 13
    }
}
