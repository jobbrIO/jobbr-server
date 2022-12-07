using System;
using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server.JobRegistry
{
    /// <summary>
    /// Builds a registry that contains jobs, job descriptions and their triggers.
    /// </summary>
    public class RegistryBuilder : IRegistryBuilder
    {
        private readonly ILogger<RegistryBuilder> _logger;
        private bool _isSingleSourceOfTruth;

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistryBuilder"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger.</param>
        public RegistryBuilder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<RegistryBuilder>();
        }

        /// <summary>
        /// If registry builder has a configuration.
        /// </summary>
        internal bool HasConfiguration { get; private set; }

        /// <summary>
        /// Accessor for <see cref="JobDefinition"/>s.
        /// </summary>
        internal List<JobDefinition> Definitions { get; } = new ();

        /// <inheritdoc/>
        public RegistryBuilder RemoveAll()
        {
            HasConfiguration = true;

            return this;
        }

        /// <inheritdoc/>
        public RegistryBuilder AsSingleSourceOfTruth()
        {
            _isSingleSourceOfTruth = true;
            return this;
        }

        /// <inheritdoc/>
        public JobDefinition Define(Type jobType, int maxConcurrentJobRuns = 0)
        {
            if (jobType == null)
            {
                throw new ArgumentException("Job Type can't be null.");
            }

            return Define(jobType.Name, jobType.FullName, maxConcurrentJobRuns);
        }

        /// <inheritdoc/>
        public JobDefinition Define(string uniqueName, string typeName, int maxConcurrentJobRuns = 0)
        {
            var existing = Definitions.FirstOrDefault(d => string.Equals(d.UniqueName, uniqueName, StringComparison.OrdinalIgnoreCase));

            if (existing != null)
            {
                existing.ClrType = typeName;
                return existing;
            }

            var definition = new JobDefinition() { UniqueName = uniqueName, ClrType = typeName, MaxConcurrentJobRuns = maxConcurrentJobRuns };
            Definitions.Add(definition);

            HasConfiguration = true;

            return definition;
        }

        /// <inheritdoc/>
        public int Apply(IJobStorageProvider storage)
        {
            var numberOfChanges = 0;

            if (!HasConfiguration)
            {
                return numberOfChanges;
            }

            // Deactivate non existent
            if (_isSingleSourceOfTruth)
            {
                numberOfChanges += SoftDeleteOldJobsAndTriggers(storage);
            }

            foreach (var jobDefinition in Definitions)
            {
                var existingJob = storage.GetJobByUniqueName(jobDefinition.UniqueName);

                if (existingJob == null)
                {
                    // Add new Job
                    _logger.LogInformation("Adding job '{name}' of type '{clrType}'", jobDefinition.UniqueName, jobDefinition.ClrType);
                    var job = new Job
                    {
                        UniqueName = jobDefinition.UniqueName,
                        Type = jobDefinition.ClrType,
                        Parameters = jobDefinition.Parameter,
                        MaxConcurrentJobRuns = jobDefinition.MaxConcurrentJobRuns,
                    };
                    storage.AddJob(job);

                    foreach (var trigger in jobDefinition.Triggers)
                    {
                        AddTrigger(storage, trigger, jobDefinition, job.Id);
                        numberOfChanges++;
                    }
                }
                else
                {
                    existingJob.Deleted = false;
                    existingJob.MaxConcurrentJobRuns = jobDefinition.MaxConcurrentJobRuns;

                    // Update existing Jobs and triggers
                    _logger.LogInformation("Updating type for Job '{name}' (Id: '{id}') from '{type}' to '{clrType}'", existingJob.UniqueName, existingJob.Id, existingJob.Type, jobDefinition.ClrType);
                    existingJob.Type = jobDefinition.ClrType;
                    existingJob.Parameters = jobDefinition.Parameter;

                    storage.Update(existingJob);

                    numberOfChanges++;

                    if (jobDefinition.HasTriggerDefinition)
                    {
                        // Setup triggers
                        var activeTriggers = storage.GetTriggersByJobId(existingJob.Id, 1, int.MaxValue).Items.Where(t => t.IsActive).ToList();
                        var toDeactivateTriggers = new List<JobTriggerBase>(activeTriggers.Where(t => !(t is InstantTrigger)));

                        if (jobDefinition.Triggers.Any())
                        {
                            _logger.LogInformation("Job '{name}' has {triggerCount} triggers explicitly specified by definition. Going to apply the TriggerDefinition to the actual storage provider.", existingJob.UniqueName, jobDefinition.Triggers.Count);
                        }

                        // Update or add new ones
                        foreach (var trigger in jobDefinition.Triggers)
                        {
                            var existingOne = activeTriggers.FirstOrDefault(t => t.IsTriggerEqual(trigger));

                            if (existingOne == null)
                            {
                                // Add one
                                AddTrigger(storage, trigger, jobDefinition, existingJob.Id);
                                _logger.LogInformation("Added trigger (type: '{typeName}' to job '{jobName}' (JobId: '{id}')'", trigger.GetType().Name, jobDefinition.UniqueName, trigger.Id);

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
                            _logger.LogInformation("Deactivating trigger (type: '{typeName}' to job '{name}' (JobId: '{id}')'", trigger.GetType().Name, jobDefinition.UniqueName, trigger.Id);
                            storage.DisableTrigger(existingJob.Id, trigger.Id);
                            numberOfChanges++;
                        }
                    }
                }
            }

            return numberOfChanges;
        }

        private static IList<Job> GetUndefinedJobs(ICollection<string> allDefinedJobs, IEnumerable<Job> allJobsInStorage)
        {
            return allJobsInStorage.Where(job => !allDefinedJobs.Contains(job.UniqueName)).ToList();
        }

        private static int OmitJobRunsFromJob(IJobStorageProvider storage, Job undefinedJob)
        {
            var numberOfChanges = 0;
            foreach (var jobRun in storage.GetJobRunsByJobId((int)undefinedJob.Id, pageSize: int.MaxValue).Items)
            {
                jobRun.Deleted = true;
                jobRun.State = JobRunStates.Omitted;
                storage.Update(jobRun);
                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private int SoftDeleteTriggers(IJobStorageProvider storage, IEnumerable<JobTriggerBase> triggersOfJob)
        {
            var numberOfChanges = 0;
            foreach (var trigger in triggersOfJob)
            {
                DeleteAndDeactivateTrigger(storage, trigger);
                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private void AddTrigger(IJobStorageProvider storage, JobTriggerBase trigger, JobDefinition jobDef, long jobId)
        {
            trigger.IsActive = true;
            trigger.JobId = jobId;
            trigger.Parameters = trigger.Parameters;

            _logger.LogInformation("Adding trigger (type: '{typeName}' to job '{jobName}' (JobId: '{id}')", trigger.GetType().Name, jobDef.UniqueName, jobId);

            switch (trigger)
            {
                case ScheduledTrigger scheduledTrigger:
                    storage.AddTrigger(jobId, scheduledTrigger);
                    break;
                case RecurringTrigger recurringTrigger:
                    storage.AddTrigger(jobId, recurringTrigger);
                    break;
            }
        }

        private IEnumerable<JobTriggerBase> GetTriggersOfJobs(IEnumerable<long> jobIds, IJobStorageProvider storage)
        {
            var triggers = new List<JobTriggerBase>();
            foreach (var jobId in jobIds)
            {
                triggers.AddRange(storage.GetTriggersByJobId(jobId, pageSize: int.MaxValue).Items);
            }

            _logger.LogInformation("{triggerCount} are in the storage but not defined.", triggers.Count);
            return triggers;
        }

        private int SoftDeleteJobs(IJobStorageProvider storage, IList<Job> undefinedJobs)
        {
            var numberOfChanges = 0;
            foreach (var undefinedJob in undefinedJobs)
            {
                undefinedJob.Deleted = true;
                _logger.LogInformation("Deleting job ({name}) with the id: {id}", undefinedJob.UniqueName, undefinedJob.Id);
                storage.Update(undefinedJob);
                numberOfChanges = OmitJobRunsFromJob(storage, undefinedJob);
                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private void DeleteAndDeactivateTrigger(IJobStorageProvider storage, JobTriggerBase trigger)
        {
            _logger.LogInformation("Deleting trigger with the id: {id}", trigger.Id);
            trigger.Deleted = true;
            trigger.IsActive = false;
            storage.Update(trigger.JobId, trigger as dynamic);
        }

        private int SoftDeleteOldJobsAndTriggers(IJobStorageProvider storage)
        {
            var allDefinedUniqueJobNames = Definitions.Select(d => d.UniqueName).ToList();
            var allJobsInStorage = storage.GetJobs(pageSize: int.MaxValue).Items;

            var numberOfChanges = 0;
            var undefinedJobs = GetUndefinedJobs(allDefinedUniqueJobNames, allJobsInStorage);
            numberOfChanges += SoftDeleteJobs(storage, undefinedJobs);

            var triggersOfJob = GetTriggersOfJobs(undefinedJobs.Select(j => j.Id), storage);
            numberOfChanges += SoftDeleteTriggers(storage, triggersOfJob);

            numberOfChanges += SoftDeleteOrphanedTriggers(storage);

            return numberOfChanges;
        }

        private int SoftDeleteOrphanedTriggers(IJobStorageProvider storage)
        {
            var triggersInDefinition = Definitions.SelectMany(d => d.Triggers).ToList();
            var currentTriggersInStorage = GetTriggersFromActiveJobs(storage);

            var orphanedTriggers = currentTriggersInStorage.Except(triggersInDefinition, new TriggerComparer()).ToList();
            var numberOfChanges = 0;

            foreach (var orphanedTrigger in orphanedTriggers)
            {
                DeleteAndDeactivateTrigger(storage, orphanedTrigger);
                var jobRuns = storage.GetJobRunsByTriggerId(orphanedTrigger.JobId, orphanedTrigger.Id, pageSize: int.MaxValue).Items;
                foreach (var jobRunToOmit in jobRuns)
                {
                    jobRunToOmit.Deleted = true;
                    jobRunToOmit.State = JobRunStates.Omitted;
                    storage.Update(jobRunToOmit);
                    numberOfChanges++;
                }

                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private IEnumerable<JobTriggerBase> GetTriggersFromActiveJobs(IJobStorageProvider storage)
        {
            var triggersFromActiveJobs = new List<JobTriggerBase>();
            foreach (var uniqueName in Definitions.Select(d => d.UniqueName))
            {
                var job = storage.GetJobByUniqueName(uniqueName);
                if (job != null)
                {
                    triggersFromActiveJobs.AddRange(storage.GetTriggersByJobId(job.Id, pageSize: int.MaxValue).Items);
                }
            }

            return triggersFromActiveJobs;
        }

        private class TriggerComparer : IEqualityComparer<JobTriggerBase>
        {
            public bool Equals(JobTriggerBase x, JobTriggerBase y)
            {
                return x.IsTriggerEqual(y);
            }

            public int GetHashCode(JobTriggerBase obj)
            {
                return 0;
            }
        }
    }
}