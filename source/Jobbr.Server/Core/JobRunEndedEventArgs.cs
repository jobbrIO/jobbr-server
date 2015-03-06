using System;

using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    internal class JobRunEndedEventArgs : EventArgs
    {
        public int ExitCode { get; set; }

        public JobRun JobRun { get; set; }
    }
}