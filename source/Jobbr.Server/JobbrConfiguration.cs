using System;

namespace Jobbr.Server
{
    public class JobbrConfiguration : IJobbrConfiguration
    {
        public IJobQueueProvider JobQueueProvider { get; set; }

        public IJobRepositoryProvider JobRepositoryProvider { get; set; }

        public Func<string> JobRunnerExeResolver { get; set; }

        public string BackendAddress { get; set; }

        public JobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
        }
    }
}