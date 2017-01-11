using System;
using System.Collections.Generic;

using Jobbr.Server.Common;

namespace Jobbr.Server.Configuration
{
    public abstract class EmptyJobbrConfiguration : IJobbrConfiguration
    {
        public IJobStorageProvider JobStorageProvider { get; set; }

        public Func<string> JobRunnerExeResolver { get; set; }

        public string BackendAddress { get; set; }

        public int AllowChangesBeforeStartInSec { get; set; }

        public int MaxConcurrentJobs { get; set; }

        public string JobRunDirectory { get; set; }

        public bool IsRuntimeWaitingForDebugger { get; set; }

        public IArtefactsStorageProvider ArtefactStorageProvider { get; set; }

        public virtual void OnRepositoryCreating(RepositoryBuilder repositoryBuilder)
        {
        }

        public Func<string, string, IEnumerable<KeyValuePair<string, string>>> CustomJobRunnerParameters { get; set; }

        public List<IJobbrComponent> Components { get; set; }
    }
}