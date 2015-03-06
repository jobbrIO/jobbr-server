using System;
using System.Threading;

using Jobbr.Server.Common;
using Jobbr.Server.Core;

namespace Jobbr.Server
{
    /// <summary>
    /// The JobStarter interface.
    /// </summary>
    public interface IJobExecutor
    {
        void Start();

        void Stop();
    }
}