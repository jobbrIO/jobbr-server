using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.JobRegistry
{
    public class JobDefinition
    {
        private readonly List<JobTriggerBase> triggers = new List<JobTriggerBase>();

        private bool hasTriggerDefinition;

        internal string UniqueName { get; set; }

        internal string ClrType { get; set; }

        internal List<JobTriggerBase> Triggers
        {
            get
            {
                return this.triggers;
            }
        }

        public bool HasTriggerDefinition
        {
            get
            {
                return this.hasTriggerDefinition;
            }
        }

        public JobDefinition WithTrigger(DateTime startDateTimeUtc)
        {
            this.hasTriggerDefinition = true;

            this.triggers.Add(new ScheduledTrigger() { StartDateTimeUtc = startDateTimeUtc });
            
            return this;
        }

        public JobDefinition WithTrigger(string cronDefinition, DateTime? validFromDateTimeUtc = null, DateTime? validToDateTimeUtc = null, bool noParallelExecution = false)
        {
            this.hasTriggerDefinition = true;

            NCrontab.CrontabSchedule.Parse(cronDefinition);

            this.triggers.Add(new RecurringTrigger { StartDateTimeUtc = validFromDateTimeUtc, EndDateTimeUtc = validToDateTimeUtc, Definition = cronDefinition, NoParallelExecution = noParallelExecution });

            return this;
        }

    }
}