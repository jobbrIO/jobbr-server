using System;
using System.Diagnostics.CodeAnalysis;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using Ninject;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// Builder class for the entire Jobbr server.
    /// </summary>
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Cannot dispose container, it is used for the jobbr-server instance.")]
    public class JobbrBuilder : IJobbrBuilder
    {
        private readonly ILogger<JobbrBuilder> _logger;
        private readonly StandardKernel _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">Factory for creating typed loggers.</param>
        public JobbrBuilder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<JobbrBuilder>();
            _container = new DefaultContainer(loggerFactory);
        }

        /// <summary>
        /// Creates a <see cref="JobbrServer"/>.
        /// </summary>
        /// <param name="maxConcurrentJobs">Maximum amount of concurrent jobs.</param>
        /// <returns>A new <see cref="JobbrServer"/>.</returns>
        public JobbrServer Create(int maxConcurrentJobs = 4)
        {
            // Register default implementations if user did not specify any separate
            if (_container.TryGet<IJobStorageProvider>() == null)
            {
                _logger.LogError("There was no JobStorageProvider registered. Will continue building with an in-memory version, which does not support production scenarios.");

                var inMemoryJobStorageProvider = new InMemoryJobStorageProvider();
                _container.Bind<IJobStorageProvider>().ToConstant(inMemoryJobStorageProvider);
            }

            // Register default implementations if user did not specify any separate
            if (_container.TryGet<IArtefactsStorageProvider>() == null)
            {
                _logger.LogWarning("There was no ArtefactsStorageProvider registered. Adding a default InMemoryArtefactStorage, which stores artefacts in memory. Please register a proper ArtefactStorage for production use.");
                var fileSystemArtefactsStorageProvider = new InMemoryArtefactsStorage();
                _container.Bind<IArtefactsStorageProvider>().ToConstant(fileSystemArtefactsStorageProvider);
            }

            // Register default implementations if user did not specify any separate
            if (_container.TryGet<IJobExecutor>() == null)
            {
                _logger.LogError("There was no JobExecutor registered. Adding a Non-Operational JobExecutor");
                _container.Bind<IJobExecutor>().To<NoExecutor>();
            }

            // Register default implementations if user did not specify any separate
            if (_container.TryGet<IJobScheduler>() == null)
            {
                // Don't warn because the internal Scheduler is usually in use
                this.AddDefaultScheduler();
            }

            var serverManagementService = _container.TryGet<IServerManagementService>();

            if (serverManagementService != null)
            {
                serverManagementService.MaxConcurrentJobs = maxConcurrentJobs;
            }
            else
            {
                _logger.LogError("No Server Management Service found.");
            }

            return _container.Get<JobbrServer>();
        }

        public void Register<T>(Type type)
        {
            _container.Bind<T>().To(type).InSingletonScope();
        }

        public void Add<T>(object instance)
        {
            if (instance is IFeatureConfiguration featureConfiguration)
            {
                _container.Bind<IFeatureConfiguration>().ToConstant(featureConfiguration);
            }

            _container.Bind<T>().ToConstant((T)instance);
        }
    }
}