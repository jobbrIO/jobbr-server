using System;
using System.Diagnostics.CodeAnalysis;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Logging;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Storage;
using Ninject;

namespace Jobbr.Server.Builder
{
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Cannot disopose container, its used for the jobbr-server instance")]
    [SuppressMessage("Design", "CA2213:Disposable fields should be disposed", Justification = "Cannot disopose container, its used for the jobbr-server instance")]
    public class JobbrBuilder : IJobbrBuilder
    {
        private static readonly ILog Logger = LogProvider.For<JobbrBuilder>();
        private readonly StandardKernel container;

        public JobbrBuilder()
        {
            this.container = new DefaultContainer();
        }

        public JobbrServer Create(int maxConcurrentJobs = 4)
        {
            // Register default implementations if user did not specify any separate
            if (this.container.TryGet<IJobStorageProvider>() == null)
            {
                Logger.Error("There was no JobStorageProvider registered. Will continue building with an InMemory version, which does not support production scenarios.");

                var inMemoryJobStorageProvider = new InMemoryJobStorageProvider();
                this.container.Bind<IJobStorageProvider>().ToConstant(inMemoryJobStorageProvider);
            }

            // Register default implementations if user did not specify any separate
            if (this.container.TryGet<IArtefactsStorageProvider>() == null)
            {
                Logger.Warn("There was no ArtefactsStorageProvider registered. Adding a default InMemoryArtefactStorage, which stores artefacts in memory. Please register a proper ArtefactStorage for production use.");
                var fileSystemArtefactsStorageProvider = new InMemoryArtefactsStorage();
                this.container.Bind<IArtefactsStorageProvider>().ToConstant(fileSystemArtefactsStorageProvider);
            }

            // Register default implementations if user did not specify any separate
            if (this.container.TryGet<IJobExecutor>() == null)
            {
                Logger.Error("There was no JobExecutor registered. Adding a Non-Operational JobExecutor");
                this.container.Bind<IJobExecutor>().To<NoExecutor>();
            }

            // Register default implementations if user did not specify any separate
            if (this.container.TryGet<IJobScheduler>() == null)
            {
                // Don't warn because the internal Scheduler is usually in use
                this.AddDefaultScheduler();
            }

            var serverManagementService = this.container.TryGet<IServerManagementService>();

            if (serverManagementService != null)
            {
                serverManagementService.MaxConcurrentJobs = maxConcurrentJobs;
            }
            else
            {
                Logger.Error("No Server Management Service found.");
            }

            return this.container.Get<JobbrServer>();
        }

        public void Register<T>(Type type)
        {
            this.container.Bind<T>().To(type).InSingletonScope();
        }

        public void Add<T>(object instance)
        {
            if (instance is IFeatureConfiguration featureConfiguration)
            {
                this.container.Bind<IFeatureConfiguration>().ToConstant(featureConfiguration);
            }

            this.container.Bind<T>().ToConstant((T)instance);
        }
    }
}