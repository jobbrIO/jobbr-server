using System.IO;

using Jobbr.Server.Common;

namespace Jobbr.Server.Configuration
{
    public class DefaultJobbrConfiguration : EmptyJobbrConfiguration, IJobbrConfiguration
    {
        public DefaultJobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
            this.MaxConcurrentJobs = 4;
            this.JobRunDirectory = Path.GetTempPath();
        }
    }
}