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
        private readonly ITriggerService _triggerService;
        private readonly IJobService _jobService;
        private readonly IJobRunService _jobRunService;

        private readonly IMapper _mapper;

        public JobManagementService(ITriggerService triggerService, IJobService jobService, IJobRunService jobRunService, IMapper mapper)
        {
            _triggerService = triggerService;
            _jobService = jobService;
            _jobRunService = jobRunService;
            _mapper = mapper;
        }

        public void AddJob(Job job)
        {
            var model = _mapper.Map<JobModel>(job);

            var newJOb = _jobService.Add(model);
            job.Id = newJOb.Id;
        }

        public void UpdateJob(Job job)
        {
            var model = _mapper.Map<JobModel>(job);

            _jobService.Update(model);
        }

        public void DeleteJob(long jobId)
        {
            // TODO: implement :)
            // - terminate job if it is running
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            var model = _mapper.Map<RecurringTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            var model = _mapper.Map<ScheduledTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            var model = _mapper.Map<InstantTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            _triggerService.Disable(jobId, triggerId);
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            _triggerService.Enable(jobId, triggerId);
        }

        public void DeleteTrigger(long jobId, long triggerId)
        {
            _triggerService.Delete(jobId, triggerId);
        }

        public void UpdateTriggerDefinition(long jobId, long triggerId, string definition)
        {
            _triggerService.Update(jobId, triggerId, definition);
        }

        public void Update(RecurringTrigger trigger)
        {
            var triggerModel = _mapper.Map<RecurringTriggerModel>(trigger);

            _triggerService.Update(triggerModel);
        }

        public void Update(ScheduledTrigger trigger)
        {
            var triggerModel = _mapper.Map<ScheduledTriggerModel>(trigger);

            _triggerService.Update(triggerModel);
        }

        public void UpdateTriggerStartTime(long jobId, long triggerId, DateTime startDateTimeUtc)
        {
            _triggerService.Update(jobId, triggerId, startDateTimeUtc);
        }

        public void DeleteJobRun(long jobRunId)
        {
            _jobRunService.Delete(jobRunId);
        }

        public List<JobArtefact> GetArtefactForJob(long jobRunId)
        {
            var artefacts = _jobRunService.GetArtefactsByJobRunId(jobRunId);

            return _mapper.Map<List<JobArtefact>>(artefacts);
        }

        public Stream GetArtefactAsStream(long jobRunId, string filename)
        {
            return _jobRunService.GetArtefactAsStream(jobRunId, filename);
        }
    }
}