using System;
using System.Collections.Generic;
using System.Text.Json;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.JobRegistry
{
    /// <summary>
    /// Definition for a <see cref="Job"/>.
    /// </summary>
    public class JobDefinition
    {
        private readonly List<JobTriggerBase> _triggers = new List<JobTriggerBase>();
        private bool _hasTriggerDefinition;

        /// <summary>
        /// If trigger for the job has been defined.
        /// </summary>
        public bool HasTriggerDefinition => _hasTriggerDefinition;

        /// <summary>
        /// Determines how many <see cref="Job"/>s can be run at the same time.
        /// </summary>
        public int MaxConcurrentJobRuns { get; set; }

        /// <summary>
        /// Unique name for the <see cref="Job"/>.
        /// </summary>
        internal string UniqueName { get; set; }

        /// <summary>
        /// CLR type for the <see cref="Job"/>.
        /// </summary>
        internal string ClrType { get; set; }

        /// <summary>
        /// Parameter.
        /// </summary>
        internal string Parameter { get; set; }

        /// <summary>
        /// List of triggers for the <see cref="Job"/>.
        /// </summary>
        internal List<JobTriggerBase> Triggers => _triggers;

        /// <summary>
        /// Adds a trigger to the <see cref="JobDefinition"/>.
        /// </summary>
        /// <param name="startDateTimeUtc">Trigger start time.</param>
        /// <param name="parameters">Parameters for the trigger.</param>
        /// <param name="userId">User ID.</param>
        /// <param name="userDisplayName">User display name.</param>
        /// <returns>The resulting <see cref="JobDefinition"/>.</returns>
        public JobDefinition WithTrigger(DateTime startDateTimeUtc, object parameters = null, string userId = null, string userDisplayName = null)
        {
            _hasTriggerDefinition = true;

            string parametersAsJson = null;

            if (parameters != null)
            {
                parametersAsJson = JsonSerializer.Serialize(parameters);
            }

            _triggers.Add(new ScheduledTrigger { StartDateTimeUtc = startDateTimeUtc, Parameters = parametersAsJson, UserId = userId, UserDisplayName = userDisplayName });

            return this;
        }

        /// <summary>
        /// Adds a trigger to the <see cref="JobDefinition"/>.
        /// </summary>
        /// <param name="cronDefinition">CRON time definition for the trigger.</param>
        /// <param name="validFromDateTimeUtc">Validity start time.</param>
        /// <param name="validToDateTimeUtc">Validity end time.</param>
        /// <param name="noParallelExecution">If parallel execution is allowed.</param>
        /// <param name="parameters">Parameters for the trigger.</param>
        /// <param name="userId">User ID.</param>
        /// <param name="userDisplayName">User display name.</param>
        /// <returns>The resulting <see cref="JobDefinition"/>.</returns>
        public JobDefinition WithTrigger(string cronDefinition, DateTime? validFromDateTimeUtc = null, DateTime? validToDateTimeUtc = null, bool noParallelExecution = false, object parameters = null, string userId = null, string userDisplayName = null)
        {
            _hasTriggerDefinition = true;

            NCrontab.CrontabSchedule.Parse(cronDefinition);

            string parametersAsJson = null;

            if (parameters != null)
            {
                parametersAsJson = JsonSerializer.Serialize(parameters);
            }

            _triggers.Add(new RecurringTrigger
            {
                StartDateTimeUtc = validFromDateTimeUtc,
                EndDateTimeUtc = validToDateTimeUtc,
                Definition = cronDefinition,
                NoParallelExecution = noParallelExecution,
                Parameters = parametersAsJson,
                UserId = userId,
                UserDisplayName = userDisplayName
            });

            return this;
        }

        /// <summary>
        /// Adds a parameter to a <see cref="JobDefinition"/>.
        /// </summary>
        /// <param name="parameter">The parameter that is being added.</param>
        /// <returns><see cref="JobDefinition"/> with the new parameter.</returns>
        public JobDefinition WithParameter(object parameter)
        {
            Parameter = JsonSerializer.Serialize(parameter);

            return this;
        }
    }
}