using System;
using System.Threading;

using Jobbr.Server.Common;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Responsible for Starting extenal processes with the job inside
    /// </summary>
    public class JobProcessStarter : IJobStarter
    {
        private IJobService jobService;

        private IJobbrConfiguration configuration;

        private Timer timer;

        public JobProcessStarter(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;

            this.timer = new Timer(this.Callback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private void Callback(object state)
        {
            
        }

        public void Start()
        {
            this.timer.Change(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(60));
        }

        public void Stop()
        {

        }
    }
}
