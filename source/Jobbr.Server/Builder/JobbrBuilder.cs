using System;
using System.IO;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;
using Jobbr.Server.Configuration;
using Jobbr.Server.Logging;
using Ninject;

namespace Jobbr.Server.Builder
{
    public class JobbrBuilder : IJobbrBuilder
    {
        private static readonly ILog Logger = LogProvider.For<JobbrBuilder>();

        private StandardKernel container;

        public JobbrBuilder()
        {
            this.container = new DefaultContainer();
        }

        public JobbrServer Create()
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
                Logger.Error("There was no ArtefactsStorageProvider registered. Adding a default FileSystemArtefactsStory, which stores artefacts in the current directory.");
                var fileSystemArtefactsStorageProvider = new FileSystemArtefactsStorageProvider(Directory.GetCurrentDirectory());
                this.container.Bind<IArtefactsStorageProvider>().ToConstant(fileSystemArtefactsStorageProvider);
            }

            // Register default implementations if user did not specify any separate
            if (this.container.TryGet<IJobExecutor>() == null)
            {
                Logger.Error("There was no JobExecutor registered. Adding a Non-Operational JobExecutor");
                this.container.Bind<IJobExecutor>().To<NoExecutor>();
            }

            if (this.container.TryGet<IJobbrConfiguration>() == null)
            {
                this.container.Bind<IJobbrConfiguration>().ToConstant(new DefaultJobbrConfiguration());
            }

            return this.container.Get<JobbrServer>();
        }

        public void Register<T>(Type type)
        {
            this.container.Bind<T>().To(type).InSingletonScope();
        }

        public void Add<T>(object instance)
        {
            this.container.Bind<T>().ToConstant((T)instance);
        }
    }
}
