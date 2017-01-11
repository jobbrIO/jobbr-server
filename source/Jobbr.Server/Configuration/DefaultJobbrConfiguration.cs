using System.IO;

namespace Jobbr.Server.Configuration
{
    public class DefaultJobbrConfiguration : EmptyJobbrConfiguration
    {
        public DefaultJobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
            this.MaxConcurrentJobs = 4;
            this.JobRunDirectory = Path.GetTempPath();
        }
    }
}