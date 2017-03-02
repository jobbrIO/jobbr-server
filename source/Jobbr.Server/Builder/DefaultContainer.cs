using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Logging;
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
        private static ILog Logger = LogProvider.For<JobRunInformationService>();

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
            var profileTypes = this.GetType().Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("Jobbr.Server") && typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract);

            var profiles = new List<Profile>();

            Logger.Debug($"Found {profileTypes} types that need to be registered in internal AutoMapper.");

            foreach (var profileType in profileTypes)
            {
                Logger.Debug($"Activating type {profileType.Name} in '{profileType.Namespace}' from '{profileType.Assembly}'");

                // Don't try/catch here, better fail early (in the creation of Jobbr server)
                var profile = (Profile)Activator.CreateInstance(profileType);
                profiles.Add(profile);
            }

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