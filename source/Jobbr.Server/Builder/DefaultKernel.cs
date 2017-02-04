using System;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Core;
using Jobbr.Server.Storage;
using Ninject;
using Ninject.Activation;
using TinyMessenger;

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

            this.AddAutoMapper();

            this.AddComponentModelServices();
        }

        private void AddAutoMapper()
        {
            var profiles = Assembly.GetCallingAssembly().GetTypes().Where(t => typeof(Profile).IsAssignableFrom(t)).Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            config.AssertConfigurationIsValid();

            this.Bind<MapperConfiguration>().ToConstant(config);
            this.Bind<IMapper>().ToProvider<AutoMapperProvider>();
        }

        private void AddComponentModelServices()
        {
            // Registration
            this.Bind<IJobbrServiceProvider>().ToConstant(new JobbrServiceProvider(this));

            // Message Bus
            this.Bind<ITinyMessengerHub>().To<TinyMessengerHub>().InSingletonScope();

            // Management related services
            this.Bind<IJobManagementService>().To<JobManagementService>().InSingletonScope();
            this.Bind<IQueryService>().To<JobQueryService>().InSingletonScope();
            this.Bind<IServerManagementService>().To<ServerManagementService>().InSingletonScope();

            // Execution related services
            this.Bind<IJobRunInformationService>().To<JobRunInformationService>().InSingletonScope();
            this.Bind<IJobRunProgressChannel>().To<JobRunProgressReceiver>().InSingletonScope();
        }
    }

    internal class AutoMapperProvider : IProvider
    {
        private readonly MapperConfiguration mapperConfiguration;

        public AutoMapperProvider(MapperConfiguration mapperConfiguration)
        {
            this.mapperConfiguration = mapperConfiguration;
        }

        public object Create(IContext context)
        {
            return this.mapperConfiguration.CreateMapper();
        }

        public Type Type => typeof(IMapper);
    }
}