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
        private readonly JobRunService jobRunService;

        private readonly IMapper mapper;

        public JobManagementService(TriggerService triggerService, JobService jobService, JobRunService jobRunService, IMapper mapper)
        {
            this.triggerService = triggerService;
            this.jobService = jobService;
            this.jobRunService = jobRunService;
            this.mapper = mapper;
        }

        public void AddJob(Job job)
        {
            var model = this.mapper.Map<JobModel>(job);

            var newJOb = this.jobService.Add(model);
            job.Id = newJOb.Id;
        }

        public void UpdateJob(Job job)
        {
            var model = this.mapper.Map<JobModel>(job);

            this.jobService.Update(model);
        }

        public void DeleteJob(long jobId)
        {
            // TODO: implement :)
            // - terminate job if it is running
            
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            var model = this.mapper.Map<RecurringTriggerModel>(trigger);

            this.triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            var model = this.mapper.Map<ScheduledTriggerModel>(trigger);

            this.triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            var model = this.mapper.Map<InstantTriggerModel>(trigger);

            this.triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            this.triggerService.Disable(jobId, triggerId);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            this.triggerService.Enable(jobId, triggerId);
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
            this.triggerService.Delete(jobId, triggerId);
        }

        public void UpdateTriggerDefinition(long jobId, long triggerId, string definition)
        {
            this.triggerService.Update(jobId, triggerId, definition);
        }

        public void Update(RecurringTrigger trigger)
        {
            var triggerModel = this.mapper.Map<RecurringTriggerModel>(trigger);

            this.triggerService.Update(triggerModel);
        }

        public void Update(ScheduledTrigger trigger)
        {
            var triggerModel = this.mapper.Map<ScheduledTriggerModel>(trigger);

            this.triggerService.Update(triggerModel);
        }

        public void UpdateTriggerStartTime(long jobId, long triggerId, DateTime startDateTimeUtc)
        {
            this.triggerService.Update(jobId, triggerId, startDateTimeUtc);
        }

        public void DeleteJobRun(long jobRunId)
        {
            this.jobRunService.Delete(jobRunId);
        }

        public List<JobArtefact> GetArtefactForJob(long jobRunId)
        {
            var artefacts = this.jobRunService.GetArtefactsByJobRunId(jobRunId);

            return this.mapper.Map<List<JobArtefact>>(artefacts);
        }

        public Stream GetArtefactAsStream(long jobRunId, string filename)
        {
            return this.jobRunService.GetArtefactAsStream(jobRunId, filename);
        }
    }
}