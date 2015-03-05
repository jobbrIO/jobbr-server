namespace Jobbr.Server
{
    using System;

    /// <summary>
    /// The JobbrConfiguration interface.
    /// </summary>
    public interface IJobbrConfiguration
    {
        /// <summary>
        /// Gets or sets the job repository provider.
        /// </summary>
        IJobRepositoryProvider JobRepositoryProvider { get; set; }

        /// <summary>
        /// Gets or sets the job runner exe resolver.
        /// </summary>
        Func<string> JobRunnerExeResolver { get; set; }

        /// <summary>
        /// Gets or sets the backend address.
        /// </summary>
        string BackendAddress { get; set; }
    }
}