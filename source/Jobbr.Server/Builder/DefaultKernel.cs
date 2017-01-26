using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Core;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Storage;
using Ninject;
using IJobManagementService = Jobbr.ComponentModel.Management.IJobManagementService;
using JobManagementService = Jobbr.Server.ComponentServices.Management.JobManagementService;

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
            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();

            this.AddComponentModelServices();
        }

        private void AddComponentModelServices()
        {
            // Registration
            this.Bind<IJobbrServiceProvider>().ToConstant(new JobbrServiceProvider(this));

            // Management related services
            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IQueryService>().To<JobQueryService>().InSingletonScope();

            // Execution related services
            this.Bind<IJobRunInformationService>().To<JobRunInformationService>().InSingletonScope();
            this.Bind<IJobRunProgressChannel>().To<JobRunProgressReceiver>().InSingletonScope();
        }
    }
}