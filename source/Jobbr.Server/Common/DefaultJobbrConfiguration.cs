using System;
using System.IO;

namespace Jobbr.Server.Common
{
    public class DefaultJobbrConfiguration : IJobbrConfiguration
    {
        public IJobbrStorageProvider StorageProvider { get; set; }

        public Func<string> JobRunnerExeResolver { get; set; }

        public string BackendAddress { get; set; }

        public int AllowChangesBeforeStartInSec { get; set; }

        public int MaxConcurrentJobs { get; set; }

        public string JobRunDirectory  { get; set; }

        public DefaultJobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
            this.MaxConcurrentJobs = 4;
            this.JobRunDirectory = Path.GetTempPath();
        }
    }
}