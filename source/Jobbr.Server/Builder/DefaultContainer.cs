using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Retention;
using Jobbr.Server.Storage;
using Ninject;
using TinyMessenger;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// The kernel.
    /// </summary>
    internal class DefaultContainer : StandardKernel
    {
        private readonly AutoMapperConfigurationFactory autoMapperConfigurationFactory = new AutoMapperConfigurationFactory();

        public DefaultContainer()
        {
            this.AddCoreServices();

            this.AddAutoMapper();

            this.AddComponentModelImplementations();
        }

        private void AddCoreServices()
        {
            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            this.Bind<ITinyMessengerHub>().To<TinyMessengerHub>().InSingletonScope();
        }

        private void AddAutoMapper()
        {
            var config = this.autoMapperConfigurationFactory.GetNew();

            this.Bind<MapperConfiguration>().ToConstant(config);
            this.Bind<IMapper>().ToProvider<AutoMapperProvider>();
        }

        private void AddComponentModelImplementations()
        {
            // Registration
            this.Bind<IJobbrServiceProvider>().ToConstant(new JobbrServiceProvider(this));

            // Management related services
            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IQueryService>().To<JobQueryService>().InSingletonScope();
            this.Bind<IServerManagementService>().To<ServerManagementService>().InSingletonScope();

            // Execution related services
            this.Bind<IJobRunInformationService>().To<JobRunInformationService>().InSingletonScope();
            this.Bind<IJobRunProgressChannel>().To<JobRunProgressReceiver>().InSingletonScope();
        }
    }
}