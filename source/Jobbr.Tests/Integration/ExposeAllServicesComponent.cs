using System.Threading;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Tests.Integration
{
    /// <summary>
    /// This component consumes all services provided by a standard JobbrServer instance and makes them available by separate properties
    /// 
    /// To use this class, register it as a component in the JobbrBuilder.
    /// </summary>
    public class ExposeAllServicesComponent : IJobbrComponent
    {
        public static ExposeAllServicesComponent Instance => instancesPerThread.Value;

        private static ThreadLocal<ExposeAllServicesComponent> instancesPerThread = new ThreadLocal<ExposeAllServicesComponent>();

        public ExposeAllServicesComponent(IJobbrServiceProvider serviceProvider, IArtefactsStorageProvider artefactsStorageProvider, IJobStorageProvider jobStorageProvider, IJobManagementService jobManagementService, IQueryService queryService, IServerManagementService managementService, IJobRunInformationService informationService, IJobRunProgressChannel progressChannel)
        {
            this.ServiceProvider = serviceProvider;
            this.ArtefactsStorageProvider = artefactsStorageProvider;
            this.JobStorageProvider = jobStorageProvider;
            this.JobManagementService = jobManagementService;
            this.QueryService = queryService;
            this.ManagementService = managementService;
            this.InformationService = informationService;
            this.ProgressChannel = progressChannel;

            instancesPerThread.Value = this;
        }

        public IJobbrServiceProvider ServiceProvider { get; }

        public IArtefactsStorageProvider ArtefactsStorageProvider { get; }

        public IJobStorageProvider JobStorageProvider { get; }

        public IJobManagementService JobManagementService { get; }

        public IQueryService QueryService { get; }

        public IServerManagementService ManagementService { get; }

        public IJobRunInformationService InformationService { get; }
        public IJobRunProgressChannel ProgressChannel { get; }
        public void Dispose()
        {
            instancesPerThread.Value = null;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}