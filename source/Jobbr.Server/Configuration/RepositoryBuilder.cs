using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Server.Model;

namespace Jobbr.Server.Configuration
{
    public class RepositoryBuilder
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogProvider.For<RepositoryBuilder>();

        private List<JobDefinition> definitions = new List<JobDefinition>();

        internal bool HasConfiguration { get; private set; }

        internal bool RemoveNonExistent { get; private set; }

        internal List<JobDefinition> Definitions
        {
            get
            {
                return this.definitions;
            }
        }

        public RepositoryBuilder RemoveAll()
        {
            this.HasConfiguration = true;
            this.RemoveNonExistent = true;

            return this;
        }

        public JobDefinition Define(string uniqueName, string typeName)
        {
            var existing = this.definitions.FirstOrDefault(d => d.UniqueName == uniqueName);

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
            var numnberOfChanges = 0;

            if (this.HasConfiguration)
            {
                foreach (var jobDef in this.Definitions)
                {
                    var existentJob = storage.GetJobByUniqueName(jobDef.UniqueName);

                    if (existentJob == null)
                    {
                        // Add new Job
                        Logger.InfoFormat("Adding job '{0}' of type '{1}'", jobDef.UniqueName, jobDef.ClrType);
                        var jobId = storage.AddJob(new Job() { UniqueName = jobDef.UniqueName, Type = jobDef.ClrType });

                        foreach (var trigger in jobDef.Triggers)
                        {
                            AddTrigger(storage, trigger, jobDef, jobId);
                            numnberOfChanges++;
                        }
                    }
                    else
                    {
                        // Update existing Jobs and triggers
                        if (existentJob.Type != jobDef.ClrType)
                        {
                            Logger.InfoFormat("Updating type for Job '{0}' (Id: '{1}') from '{2}' to '{2}'", existentJob.UniqueName, existentJob.Id, existentJob.Type, jobDef.ClrType);
                            existentJob.Type = jobDef.ClrType;

                            storage.Update(existentJob);

                            numnberOfChanges++;
                        }

                        // Setup triggers
                        var job = storage.GetJobByUniqueName(jobDef.UniqueName);
                        var activeTriggers = storage.GetTriggersByJobId(job.Id).Where(t => t.IsActive).ToList();
                        var toDeactivateTriggers = new List<JobTriggerBase>(activeTriggers.Where(t => !(t is InstantTrigger)));

                        // Update or add new ones
                        foreach (var trigger in jobDef.Triggers)
                        {
                            var existingOne = activeTriggers.FirstOrDefault(t => this.IsSame(t as dynamic, trigger as dynamic));

                            if (existingOne == null)
                            {
                                // Add one
                                AddTrigger(storage, trigger, jobDef, job.Id);
                                numnberOfChanges++;
                            }
                            else
                            {
                                // TODO: Update
                                toDeactivateTriggers.Remove(existingOne);
                            }
                        }


                        // Deactivate not specified triggers
                        foreach (var trigger in toDeactivateTriggers)
                        {
                            Logger.InfoFormat("Dactivating trigger (type: '{0}' to job '{1}' (JobId: '{2}')'", trigger.TriggerType, jobDef.UniqueName, trigger.Id);
                            storage.DisableTrigger(trigger.Id);
                            numnberOfChanges++;
                        }
                        
                    }

                }

                // Deactivate non existent
            }

            return numnberOfChanges;
        }

        private static void AddTrigger(IJobStorageProvider storage, JobTriggerBase trigger, JobDefinition jobDef, long jobId)
        {
            trigger.IsActive = true;
            trigger.JobId = jobId;

            Logger.InfoFormat("Adding trigger (type: '{0}' to job '{1}' (JobId: '{2}')", trigger.TriggerType, jobDef.UniqueName, jobId);

            if (trigger is ScheduledTrigger)
            {
                storage.AddTrigger((ScheduledTrigger)trigger);
            }

            if (trigger is RecurringTrigger)
            {
                storage.AddTrigger((RecurringTrigger)trigger);
            }
        }

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
            return left.Definition == right.Definition;
        }
    }
}