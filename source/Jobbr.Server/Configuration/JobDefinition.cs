using System;
using System.Collections.Generic;

using Jobbr.Server.Model;

namespace Jobbr.Server.Configuration
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

            this.triggers.Add(new ScheduledTrigger() { TriggerType = ScheduledTrigger.TypeName, StartDateTimeUtc = startDateTimeUtc });
            
            return this;
        }

        public JobDefinition WithTrigger(string cronDefinition, DateTime? validFromDateTimeUtc = null, DateTime? validToDateTimeUtc = null)
        {
            this.hasTriggerDefinition = true;

            NCrontab.CrontabSchedule.Parse(cronDefinition);

            this.triggers.Add(new RecurringTrigger { TriggerType = RecurringTrigger.TypeName, StartDateTimeUtc = validFromDateTimeUtc, EndDateTimeUtc = validToDateTimeUtc, Definition = cronDefinition });

            return this;
        }

    }
}