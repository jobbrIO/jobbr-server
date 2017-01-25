using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Logging;

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
        private DefaultScheduler scheduler;

        /// <summary>
        /// The executor.
        /// </summary>
        private IJobExecutor executor;

        private bool isRunning;

        private readonly List<IJobbrComponent> components;
        private readonly List<IConfigurationValidator> configurationValidators;
        private readonly JobbrServiceProvider jobbrServiceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        public JobbrServer(IJobbrConfiguration configuration, DefaultScheduler scheduler, IJobExecutor jobExecutor, List<IJobbrComponent> components, List<IConfigurationValidator> configurationValidators, JobbrServiceProvider jobbrServiceProvider )
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }

            Logger.Debug("A new instance of a a JobbrServer has been created.");

            this.scheduler = scheduler;

            this.components = components;
            this.configurationValidators = configurationValidators;
            this.jobbrServiceProvider = jobbrServiceProvider;
            this.executor = jobExecutor;
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
            }
        }

        private void WaitForDb()
        {
            while (true)
            {
                try
                {
                    // TODO: Check DB Access with storage provider explicitly
                    // this.configuration.JobStorageProvider.GetJobs();
                    return;
                }
                catch (Exception e)
                {
                    Thread.Sleep(1000);
                }
            }
        }

        private void SetScheduledJobsFromPastToOmitted()
        {
            // TODO: Reimplement this
            //var scheduledJobs = this.configuration.JobStorageProvider.GetJobRunsByState(JobRunState.Scheduled).Where(p => p.PlannedStartDateTimeUtc < DateTime.UtcNow);

            //foreach (var job in scheduledJobs)
            //{
            //    job.State = JobRunState.Omitted;
            //    this.configuration.JobStorageProvider.Update(job);
            //}
        }

        private void RegisterJobsFromRepository()
        {
            try
            {
                Logger.Info("Registering Jobs from configuration");

                // TODO: Re implement this
                //var model = new RepositoryBuilder();

                //this.configuration.OnRepositoryCreating(model);
                //var numberOfChanges = model.Apply(this.configuration.JobStorageProvider);
                //var numberOfJobs = this.configuration.JobStorageProvider.GetJobs().Count;

                //Logger.InfoFormat("There were {0} changes for the JobRegistry which contains {1} jobs right now.", numberOfChanges, numberOfJobs);
            }
            catch (Exception e)
            {
                Logger.FatalException("Cannot register Jobs on startup. See Execption for details", e);
            }
        }

        private void LogConfiguration()
        {
            //Logger.InfoFormat("JobbrServer is now starting with the following configuration: BackendUrl='{0}', MaxConcurrentJobs='{1}', JobRunDirectory='{2}', JobRunnerExeutable='{3}', JobStorageProvider='{4}' ArtefactsStorageProvider='{5}'",
            //    this.configuration.BackendAddress,
            //    this.configuration.MaxConcurrentJobs,
            //    this.configuration.JobRunDirectory,
            //    this.configuration.JobRunnerExeResolver != null ? Path.GetFullPath(this.configuration.JobRunnerExeResolver()) : "none",
            //    this.configuration.JobStorageProvider,
            //    this.configuration.ArtefactStorageProvider);
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

        private void ValidateConfigurationAndThrowOnErrors()
        {
            if (this.configurationValidators == null || !this.configurationValidators.Any())
            {
                Logger.Debug("Skipping validating configuration because there are no validators available.");
                return;
            }

            Logger.Debug("Validating the configuration...");

            var results = new Dictionary<Type, bool>();

            foreach (var validator in this.configurationValidators)
            {
                var forType = validator.ConfigurationType;

                var config = this.jobbrServiceProvider.GetService(forType);

                if (config == null)
                {
                    Logger.Warn($"Unable to use Validator '{validator.GetType().FullName}' because there are no compatible configurations (of Type '{forType.FullName}') registered.");
                    continue;
                }

                Logger.Debug($"Validating configuration of Type '{config.GetType()}'");

                try
                {
                    var result = validator.Validate(config);

                    if (result)
                    {
                        Logger.Info($"Configuration '{config.GetType().Name}' has been validated successfully");
                    }
                    else
                    {
                        Logger.Warn($"Validation for Configuration '{config.GetType().Name}' failed.");
                    }

                    results.Add(forType, result);
                }
                catch (Exception e)
                {
                    Logger.ErrorException($"Validator '{validator.GetType().FullName}' has failed while validation!", e);
                    results.Add(forType, false);
                }
            }

            if (!results.Values.All(r => r))
            {
                this.State = JobbrState.Error;
                throw new Exception("Configuration failed for one or more configurations");
            }

            Logger.Info("The configuration was validated and seems ok.");

            // TODO: Move validation to API Feature
            //if (string.IsNullOrEmpty(this.configuration.BackendAddress))
            //{
            //    throw new ArgumentException("Please provide a backend address to host the api!");
            //}

            // TODO: Move validation to foked execution feature
            //if (string.IsNullOrEmpty(this.configuration.JobRunDirectory))
            //{
            //    throw new ArgumentException("Please provide a JobRunDirectory!");
            //}

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
