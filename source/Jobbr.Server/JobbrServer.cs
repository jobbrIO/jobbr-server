using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Configuration;
using Jobbr.Server.Core;
using Jobbr.Server.Logging;

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
        private DefaultScheduler scheduler;

        /// <summary>
        /// The executor.
        /// </summary>
        private IJobExecutor executor;

        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public JobbrServer(IJobbrConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            Logger.Debug("A new instance of a a JobbrServer has been created.");

            this.configuration = configuration;
        }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        public JobbrState State { get; private set; }

        public bool Start(int waitForStartupTimeout = 2000)
        {
            var startupTask = this.StartAsync(CancellationToken.None);

            try
            {
                startupTask.Wait(waitForStartupTimeout);
            }
            catch (AggregateException e)
            {
                throw e.InnerExceptions[0];
            }

            if (!startupTask.IsCompleted)
            {
                Logger.FatalFormat("Jobbr was unable to start within {0}ms. Keep starting but returning now from Start()", waitForStartupTimeout);
                return false;
            }

            return true;
        }

        /// <summary>
        /// The start.
        /// </summary>
        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            this.LogConfiguration();

            this.State = JobbrState.Initializing;
            this.ValidateConfigurationAndThrowOnErrors();
            this.ResolveServicesAndThrowOnErrors();

            this.State = JobbrState.Starting;

            var waitForDbTask = new Task(this.WaitForDb, cancellationToken);

            var startComponents = waitForDbTask.ContinueWith(
                t =>
                    {
                        this.RegisterJobsFromRepository();
                        this.SetScheduledJobsFromPastToOmitted();
                        this.StartInternalComponents();
                        this.StartOptionalComponents();

                        this.isRunning = true;
                    },
                cancellationToken);

            waitForDbTask.Start();

            await Task.WhenAll(waitForDbTask, startComponents);

            this.State = JobbrState.Running;

            return true;
        }

        private void StartInternalComponents()
        {
            try
            {
                Logger.DebugFormat("Starting Scheduler ({0})...", this.scheduler.GetType().Name);
                this.scheduler.Start();

                Logger.DebugFormat("Starting Executor ({0})...", this.executor.GetType().Name);
                this.executor.Start();

                Logger.Info("All services (Scheduler, Executor) have been started sucessfully.");

            }
            catch (Exception e)
            {
                Logger.FatalException("A least one service couldn't be started. Please see the exception for details.", e);
            }
        }

        private void StartOptionalComponents()
        {
            if (this.configuration.Components == null)
            {
                return;
            }

            try
            {
                foreach (var jobbrComponent in this.configuration.Components)
                {
                    var type = jobbrComponent.GetType();
                    Logger.DebugFormat($"Starting JobbrComponent '{type.FullName}' ...");
                    jobbrComponent.Start();
                }

                Logger.Info("All Optional Services have been started sucessfully.");

                this.isRunning = true;
            }
            catch (Exception e)
            {
                Logger.FatalException("A least one service couldn't be started. Please see the exception for details.", e);
            }
        }

        private void WaitForDb()
        {
            while (true)
            {
                try
                {
                    this.configuration.JobStorageProvider.GetJobs();
                    return;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void SetScheduledJobsFromPastToOmitted()
        {
            var scheduledJobs = this.configuration.JobStorageProvider.GetJobRunsByState(JobRunState.Scheduled).Where(p => p.PlannedStartDateTimeUtc < DateTime.UtcNow);

            foreach (var job in scheduledJobs)
            {
                job.State = JobRunState.Omitted;
                this.configuration.JobStorageProvider.Update(job);
            }
        }

        private void RegisterJobsFromRepository()
        {
            try
            {
                Logger.Info("Registering Jobs from configuration");

                var model = new RepositoryBuilder();

                this.configuration.OnRepositoryCreating(model);
                var numberOfChanges = model.Apply(this.configuration.JobStorageProvider);
                var numberOfJobs = this.configuration.JobStorageProvider.GetJobs().Count;

                Logger.InfoFormat("There were {0} changes for the JobRegistry which contains {1} jobs right now.", numberOfChanges, numberOfJobs);
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot register Jobs on startup. See Execption for details", e);
            }
        }

        private void LogConfiguration()
        {
            Logger.InfoFormat(
                "JobbrServer is now starting with the following configuration: BackendUrl='{0}', MaxConcurrentJobs='{1}', JobRunDirectory='{2}', JobRunnerExeutable='{3}', JobStorageProvider='{4}' ArtefactsStorageProvider='{5}'",
                this.configuration.BackendAddress,
                this.configuration.MaxConcurrentJobs,
                this.configuration.JobRunDirectory,
                this.configuration.JobRunnerExeResolver != null ? Path.GetFullPath(this.configuration.JobRunnerExeResolver()) : "none",
                this.configuration.JobStorageProvider,
                this.configuration.ArtefactStorageProvider);
        }

        private void ResolveServicesAndThrowOnErrors()
        {
            Logger.Debug("Creating DI-Container.");
            var kernel = new DefaultKernel(this.configuration);

            Logger.Debug("Resolving Services...");

            try
            {
                this.scheduler = kernel.GetService<DefaultScheduler>();
                this.executor = kernel.GetService<IJobExecutor>();

                if (this.scheduler == null)
                {
                    throw new NullReferenceException("'Schheduler' is not set!");
                }

                if (this.executor == null)
                {
                    throw new NullReferenceException("'Executor' is not set!");
                }

                Logger.Debug("Done resolving services");
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot resolve components. See the exception for details.", e);
                throw;
            }
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            Logger.Info("Attempt to shut down JobbrServer...");

            this.configuration.Components.ForEach(component => component.Stop());

            this.scheduler.Stop();
            this.executor.Stop();

            Logger.Info("All components stopped.");
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (this.isRunning)
            {
                this.Stop();
            }
        }

        private void ValidateConfigurationAndThrowOnErrors()
        {
            Logger.Debug("Validating the configuration...");

            try
            {
                if (this.configuration.JobRunnerExeResolver == null)
                {
                    throw new ArgumentException("You should set a runner-Executable which runs your jobs later!");
                }

                /*
                var executableFullPath = Path.GetFullPath(this.configuration.JobRunnerExeResolver());

                if (!File.Exists(executableFullPath))
                {
                    throw new ArgumentException(string.Format("The RunnerExecutable '{0}' cannot be found!", executableFullPath));
                }
                */

                if (string.IsNullOrEmpty(this.configuration.BackendAddress))
                {
                    throw new ArgumentException("Please provide a backend address to host the api!");
                }

                if (string.IsNullOrEmpty(this.configuration.JobRunDirectory))
                {
                    throw new ArgumentException("Please provide a JobRunDirectory!");
                }

                if (this.configuration.JobStorageProvider == null)
                {
                    throw new ArgumentException("Please provide a storage provider for Jobs!");
                }

                if (this.configuration.ArtefactStorageProvider == null)
                {
                    throw new ArgumentException("Please provide a storage provider for artefacts!");
                }

                Logger.Info("The configuration was validated and seems ok.");
            }
            catch (Exception)
            {
                this.State = JobbrState.Error;
                throw;
            }

        }
    }

    public enum JobbrState
    {
        Unknown = 0,
        Initializing = 11,
        Validating = 12,
        Starting = 13,
        Running = 14,

        Stopping = 21,
        Stopped = 29,
        Error = 99,


    }
}
