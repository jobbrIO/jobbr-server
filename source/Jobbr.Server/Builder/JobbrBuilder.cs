using System;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using SimpleInjector;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// Builder class for the entire Jobbr server.
    /// </summary>
    public class JobbrBuilder : IJobbrBuilder
    {
        private readonly ILogger<JobbrBuilder> _logger;
        private readonly Container _dependencyContainer;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        public JobbrBuilder(ILoggerFactory loggerFactory)
        {
            _dependencyContainer = new DefaultContainer(loggerFactory);
            RegisterLogging(loggerFactory);

            _logger = loggerFactory.CreateLogger<JobbrBuilder>();
        }

        /// <summary>
        /// Creates a <see cref="JobbrServer"/>.
        /// </summary>
        /// <param name="maxConcurrentJobs">Maximum amount of concurrent jobs.</param>
        /// <returns>A new <see cref="JobbrServer"/>.</returns>
        public JobbrServer Create(int maxConcurrentJobs = 4)
        {
            _dependencyContainer.Register<JobbrServer>();

            // Register default implementations if user did not specify any separate
            if (_dependencyContainer.GetRegistration(typeof(IJobStorageProvider)) == null)
            {
                _logger.LogError("There was no JobStorageProvider registered. Will continue building with an in-memory version, which does not support production scenarios.");
                _dependencyContainer.RegisterInstance<IJobStorageProvider>(new InMemoryJobStorageProvider());
            }

            // Register default implementations if user did not specify any separate
            if (_dependencyContainer.GetRegistration(typeof(IArtefactsStorageProvider)) == null)
            {
                _logger.LogWarning("There was no ArtefactsStorageProvider registered. Adding a default InMemoryArtefactStorage, which stores artefacts in memory. Please register a proper ArtefactStorage for production use.");
                _dependencyContainer.RegisterInstance<IArtefactsStorageProvider>(new InMemoryArtefactsStorage());
            }

            // Register default implementations if user did not specify any separate
            if (_dependencyContainer.GetRegistration(typeof(IJobExecutor)) == null)
            {
                _logger.LogError("There was no JobExecutor registered. Adding a Non-Operational JobExecutor");
                _dependencyContainer.Register<IJobExecutor, NoExecutor>(Lifestyle.Singleton);
            }

            // Register default implementations if user did not specify any separate
            if (_dependencyContainer.GetRegistration(typeof(IJobScheduler)) == null)
            {
                // Don't warn because the internal Scheduler is usually in use
                this.AddDefaultScheduler();
            }

            var serverMngmtRegistration = _dependencyContainer.GetRegistration<IServerManagementService>();

            if (serverMngmtRegistration != null)
            {
                var serverManagementService = _dependencyContainer.GetInstance<IServerManagementService>();
                serverManagementService.MaxConcurrentJobs = maxConcurrentJobs;
            }
            else
            {
                _logger.LogError("No Server Management Service found.");
            }

            return _dependencyContainer.GetInstance<JobbrServer>();
        }

        /// <summary>
        /// Registers given implementation type to given service type as singleton.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="type">Implementation type.</param>
        public void Register<T>(Type type)
        {
            _dependencyContainer.Register(typeof(T), type, Lifestyle.Singleton);
        }

        /// <summary>
        /// Registers given instance to given service type.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="instance">Instance to register.</param>
        public void Add<T>(object instance)
        {
            _dependencyContainer.RegisterInstance(typeof(T), instance);
        }

        /// <summary>
        /// Appends given instance to a collection of service type registrations.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="instance">Instance to register.</param>
        public void AppendInstanceToCollection<T>(object instance)
        {
            _dependencyContainer.Collection.AppendInstance(typeof(T), instance);
        }

        /// <summary>
        /// Appends given type to a collection of service type registrations.
        /// </summary>
        /// <typeparam name="T">Service type.</typeparam>
        /// <param name="type">Type to register.</param>
        public void AppendTypeToCollection<T>(Type type)
        {
            _dependencyContainer.Collection.Append(typeof(T), type);
        }

        private void RegisterLogging(ILoggerFactory loggerFactory)
        {
            _dependencyContainer.RegisterInstance(loggerFactory);
            _dependencyContainer.RegisterSingleton(typeof(ILogger<>), typeof(Logger<>));
        }
    }
}