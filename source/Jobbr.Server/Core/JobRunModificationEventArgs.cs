namespace Jobbr.Server.Core
{
    using System;

    using Jobbr.Server.Model;

    public class JobRunModificationEventArgs : EventArgs
    {
        public Job Job { get; set; }

        public JobTriggerBase Trigger { get; set; }

        public JobRun JobRun { get; set; }
    }
}