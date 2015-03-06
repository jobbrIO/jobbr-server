using System;

namespace Jobbr.Server.Common
{
    /// <summary>
    /// The JobbrConfiguration interface.
    /// </summary>
    public interface IJobbrConfiguration
    {
        /// <summary>
        /// Gets or sets the job repository provider.
        /// </summary>
        IJobbrStorageProvider StorageProvider { get; set; }

        /// <summary>
        /// Gets or sets the job runner exe resolver.
        /// </summary>
        Func<string> JobRunnerExeResolver { get; set; }

        /// <summary>
        /// Gets or sets the backend address.
        /// </summary>
        string BackendAddress { get; set; }

        /// <summary>
        /// Gets or sets the allow changes before start in sec.
        /// </summary>
        int AllowChangesBeforeStartInSec { get; set; }

        /// <summary>
        /// Gets the max concurrent jobs.
        /// </summary>
        int MaxConcurrentJobs { get; }

        string JobRunDirectory { get; }

        bool BeChatty { get; set; }
    }
}