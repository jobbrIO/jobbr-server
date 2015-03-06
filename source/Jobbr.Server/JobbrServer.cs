using System;
using System.IO;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Web;

using Microsoft.Owin.Hosting.Services;

namespace Jobbr.Server
{
    /// <summary>
    /// The jobber job server.
    /// </summary>
    public class JobbrServer : IDisposable
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IJobbrConfiguration configuration;

        /// <summary>
        /// The scheduler.
        /// </summary>
        private readonly DefaultScheduler scheduler;

        /// <summary>
        /// The executor.
        /// </summary>
        private readonly IJobExecutor executor;

        /// <summary>
        /// The web host.
        /// </summary>
        private readonly WebHost webHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public JobbrServer(IJobbrConfiguration configuration)
        {
            this.configuration = configuration;
            var kernel = new DefaultKernel(this.configuration);

            this.webHost = kernel.GetService<WebHost>();
            this.scheduler = kernel.GetService<DefaultScheduler>();
            this.executor = kernel.GetService<IJobExecutor>();
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            this.ValidateConfiguration();

            this.webHost.Start();
            this.scheduler.Start();
            this.executor.Start();
        }

        private void ValidateConfiguration()
        {
            var executableFullPath = Path.GetFullPath(this.configuration.JobRunnerExeResolver());

            if (!File.Exists(executableFullPath))
            {
                throw new Exception(string.Format("The RunnerExecutable '{0}' cannot be found!", executableFullPath));
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.webHost.Stop();
            this.scheduler.Stop();
            this.executor.Stop();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Stop();
        }
    }
}
