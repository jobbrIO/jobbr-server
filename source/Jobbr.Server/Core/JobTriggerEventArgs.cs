using System;

using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The job trigger event args.
    /// </summary>
    public class JobTriggerEventArgs : EventArgs
    {
        public JobTriggerBase Trigger { get; set; }
    }
}