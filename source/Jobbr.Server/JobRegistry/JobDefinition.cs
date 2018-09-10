using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.JobStorage.Model;
using Newtonsoft.Json;

namespace Jobbr.Server.JobRegistry
{
    public class JobDefinition
    {
        private readonly List<JobTriggerBase> triggers = new List<JobTriggerBase>();

        private bool hasTriggerDefinition;

        public bool HasTriggerDefinition => this.hasTriggerDefinition;

        internal string UniqueName { get; set; }

        internal string ClrType { get; set; }

        internal string Parameter { get; set; }

        internal List<JobTriggerBase> Triggers => this.triggers;

        public JobDefinition WithTrigger(DateTime startDateTimeUtc, object parameters = null, string userId = null, string userDisplayName = null)
        {
            this.hasTriggerDefinition = true;

            string parametersAsJson = null;

            if (parameters != null)
            {
                parametersAsJson = JsonConvert.SerializeObject(parameters);
            }

            this.triggers.Add(new ScheduledTrigger { StartDateTimeUtc = startDateTimeUtc, Parameters = parametersAsJson, UserId = userId, UserDisplayName = userDisplayName });

            return this;
        }

        public JobDefinition WithTrigger(string cronDefinition, DateTime? validFromDateTimeUtc = null, DateTime? validToDateTimeUtc = null, bool noParallelExecution = false, object parameters = null, string userId = null, string userDisplayName = null)
        {
            this.hasTriggerDefinition = true;

            NCrontab.CrontabSchedule.Parse(cronDefinition);

            string parametersAsJson = null;

            if (parameters != null)
            {
                parametersAsJson = JsonConvert.SerializeObject(parameters);
            }
            
            this.triggers.Add(new RecurringTrigger { StartDateTimeUtc = validFromDateTimeUtc, EndDateTimeUtc = validToDateTimeUtc, Definition = cronDefinition, NoParallelExecution = noParallelExecution, Parameters = parametersAsJson, UserId = userId, UserDisplayName = userDisplayName });

            return this;
        }

        public JobDefinition WithParameter(object parameter)
        {
            this.Parameter = JsonConvert.SerializeObject(parameter);

            return this;
        }
    }
}