using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Extensions;
using Jobbr.Server.JobRegistry;
using Jobbr.Server.Logging;
using Jobbr.Server.Scheduling;

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
        /// The scheduler.
        /// </summary>
        private readonly IJobScheduler scheduler;

        /// <summary>
        /// The executor.
        /// </summary>
        private readonly IJobExecutor executor;

        private readonly IJobStorageProvider jobStorageProvider;

        private readonly List<IJobbrComponent> components;

        private readonly ConfigurationManager configurationManager;
        private readonly RegistryBuilder registryBuilder;

        private bool isRunning;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        public JobbrServer(IJobScheduler scheduler, IJobExecutor jobExecutor, IJobStorageProvider jobStorageProvider, List<IJobbrComponent> components, MessageDispatcher messageDispatcher, ConfigurationManager configurationManager, RegistryBuilder registryBuilder)
        {
            Logger.Debug("A new instance of a a JobbrServer has been created.");

            this.scheduler = scheduler;
            this.executor = jobExecutor;

            this.components = components;
            this.configurationManager = configurationManager;
            this.registryBuilder = registryBuilder;
            this.jobStorageProvider = jobStorageProvider;

            messageDispatcher.WireUp();
        }

        public bool IsRunning => this.isRunning;

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
                this.State = JobbrState.Error;
                throw e.InnerExceptions[0];
            }

            if (!startupTask.IsCompleted)
            {
                Logger.FatalFormat("Jobbr was unable to start within {0}ms. Keep starting but returning now from Start()", waitForStartupTimeout);
                return false;
            }

            return true;
        }

        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            this.State = JobbrState.Initializing;

            this.LogVersions();

            Logger.InfoFormat("The JobServer has been set-up by the following configurations");
            this.configurationManager.LogConfiguration();

            this.State = JobbrState.Validating;

            try
            {
                this.configurationManager.ValidateConfigurationAndThrowOnErrors();
                Logger.Info("The configuration was validated and seems ok. Final configuration below:");

                this.configurationManager.LogConfiguration();
            }
            catch (Exception)
            {
                this.State = JobbrState.Error;
                throw;
            }

            this.State = JobbrState.Starting;

            var waitForDbTask = new Task(this.WaitForDb, cancellationToken);

            var startComponents = waitForDbTask.ContinueWith(
                t =>
                    {
                        this.RegisterJobsFromRepository();
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

        private void LogVersions()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Version Information:");

            var assemblies = new List<Assembly> { this.GetType().Assembly };
            assemblies.AddRange(this.components.Select(component => component.GetType().Assembly));

            assemblies.Distinct().ToList().ForEach(assembly => { sb.AppendLine($"{assembly.ManifestModule.Name} {assembly.GetVersion()}"); });

            Logger.Info(sb.ToString().TrimEnd());
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            Logger.Info("Attempt to shut down JobbrServer...");

            this.components.ForEach(component => component.Stop());

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
            if (this.components == null)
            {
                return;
            }

            try
            {
                foreach (var jobbrComponent in this.components)
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
                Logger.FatalException($"A least one service couldn't be started. Reason: {e}\n\n Please see the exception for details.", e.InnerException);
                throw;
            }
        }

        private void WaitForDb()
        {
            while (true)
            {
                try
                {
                    this.jobStorageProvider.IsAvailable();
                    return;
                }
                catch (Exception ex)
                {
                    Logger.ErrorException("Exception while connecting to database to get all Jobs on startup.", ex);
                    Thread.Sleep(1000);
                }
            }
        }

        private void RegisterJobsFromRepository()
        {
            Logger.Info("Adding jobs from the registry");

            try
            {
                var numberOfChanges = this.registryBuilder.Apply(this.jobStorageProvider);
                var numberOfJobs = this.jobStorageProvider.GetJobs().Count;

                Logger.InfoFormat("There were {0} by the JobRegistry. There are now {1} known jobs right available.", numberOfChanges, numberOfJobs);
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot register Jobs on startup. See Execption for details", e);
            }
        }
    }
}
