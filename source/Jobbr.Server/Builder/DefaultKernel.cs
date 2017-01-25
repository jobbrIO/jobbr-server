using System;
using System.IO;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;

using Jobbr.Server.Core;
using Jobbr.Server.Scheduling;
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
            // Management related services
            this.Bind<IJobManagementService>().To<ComponentModel.Services.JobManagementService>().InSingletonScope();
            this.Bind<IQueryService>().To<ComponentModel.Services.JobQueryService>().InSingletonScope();

            // Execution related services
            this.Bind<IJobRunInformationService>().To<ComponentModel.Services.JobRunInformationService>().InSingletonScope();
            this.Bind<IJobRunProgressChannel>().To<ComponentModel.Services.JobRunProgressReceiver>().InSingletonScope();
        }
    }
}

namespace Jobbr.Server.ComponentModel.Services
{
    internal class JobRunProgressReceiver : IJobRunProgressChannel
    {
        public void PublishStatusUpdate(JobRunInfo jobRunInfo, JobRunStates state)
        {
            //TODO: this.stateService.SetJobRunStartTime(jobRun, DateTime.UtcNow);
        }

        public void PublishProgressUpdate(JobRunInfo jobRunInfo, double progress)
        {

        }

        public void PublicArtefact(Guid uniqueId, string fileName, Stream result)
        {
            throw new NotImplementedException();
        }
    }
}