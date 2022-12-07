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
using Jobbr.Server.Scheduling;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server
{
    /// <summary>
    /// The Jobbr job server.
    /// </summary>
    public class JobbrServer : IDisposable
    {
        private readonly ILogger<JobbrServer> _logger;
        private readonly IJobScheduler _scheduler;
        private readonly IJobExecutor _executor;
        private readonly IJobStorageProvider _jobStorageProvider;
        private readonly List<IJobbrComponent> _jobbrComponents;
        private readonly IConfigurationManager _configurationManager;
        private readonly IRegistryBuilder _registryBuilder;

        private bool _isRunning;

        /// <summary>
        /// The state of the Jobbr server.
        /// </summary>
        public JobbrState State { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        public JobbrServer(ILoggerFactory loggerFactory, IJobScheduler scheduler, IJobExecutor jobExecutor, IJobStorageProvider jobStorageProvider, List<IJobbrComponent> jobbrComponents, IMessageDispatcher messageDispatcher, IConfigurationManager configurationManager, IRegistryBuilder registryBuilder)
        {
            _logger = loggerFactory.CreateLogger<JobbrServer>();

            _scheduler = scheduler;
            _executor = jobExecutor;
            _jobbrComponents = jobbrComponents;
            _configurationManager = configurationManager;
            _registryBuilder = registryBuilder;
            _jobStorageProvider = jobStorageProvider;

            _logger.LogDebug("A new instance of a JobbrServer has been created.");

            messageDispatcher.WireUp();
        }

        /// <summary>
        /// Start the Jobbr server synchronously.
        /// </summary>
        /// <param name="waitForStartupTimeout">The timeout for the start in milliseconds.</param>
        /// <returns>True if start was a success, false if not.</returns>
        public bool Start(int waitForStartupTimeout = 2000)
        {
            var startupTask = StartAsync(CancellationToken.None);

            try
            {
                startupTask.Wait(waitForStartupTimeout);
            }
            catch (AggregateException e)
            {
                State = JobbrState.Error;
                throw e.InnerExceptions[0];
            }

            if (!startupTask.IsCompleted)
            {
                _logger.LogCritical("Jobbr was unable to start within {wait}ms. Keep starting but returning now from Start()", waitForStartupTimeout);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Start the Jobbr server asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token for cancelling the async call.</param>
        /// <returns>A task that will return true if start was a success and false if not.</returns>
        public async Task<bool> StartAsync(CancellationToken cancellationToken)
        {
            State = JobbrState.Initializing;

            LogVersions();

            _logger.LogInformation("The JobServer has been set-up by the following configurations");
            _configurationManager.LogConfiguration();

            State = JobbrState.Validating;

            try
            {
                _configurationManager.ValidateConfigurationAndThrowOnErrors();
                _logger.LogInformation("The configuration was validated and seems ok. Final configuration below:");

                _configurationManager.LogConfiguration();
            }
            catch (Exception)
            {
                State = JobbrState.Error;
                throw;
            }

            State = JobbrState.Starting;

            var waitForDbTask = new Task(WaitForDb, cancellationToken);

            var startComponents = waitForDbTask.ContinueWith(
                t =>
                    {
                        RegisterJobsFromRepository();
                        StartInternalComponents();
                        StartOptionalComponents();

                        _isRunning = true;
                    },
                cancellationToken);

            waitForDbTask.Start();

            await Task.WhenAll(waitForDbTask, startComponents);

            State = JobbrState.Running;

            return true;
        }

        /// <summary>
        /// Stop the server and it's components.
        /// </summary>
        public void Stop()
        {
            _logger.LogInformation("Attempting to shut down JobbrServer...");

            _jobbrComponents.ForEach(component => component.Stop());

            _scheduler.Stop();
            _executor.Stop();

            _logger.LogInformation("All components stopped.");
        }

        /// <summary>
        /// Stop the server if it is running.
        /// </summary>
        public void Dispose()
        {
            if (_isRunning)
            {
                Stop();
            }
        }

        private void LogVersions()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Version Information:");

            var assemblies = new List<Assembly> { GetType().Assembly };
            assemblies.AddRange(_jobbrComponents.Select(component => component.GetType().Assembly));

            assemblies.Distinct().ToList().ForEach(assembly => { sb.AppendLine($"{assembly.ManifestModule.Name} {assembly.GetVersion()}"); });

            _logger.LogInformation("{assemblies}", sb.ToString().TrimEnd());
        }

        private void StartInternalComponents()
        {
            try
            {
                _logger.LogDebug("Starting Scheduler ({0})...", _scheduler.GetType().Name);
                _scheduler.Start();

                _logger.LogDebug("Starting Executor ({0})...", _executor.GetType().Name);
                _executor.Start();

                _logger.LogInformation("All services (Scheduler, Executor) have been started successfully.");
            }
            catch (Exception e)
            {
                _logger.LogCritical("A least one service couldn't be started. Please see the exception for details. '{exception}'", e);
            }
        }

        private void StartOptionalComponents()
        {
            if (_jobbrComponents == null)
            {
                return;
            }

            try
            {
                foreach (var jobbrComponent in _jobbrComponents)
                {
                    var type = jobbrComponent.GetType();
                    _logger.LogDebug("Starting JobbrComponent '{fullName}' ...", type.FullName);
                    jobbrComponent.Start();
                }

                _logger.LogInformation("All Optional Services have been started successfully.");

                _isRunning = true;
            }
            catch (Exception e)
            {
                _logger.LogCritical("At least one of the services couldn't be started. Reason: {e}\n\n Please see the exception for details.", e.InnerException);
                throw;
            }
        }

        private void WaitForDb()
        {
            while (true)
            {
                try
                {
                    _jobStorageProvider.IsAvailable();
                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception while connecting to database to get all Jobs on startup: '{e}'", ex);
                    Thread.Sleep(1000);
                }
            }
        }

        private void RegisterJobsFromRepository()
        {
            _logger.LogInformation("Adding jobs from the registry");

            try
            {
                var numberOfChanges = _registryBuilder.Apply(_jobStorageProvider);
                var numberOfJobs = _jobStorageProvider.GetJobs(pageSize: int.MaxValue).Items.Count;

                _logger.LogInformation("There were {changeCount} changes by the JobRegistry. There are now {jobCount} known jobs right available.", numberOfChanges, numberOfJobs);
            }
            catch (Exception e)
            {
                _logger.LogCritical("Cannot register Jobs on startup. See Exception for details: {e}", e);
            }
        }
    }
}
