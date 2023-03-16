using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.ComponentServices.Execution;
using Jobbr.Server.ComponentServices.Management;
using Jobbr.Server.ComponentServices.Registration;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.JobRegistry;
using Jobbr.Server.Scheduling.Planer;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using TinyMessenger;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// The default dependency injection container for Jobbr.
    /// </summary>
    internal class DefaultContainer : Container
    {
        private readonly AutoMapperConfigurationFactory _autoMapperConfigurationFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultContainer"/> class.
        /// </summary>
        public DefaultContainer(ILoggerFactory loggerFactory)
        {
            // This is done so we can manually check for the services in the container and use in-memory ones if something is missing.
            // SimpleInjector will throw an error if this is enabled.
            Options.EnableAutoVerification = false;

            // This is allowed to provide overriding the defaults set in this class.
            Options.AllowOverridingRegistrations = true;

            _autoMapperConfigurationFactory = new AutoMapperConfigurationFactory(loggerFactory);
            AddCoreServices();
            AddAutoMapper();
            AddComponentModelImplementations();
        }

        private void AddCoreServices()
        {
            Register<IJobbrRepository, JobbrRepository>(Lifestyle.Singleton);
            Register<ITinyMessengerHub, TinyMessengerHub>(Lifestyle.Singleton);
        }

        private void AddAutoMapper()
        {
            var config = _autoMapperConfigurationFactory.GetNew();

            RegisterInstance(config.CreateMapper());
        }

        private void AddComponentModelImplementations()
        {
            // Registration
            RegisterInstance<IJobbrServiceProvider>(new JobbrServiceProvider(this));

            // Management related services
            Register<IJobManagementService, JobManagementService>(Lifestyle.Singleton);
            Register<IQueryService, JobQueryService>(Lifestyle.Singleton);
            Register<IServerManagementService, ServerManagementService>(Lifestyle.Singleton);

            // Execution related services
            Register<IJobRunInformationService, JobRunInformationService>(Lifestyle.Singleton);
            Register<IJobRunProgressChannel, JobRunProgressReceiver>(Lifestyle.Singleton);

            // Job run planners
            Register<IInstantJobRunPlaner, InstantJobRunPlaner>(Lifestyle.Singleton);
            Register<IScheduledJobRunPlaner, ScheduledJobRunPlaner>(Lifestyle.Singleton);
            Register<IRecurringJobRunPlaner, RecurringJobRunPlaner>(Lifestyle.Singleton);

            // Services
            Register<ITriggerService, TriggerService>(Lifestyle.Singleton);
            Register<IJobService, JobService>(Lifestyle.Singleton);
            Register<IJobRunService, JobRunService>(Lifestyle.Singleton);

            Register<IMessageDispatcher, MessageDispatcher>(Lifestyle.Singleton);
            Register<IConfigurationManager, ConfigurationManager>(Lifestyle.Singleton);
            Register<IRegistryBuilder, RegistryBuilder>(Lifestyle.Singleton);

            Collection.Register(typeof(IConfigurationValidator), new List<Type>());
            Collection.Register(typeof(IFeatureConfiguration), new List<Type>());
            Collection.Register(typeof(IJobbrComponent), new List<Type>());
        }
    }
}