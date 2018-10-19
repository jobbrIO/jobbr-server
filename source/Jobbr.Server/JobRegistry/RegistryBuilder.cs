using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Logging;

namespace Jobbr.Server.JobRegistry
{
    public class RegistryBuilder
    {
        private static readonly ILog Logger = LogProvider.For<RegistryBuilder>();

        private readonly List<JobDefinition> definitions = new List<JobDefinition>();

        internal bool HasConfiguration { get; private set; }

        internal bool RemoveNotExisting { get; private set; }

        internal List<JobDefinition> Definitions => this.definitions;

        public RegistryBuilder RemoveAll()
        {
            this.HasConfiguration = true;

            return this;
        }

        public JobDefinition Define(Type jobType)
        {
            if (jobType == null)
            {
                throw new ArgumentException($"Job Type can't be null.");
            }

            return this.Define(jobType.Name, jobType.FullName);
        }

        public JobDefinition Define(string uniqueName, string typeName)
        {
            var existing = this.definitions.FirstOrDefault(d => string.Equals(d.UniqueName, uniqueName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.ClrType = typeName;
                return existing;
            }

            var definition = new JobDefinition() { UniqueName = uniqueName, ClrType = typeName };
            this.definitions.Add(definition);

            this.HasConfiguration = true;

            return definition;
        }

        internal int Apply(IJobStorageProvider storage)
        {
            var numberOfChanges = 0;

            if (this.HasConfiguration)
            {
                foreach (var jobDefinition in this.Definitions)
                {
                    var existingJob = storage.GetJobByUniqueName(jobDefinition.UniqueName);

                    if (existingJob == null)
                    {
                        // Add new Job
                        Logger.InfoFormat("Adding job '{0}' of type '{1}'", jobDefinition.UniqueName, jobDefinition.ClrType);
                        var job = new Job { UniqueName = jobDefinition.UniqueName, Type = jobDefinition.ClrType, Parameters = jobDefinition.Parameter };
                        storage.AddJob(job);

                        foreach (var trigger in jobDefinition.Triggers)
                        {
                            AddTrigger(storage, trigger, jobDefinition, job.Id);
                            numberOfChanges++;
                        }
                    }
                    else
                    {
                        // Update existing Jobs and triggers
                        if (!string.Equals(existingJob.Type, jobDefinition.ClrType, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.InfoFormat("Updating type for Job '{0}' (Id: '{1}') from '{2}' to '{2}'", existingJob.UniqueName, existingJob.Id, existingJob.Type, jobDefinition.ClrType);
                            existingJob.Type = jobDefinition.ClrType;
                            existingJob.Parameters = jobDefinition.Parameter;

                            storage.Update(existingJob);

                            numberOfChanges++;
                        }

                        if (jobDefinition.HasTriggerDefinition)
                        {
                            // Setup triggers
                            var job = storage.GetJobByUniqueName(jobDefinition.UniqueName);
                            var activeTriggers = storage.GetTriggersByJobId(job.Id, 1, int.MaxValue).Items.Where(t => t.IsActive).ToList();
                            var toDeactivateTriggers = new List<JobTriggerBase>(activeTriggers.Where(t => !(t is InstantTrigger)));

                            if (jobDefinition.Triggers.Any())
                            {
                                Logger.InfoFormat("Job '{0}' has {1} tiggers explicitly specified by definition. Going to apply the TriggerDefiniton to the actual storage provider.", existingJob.UniqueName, jobDefinition.Triggers.Count);
                            }

                            // Update or add new ones
                            foreach (var trigger in jobDefinition.Triggers)
                            {
                                var existingOne = activeTriggers.FirstOrDefault(t => this.IsSame(t as dynamic, trigger as dynamic));

                                if (existingOne == null)
                                {
                                    // Add one
                                    AddTrigger(storage, trigger, jobDefinition, job.Id);
                                    Logger.InfoFormat("Added trigger (type: '{0}' to job '{1}' (JobId: '{2}')'", trigger.GetType().Name, jobDefinition.UniqueName, trigger.Id);

                                    numberOfChanges++;
                                }
                                else
                                {
                                    toDeactivateTriggers.Remove(existingOne);
                                }
                            }

                            // Deactivate not specified triggers
                            foreach (var trigger in toDeactivateTriggers)
                            {
                                Logger.InfoFormat("Deactivating trigger (type: '{0}' to job '{1}' (JobId: '{2}')'", trigger.GetType().Name, jobDefinition.UniqueName, trigger.Id);
                                storage.DisableTrigger(existingJob.Id, trigger.Id);
                                numberOfChanges++;
                            }
                        }
                    }
                }

                // Deactivate non existent
            }

            return numberOfChanges;
        }

        private static void AddTrigger(IJobStorageProvider storage, JobTriggerBase trigger, JobDefinition jobDef, long jobId)
        {
            trigger.IsActive = true;
            trigger.JobId = jobId;
            trigger.Parameters = trigger.Parameters;

            Logger.InfoFormat("Adding trigger (type: '{0}' to job '{1}' (JobId: '{2}')", trigger.GetType().Name, jobDef.UniqueName, jobId);

            var scheduledTrigger = trigger as ScheduledTrigger;
            if (scheduledTrigger != null)
            {
                storage.AddTrigger(jobId, scheduledTrigger);
            }

            var recurringTrigger = trigger as RecurringTrigger;
            if (recurringTrigger != null)
            {
                storage.AddTrigger(jobId, recurringTrigger);
            }
        }

        [SuppressMessage("ReSharper", "UnusedParameter.Local", Justification = "Fallback for dynamic invocation.")]
        private bool IsSame(JobTriggerBase left, JobTriggerBase right)
        {
            return false;
        }

        private bool IsSame(ScheduledTrigger left, ScheduledTrigger right)
        {
            return left.StartDateTimeUtc == right.StartDateTimeUtc;
        }

        private bool IsSame(RecurringTrigger left, RecurringTrigger right)
        {
            return string.Equals(left.Definition, right.Definition, StringComparison.OrdinalIgnoreCase);
        }
    }
}