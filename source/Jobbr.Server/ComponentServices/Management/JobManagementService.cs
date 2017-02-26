using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Models;

namespace Jobbr.Server.ComponentServices.Management
{
    /// <summary>
    /// Implementation of the IJobManagementService as defined by the Management Component Model.
    /// </summary>
    internal class JobManagementService : IJobManagementService
    {
        private readonly TriggerService triggerService;
        private readonly JobService jobService;

        private readonly IMapper mapper;

        public JobManagementService(TriggerService triggerService, JobService jobService, IMapper mapper)
        {
            this.triggerService = triggerService;
            this.jobService = jobService;
            this.mapper = mapper;
        }

        public Job AddJob(Job job)
        {
            var model = this.mapper.Map<JobModel>(job);

            var newJOb = this.jobService.Add(model);
            job.Id = newJOb.Id;

            return job;
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            var model = this.mapper.Map<RecurringTriggerModel>(trigger);

            var id = this.triggerService.Add(model);
            trigger.Id = id;

            return trigger.Id;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            var model = this.mapper.Map<ScheduledTriggerModel>(trigger);

            var id = this.triggerService.Add(model);
            trigger.Id = id;

            return trigger.Id;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            var model = this.mapper.Map<InstantTriggerModel>(trigger);

            var id = this.triggerService.Add(model);
            trigger.Id = id;

            return trigger.Id;
        }

        public bool DisableTrigger(long triggerId)
        {
            return this.triggerService.Disable(triggerId);
        }

        public bool EnableTrigger(long triggerId)
        {
            return this.triggerService.Enable(triggerId);
        }

        public void UpdateTriggerDefinition(long triggerId, string definition)
        {
            this.triggerService.Update(triggerId, definition);
        }

        public void UpdateTriggerStartTime(long triggerId, DateTime startDateTimeUtc)
        {
            this.triggerService.Update(triggerId, startDateTimeUtc);
        }

        public List<JobArtefact> GetArtefactForJob(Guid jobRunUniqueId)
        {
            throw new NotImplementedException();
        }

        public Stream GetArtefactAsStream(Guid jobRunUniqueId, string filename)
        {
            throw new NotImplementedException();
        }
    }
}