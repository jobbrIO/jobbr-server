using System.Threading;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.IntegrationTests.Integration
{
    /// <summary>
    /// This component consumes all services provided by a standard JobbrServer instance and makes them available by separate properties
    /// To use this class, register it as a component in the JobbrBuilder.
    /// </summary>
    public class ExposeAllServicesComponent : IJobbrComponent
    {
        private static readonly ThreadLocal<ExposeAllServicesComponent> InstancesPerThread = new ();

        public ExposeAllServicesComponent(IJobbrServiceProvider serviceProvider, IArtefactsStorageProvider artefactsStorageProvider, IJobStorageProvider jobStorageProvider, IJobManagementService jobManagementService, IQueryService queryService, IServerManagementService managementService, IJobRunInformationService informationService, IJobRunProgressChannel progressChannel)
        {
            ServiceProvider = serviceProvider;
            ArtefactsStorageProvider = artefactsStorageProvider;
            JobStorageProvider = jobStorageProvider;
            JobManagementService = jobManagementService;
            QueryService = queryService;
            ManagementService = managementService;
            InformationService = informationService;
            ProgressChannel = progressChannel;

            InstancesPerThread.Value = this;
        }

        public static ExposeAllServicesComponent Instance => InstancesPerThread.Value;

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
            InstancesPerThread.Value = null;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}