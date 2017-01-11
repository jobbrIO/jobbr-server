using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

using Jobbr.Server.Configuration;
using Jobbr.Server.Model;

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
        IJobStorageProvider JobStorageProvider { get; set; }

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

        bool IsRuntimeWaitingForDebugger { get; set; }

        /// <summary>
        /// Gets the artefact storage provider.
        /// </summary>
        IArtefactsStorageProvider ArtefactStorageProvider { get; }

        void OnRepositoryCreating(RepositoryBuilder repositoryBuilder);

        /// <summary>
        /// Callback to add custom parameters to jobrunner
        /// </summary>
        Func<string, string, IEnumerable<KeyValuePair<string, string>>> CustomJobRunnerParameters { get; set; }

        List<IJobbrComponent> Components { get; set; }
    }

    public interface IJobbrComponent : IDisposable
    {
        void Start();

        void Stop();
    }
}