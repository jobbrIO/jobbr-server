using System;

namespace Jobbr.Server
{
    public class JobbrConfiguration
    {
        public IJobQueueProvider JobQueueProvider;

        public IJobRepositoryProvider JobRepositoryProvider;

        public Func<string> JobRunnerExeResolver { get; set; }

        public string BackendAddress { get; set; }

        public JobbrConfiguration()
        {
            this.BackendAddress = "http://localhost:80/jobbr";
        }
    }
}