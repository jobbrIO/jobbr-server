﻿using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;

using Jobbr.Server.Core;
using Ninject;

using IJobManagementService = Jobbr.ComponentModel.Management.IJobManagementService;
using IQueryService = Jobbr.ComponentModel.Management.IQueryService;

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

            this.Bind<IJobbrRepository>().To<JobbrRepository>().InSingletonScope();
            this.Bind<Core.IJobManagementService>().To<Core.JobManagementService>().InSingletonScope();
            this.Bind<IStateService>().To<StateService>().InSingletonScope();

            this.Bind<DefaultScheduler>().To<DefaultScheduler>();

            this.AddComponentModelServices();
        }

        private void AddComponentModelServices()
        {
            this.Bind<IJobManagementService>().To<ComponentModel.Services.JobManagementService>().InSingletonScope();
            this.Bind<IQueryService>().To<ComponentModel.Services.JobQueryService>().InSingletonScope();
        }
    }
}
