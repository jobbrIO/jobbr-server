using System;
using System.IO;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Logging;
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
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogProvider.For<JobbrServer>();

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

        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public JobbrServer(IJobbrConfiguration configuration)
        {
            Logger.Log(LogLevel.Debug, () => "A new instance of a a JobbrServer has been created.");

            this.configuration = configuration;

            Logger.Log(LogLevel.Debug, () => "Creating DI-Container.");
            var kernel = new DefaultKernel(this.configuration);

            Logger.Log(LogLevel.Debug, () => "Resolving Services...");

            try
            {
                this.webHost = kernel.GetService<WebHost>();
                this.scheduler = kernel.GetService<DefaultScheduler>();
                this.executor = kernel.GetService<IJobExecutor>();

                Logger.Log(LogLevel.Debug, () => "Done");
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot resolve components. See the exception for details.", e);
            }
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            Logger.InfoFormat(
                "JobbrServer is now starting with the following configuration: BackendUrl='{0}', MaxConcurrentJobs='{1}', JobRunDirectory='{2}', JobRunnerExeutable='{3}', JobStorageProvider='{4}' ArtefactsStorageProvider='{5}'", 
                this.configuration.BackendAddress,
                this.configuration.MaxConcurrentJobs,
                this.configuration.JobRunDirectory,
                this.configuration.JobRunnerExeResolver != null ? Path.GetFullPath(this.configuration.JobRunnerExeResolver()) : "none",
                this.configuration.JobStorageProvider, 
                this.configuration.ArtefactStorageProvider);

            try
            {
                Logger.Debug("Validating the configuration...");
                
                this.ValidateConfiguration();
                
                Logger.Info("The configuration was validated and seems ok.");
            }
            catch (Exception e)
            {
                Logger.ErrorException("A least one configuration setting has failed. Please see the exception for details.", e);

                return;
            }

            try
            {
                Logger.DebugFormat("Starting WebHost ({0})...", this.webHost.GetType().Name);
                this.webHost.Start();

                Logger.DebugFormat("Starting Scheduler ({0})...", this.scheduler.GetType().Name);
                this.scheduler.Start();

                Logger.DebugFormat("Starting Executor ({0})...", this.executor.GetType().Name);
                this.executor.Start();

                Logger.Info("All services (WebHost, Scheduler, Executor) have been started sucessfully.");
            }
            catch (Exception e)
            {
                Logger.FatalException("A least one service couldn't be started. Please see the exception for details.", e);
            }

            this.isRunning = true;
        }

        private void ValidateConfiguration()
        {
            var executableFullPath = Path.GetFullPath(this.configuration.JobRunnerExeResolver());

            if (!File.Exists(executableFullPath))
            {
                throw new Exception(string.Format("The RunnerExecutable '{0}' cannot be found!", executableFullPath));
            }

            if (this.configuration.JobStorageProvider == null)
            {
                throw new Exception("Please provide a storage provider for Jobs!");
            }

            if (this.configuration.ArtefactStorageProvider == null)
            {
                throw new Exception("Please provide a storage provider for artefacts!");
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            Logger.Info("Attempt to shut down JobbrServer...");

            this.webHost.Stop();
            this.scheduler.Stop();
            this.executor.Stop();

            Logger.Info("All services (WebHost, Scheduler, Executor) have been stopped.");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.isRunning)
                this.Stop();
        }
    }
}
