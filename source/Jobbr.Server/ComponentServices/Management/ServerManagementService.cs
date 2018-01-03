using System;
using System.Diagnostics;
using Jobbr.ComponentModel.Management;

namespace Jobbr.Server.ComponentServices.Management
{
    public class ServerManagementService : IServerManagementService
    {
        public void Shutdown()
        {
        }

        public int MaxConcurrentJobs { get; set; }

        public DateTime StartTime => Process.GetCurrentProcess().StartTime.ToUniversalTime();
    }
}
