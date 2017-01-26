using System;
using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Storage;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class JobManagementService : IJobManagementService
    {
        private readonly IJobbrRepository jobbrRepository;

        public JobManagementService(IJobbrRepository jobbrRepository)
        {
            this.jobbrRepository = jobbrRepository;
        }

        public Job AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            var triggerEntity = new ComponentModel.JobStorage.Model.RecurringTrigger()
            {
                JobId = trigger.JobId,
                Comment = trigger.Comment,
                Definition = trigger.Definition,
                // NoParallelExecution = trigger.
                Parameters = trigger.Parameters,
                IsActive = trigger.IsActive,
                StartDateTimeUtc = trigger.StartDateTimeUtc,
                EndDateTimeUtc = trigger.EndDateTimeUtc,
                UserDisplayName = trigger.UserDisplayName,
                UserId = trigger.UserId,
                UserName = trigger.UserName
            };

            this.jobbrRepository.SaveAddTrigger(triggerEntity);


            trigger.Id = triggerEntity.Id;

            return trigger.Id;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public bool DisableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public bool EnableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTriggerDefinition(long triggerId, string definition)
        {
            throw new NotImplementedException();
        }

        public void UpdatetriggerStartTime(long triggerId, DateTime startDateTimeUtc)
        {
            throw new NotImplementedException();
        }

        public List<JobArtefact> GetArtefactForJob(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public Stream GetArtefactAsStream(JobRun jobRun, string filename)
        {
            throw new NotImplementedException();
        }
    }
}