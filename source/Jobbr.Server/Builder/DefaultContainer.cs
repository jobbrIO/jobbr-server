using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using Ninject;
using TinyMessenger;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// The kernel.
    /// </summary>
    internal class DefaultContainer : StandardKernel
    {
        private readonly AutoMapperConfigurationFactory _autoMapperConfigurationFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        public DefaultContainer(ILoggerFactory loggerFactory)
        {
            _autoMapperConfigurationFactory = new AutoMapperConfigurationFactory(loggerFactory.CreateLogger<AutoMapperConfigurationFactory>());
            AddCoreServices();
            AddAutoMapper();
            AddComponentModelImplementations();
        }

        private void AddCoreServices()
        {
            Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            Bind<ITinyMessengerHub>().To<TinyMessengerHub>().InSingletonScope();
        }

        private void AddAutoMapper()
        {
            var config = _autoMapperConfigurationFactory.GetNew();

            Bind<MapperConfiguration>().ToConstant(config);
            Bind<IMapper>().ToProvider<AutoMapperProvider>();
        }

        private void AddComponentModelImplementations()
        {
            // Registration
            Bind<IJobbrServiceProvider>().ToConstant(new JobbrServiceProvider(this));

            // Management related services
            Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            Bind<IQueryService>().To<JobQueryService>().InSingletonScope();
            Bind<IServerManagementService>().To<ServerManagementService>().InSingletonScope();

            // Execution related services
            Bind<IJobRunInformationService>().To<JobRunInformationService>().InSingletonScope();
            Bind<IJobRunProgressChannel>().To<JobRunProgressReceiver>().InSingletonScope();
        }
    }
}