using System;
using System.Collections.Generic;
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

        private bool isSingleSourceOfTruth;

        internal bool HasConfiguration { get; private set; }

        internal List<JobDefinition> Definitions => this.definitions;

        public RegistryBuilder RemoveAll()
        {
            this.HasConfiguration = true;

            return this;
        }

        // TODO: Make internal behaviour visible by splitting explicitly to differnt flags
        // Area                                 Old                                 Additional
        // JobNotExistentAnyMoreBehaviour      Nothing                              SoftDelete Job & Triggers & FutureRuns
        // TriggerNotExistentAnyMoreBehavior   Deactivate +(Bug kept FutureRuns)    SoftDelete & FutureRuns

        public RegistryBuilder AsSingleSourceOfTruth() // RemoveUndefined?
        {
            this.isSingleSourceOfTruth = true;
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

        // TODO: Refactor in extension method to isolate actual logic from definition datastructure
        internal int Apply(IJobStorageProvider storage)
        {
            var numberOfChanges = 0;

            if (!this.HasConfiguration)
            {
                return numberOfChanges;
            }

            // TODO: Create list of active jobs in DB to gain difference for soft delete them if required
            var jobsToDeactivate = storage.GetJobs(1, Int32.MaxValue).Items;

            foreach (var jobDefinition in this.Definitions)
            {
                var existingJob = storage.GetJobByUniqueName(jobDefinition.UniqueName);

                if (existingJob == null)
                {
                    numberOfChanges = AddNewJob(storage, jobDefinition, numberOfChanges);
                }
                else
                {
                    numberOfChanges = UpdateExisting(storage, existingJob, jobDefinition, numberOfChanges);
                }

                // TODO: Remove the ones that are either new or updated
                jobsToDeactivate.Remove()
            }

            // TODO: Handle jobs that are not existent anymore
            if (jobsToDeactivate.Any())
            {
                // Pass jobsToDeactivate
                numberOfChanges += this.HandleNonExistingJobs(storage, numberOfChanges);
            }

            return numberOfChanges;
        }

        private int HandleNonExistingJobs(IJobStorageProvider storage, int numberOfChanges)
        {
            // Deactivate non existent
            if (this.isSingleSourceOfTruth)
            {
                numberOfChanges += this.SoftDeleteOldJobsAndTriggers(storage);
            }

            return numberOfChanges;
        }

        private static int UpdateExisting(IJobStorageProvider storage, Job existingJob, JobDefinition jobDefinition,
            int numberOfChanges)
        {
            existingJob.Deleted = false;
            storage.Update(existingJob);
            // Update existing Jobs and triggers
            if (!string.Equals(existingJob.Type, jobDefinition.ClrType, StringComparison.OrdinalIgnoreCase))
            {
                Logger.InfoFormat("Updating type for Job '{0}' (Id: '{1}') from '{2}' to '{2}'", existingJob.UniqueName,
                    existingJob.Id, existingJob.Type, jobDefinition.ClrType);
                existingJob.Type = jobDefinition.ClrType;
                existingJob.Parameters = jobDefinition.Parameter;

                storage.Update(existingJob);

                numberOfChanges++;
            }

            if (jobDefinition.HasTriggerDefinition)
            {
                // Setup triggers
                var activeTriggers = storage.GetTriggersByJobId(existingJob.Id, 1, int.MaxValue).Items.Where(t => t.IsActive)
                    .ToList();
                var toDeactivateTriggers = new List<JobTriggerBase>(activeTriggers.Where(t => !(t is InstantTrigger)));

                if (jobDefinition.Triggers.Any())
                {
                    Logger.InfoFormat(
                        "Job '{0}' has {1} triggers explicitly specified by definition. Going to apply the TriggerDefinition to the actual storage provider.",
                        existingJob.UniqueName, jobDefinition.Triggers.Count);
                }

                // Update or add new ones
                foreach (var trigger in jobDefinition.Triggers)
                {
                    var existingOne = activeTriggers.FirstOrDefault(t => t.IsTriggerEqual(trigger));

                    if (existingOne == null)
                    {
                        // Add one
                        AddTrigger(storage, trigger, jobDefinition, existingJob.Id);
                        Logger.InfoFormat("Added trigger (type: '{0}' to job '{1}' (JobId: '{2}')'", trigger.GetType().Name,
                            jobDefinition.UniqueName, trigger.Id);

                        numberOfChanges++;
                    }
                    else
                    {
                        toDeactivateTriggers.Remove(existingOne);
                    }
                }

                // Deactivate not yet defined triggers
                foreach (var trigger in toDeactivateTriggers)
                {
                    Logger.InfoFormat("Deactivating trigger (type: '{0}' to job '{1}' (JobId: '{2}')'", trigger.GetType().Name,
                        jobDefinition.UniqueName, trigger.Id);
                    storage.DisableTrigger(existingJob.Id, trigger.Id);
                    numberOfChanges++;

                    // TODO: Disable future Jobruns
                    // TODO: If Master is Repository, SoftDelete Trigger instead of disabling
                }
            }

            return numberOfChanges;
        }

        private static int AddNewJob(IJobStorageProvider storage, JobDefinition jobDefinition, int numberOfChanges)
        {
            // Add new Job
            Logger.InfoFormat("Adding job '{0}' of type '{1}'", jobDefinition.UniqueName, jobDefinition.ClrType);
            var job = new Job
                {UniqueName = jobDefinition.UniqueName, Type = jobDefinition.ClrType, Parameters = jobDefinition.Parameter};
            storage.AddJob(job);

            foreach (var trigger in jobDefinition.Triggers)
            {
                AddTrigger(storage, trigger, jobDefinition, job.Id);
                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private static void AddTrigger(IJobStorageProvider storage, JobTriggerBase trigger, JobDefinition jobDef, long jobId)
        {
            trigger.IsActive = true;
            trigger.JobId = jobId;
            trigger.Parameters = trigger.Parameters;

            Logger.InfoFormat("Adding trigger (type: '{0}' to job '{1}' (JobId: '{2}')", trigger.GetType().Name, jobDef.UniqueName, jobId);

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

        private static IList<Job> GetUndefinedJobs(ICollection<string> allDefinedJobs, IEnumerable<Job> allJobsInStorage)
        {
            return allJobsInStorage.Where(job => !allDefinedJobs.Contains(job.UniqueName)).ToList();
        }

        private static IList<JobTriggerBase> GetTriggersOfJobs(IEnumerable<long> jobIds, IJobStorageProvider storage)
        {
            var triggers = new List<JobTriggerBase>();
            foreach (var jobId in jobIds)
            {
                triggers.AddRange(storage.GetTriggersByJobId(jobId, pageSize: int.MaxValue).Items);
            }

            Logger.InfoFormat($"{triggers.Count} are in the storage but not defined.");
            return triggers;
        }

        private static int SoftDeleteJobs(IJobStorageProvider storage, IList<Job> undefinedJobs)
        {
            var numberOfChanges = 0;
            foreach (var undefinedJob in undefinedJobs)
            {
                undefinedJob.Deleted = true;
                Logger.InfoFormat($"Deleting job ({undefinedJob.UniqueName}) with the id: {undefinedJob.Id}");
                storage.Update(undefinedJob);
                numberOfChanges = OmitJobRunsFromJob(storage, undefinedJob);
                numberOfChanges++;
            }

            return numberOfChanges;
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

        private static int SoftDeleteTriggers(IJobStorageProvider storage, IList<JobTriggerBase> triggersOfJob)
        {
            var numberOfChanges = 0;
            foreach (var trigger in triggersOfJob)
            {
                DeleteAndDeactivateTrigger(storage, trigger);
                numberOfChanges++;
            }

            return numberOfChanges;
        }

        private static void DeleteAndDeactivateTrigger(IJobStorageProvider storage, JobTriggerBase trigger)
        {
            Logger.InfoFormat($"Deleting trigger with the id: {trigger.Id}");
            trigger.Deleted = true;
            trigger.IsActive = false;
            storage.Update(trigger.JobId, trigger as dynamic);
        }

        private int SoftDeleteOldJobsAndTriggers(IJobStorageProvider storage)
        {
            var allDefinedUniqueJobNames = this.Definitions.Select(d => d.UniqueName).ToList();
            var allJobsInStorage = storage.GetJobs(pageSize: int.MaxValue).Items;

            var numberOfChanges = 0;
            var undefinedJobs = GetUndefinedJobs(allDefinedUniqueJobNames, allJobsInStorage);
            numberOfChanges += SoftDeleteJobs(storage, undefinedJobs);

            var triggersOfJob = GetTriggersOfJobs(undefinedJobs.Select(j => j.Id), storage);
            numberOfChanges += SoftDeleteTriggers(storage, triggersOfJob);

            numberOfChanges += this.SoftDeleteOrphanedTriggers(storage);

            return numberOfChanges;
        }

        private int SoftDeleteOrphanedTriggers(IJobStorageProvider storage)
        {
            var triggersInDefinition = this.Definitions.SelectMany(d => d.Triggers).ToList();
            var currentTriggersInStorage = this.GetTriggersFromActiveJobs(storage);

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

        private IList<JobTriggerBase> GetTriggersFromActiveJobs(IJobStorageProvider storage)
        {
            var triggersFromActiveJobs = new List<JobTriggerBase>();
            foreach (var uniqueName in this.Definitions.Select(d => d.UniqueName))
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