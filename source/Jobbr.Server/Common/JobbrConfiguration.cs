using System;

namespace Jobbr.Server.Common
{
    public class JobbrConfiguration : IJobbrConfiguration
    {
        public IJobbrStorageProvider StorageProvider { get; set; }

        public Func<string> JobRunnerExeResolver { get; set; }

        public string BackendAddress { get; set; }

        public JobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
        }
    }
}