using System;
using System.Diagnostics;
using Jobbr.ComponentModel.Management;

namespace Jobbr.Server.ComponentServices.Management
{
    /// <summary>
    /// Service for server management.
    /// </summary>
    public class ServerManagementService : IServerManagementService
    {
        /// <summary>
        /// The current maximum concurrent jobs.
        /// </summary>
        public int MaxConcurrentJobs { get; set; }

        /// <summary>
        /// The current server process start time as UTC.
        /// </summary>
        public DateTime StartTime => Process.GetCurrentProcess().StartTime.ToUniversalTime();

        /// <summary>
        /// Shutdown. Does nothing.
        /// </summary>
        public void Shutdown()
        {
        }
    }
}
