using Jobbr.Common.Model;

namespace Jobbr.Server.Core
{
    using System;

    public class JobRunModificationEventArgs : EventArgs
    {
        public Job Job { get; set; }

        public JobTriggerBase Trigger { get; set; }

        public JobRun JobRun { get; set; }
    }
}