using Jobbr.Server.Common;
using Jobbr.Server.Configuration;
using Jobbr.Tests.StorageProvider;

namespace Jobbr.Tests
{
    public class CompleteJobberConfiguration : EmptyJobbrConfiguration
    {
        public CompleteJobberConfiguration()
        {
            this.JobRunnerExeResolver = () => ".";
            this.JobStorageProvider = new InMemoryJobStorageProvider();
            this.ArtefactStorageProvider = new FileSystemArtefactsStorageProvider("test");
            this.BackendAddress = "http://localhost:49654/jobbr";
            this.JobRunDirectory = "run";
            this.MaxConcurrentJobs = 10;
        }
    }
}