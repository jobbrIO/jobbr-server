using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Storage;
using IJobManagementService = Jobbr.ComponentModel.Management.IJobManagementService;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class JobManagementService : IJobManagementService
    {
        private readonly IJobbrRepository jobbrRepository;
        private readonly TriggerService triggerService;

        public JobManagementService(IJobbrRepository jobbrRepository, TriggerService triggerService)
        {
            this.jobbrRepository = jobbrRepository;
            this.triggerService = triggerService;
        }

        public Job AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            var model = new RecurringTriggerModel()
            {
                JobId = trigger.JobId,
                Comment = trigger.Comment,
                Definition = trigger.Definition,
                NoParallelExecution = trigger.NoParallelExecution,
                Parameters = trigger.Parameters,
                IsActive = trigger.IsActive,
                StartDateTimeUtc = trigger.StartDateTimeUtc,
                EndDateTimeUtc = trigger.EndDateTimeUtc,
                UserDisplayName = trigger.UserDisplayName,
                UserId = trigger.UserId,
                UserName = trigger.UserName
            };

            var id = this.triggerService.Add(model);
            trigger.Id = id;

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
            this.triggerService.Update(triggerId, definition);
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