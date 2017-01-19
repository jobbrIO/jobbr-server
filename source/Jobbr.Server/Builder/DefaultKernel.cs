using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;
using Jobbr.Server.ComponentModel.Services;
using Jobbr.Server.Core;
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
            this.Bind<IJobbrServiceProvider>().ToConstant(new JobbrServiceProvider(this));

            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            this.Bind<IStateService>().To<StateService>().InSingletonScope();
            this.Bind<DefaultScheduler>().To<DefaultScheduler>();
            this.Bind<IJobExecutor>().To<DefaultJobExecutor>();

            this.AddComponentModelServices();
        }

        private void AddComponentModelServices()
        {
            this.Bind<Jobbr.ComponentModel.Management.IJobManagementService>().To<JobManagementServiceImplementation>();
        }


    }
}
