using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Core;

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
            this.Bind<IJobbrDependencyResolver>().ToConstant(new JobbrDependencyResolver(this));
            this.Bind<IJobbrStorageProvider>().ToConstant(configuration.StorageProvider);

            this.Bind<IJobService>().To<JobService>().InSingletonScope();
            this.Bind<DefaultScheduler>().To<DefaultScheduler>();
            this.Bind<IJobExecutor>().To<ProcessJobExecutor>();
        }
    }
}
