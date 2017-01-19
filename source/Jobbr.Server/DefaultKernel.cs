using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Shared;

using Ninject;

namespace Jobbr.Server
{
    /// <summary>
    /// The kernel.
    /// </summary>
    public class DefaultKernel : StandardKernel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKernel"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public DefaultKernel(IJobbrConfiguration configuration)
        {
            this.Bind<IJobbrConfiguration>().ToConstant(configuration);
            this.Bind<IJobStorageProvider>().ToConstant(configuration.JobStorageProvider);
            this.Bind<IArtefactsStorageProvider>().ToConstant(configuration.ArtefactStorageProvider);

            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            this.Bind<IStateService>().To<StateService>().InSingletonScope();
            this.Bind<DefaultScheduler>().To<DefaultScheduler>();
            this.Bind<IJobExecutor>().To<DefaultJobExecutor>();
        }
    }
}
