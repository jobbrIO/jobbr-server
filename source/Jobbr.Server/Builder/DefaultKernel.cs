using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Shared;
using Ninject;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// The kernel.
    /// </summary>
    internal class DefaultContainer : StandardKernel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public DefaultContainer()
        {
            this.Bind<IJobbrDependencyResolver>().ToConstant(new JobbrDependencyResolver(this));

            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            this.Bind<IStateService>().To<StateService>().InSingletonScope();
            this.Bind<DefaultScheduler>().To<DefaultScheduler>();
            this.Bind<IJobExecutor>().To<DefaultJobExecutor>();
        }
    }
}
