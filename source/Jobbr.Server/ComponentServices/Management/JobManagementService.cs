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

        /// <summary>
        /// Initializes a new instance of the <see cref="JobManagementService"/> class.
        /// </summary>
        /// <param name="triggerService">Job trigger service.</param>
        /// <param name="jobService">Job service.</param>
        /// <param name="jobRunService">Job run service.</param>
        /// <param name="mapper">The mapper.</param>
        public JobManagementService(ITriggerService triggerService, IJobService jobService, IJobRunService jobRunService, IMapper mapper)
        {
            _triggerService = triggerService;
            _jobService = jobService;
            _jobRunService = jobRunService;
            _mapper = mapper;
        }

        /// <summary>
        /// Add new <see cref="Job"/>.
        /// </summary>
        /// <param name="job"><see cref="Job"/> to add.</param>
        public void AddJob(Job job)
        {
            var model = _mapper.Map<JobModel>(job);

            var newJOb = _jobService.Add(model);
            job.Id = newJOb.Id;
        }

        /// <summary>
        /// Update <see cref="Job"/>.
        /// </summary>
        /// <param name="job"><see cref="Job"/> to update.</param>
        public void UpdateJob(Job job)
        {
            var model = _mapper.Map<JobModel>(job);

            _jobService.Update(model);
        }

        /// <summary>
        /// Delete <see cref="Job"/>. Unimplemented.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <exception cref="NotImplementedException">Throws always.</exception>
        public void DeleteJob(long jobId)
        {
            // TODO: implement :)
            // - terminate job if it is running
            throw new NotImplementedException();
        }

        /// <summary>
        /// Add a recurring job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="trigger">The <see cref="RecurringTrigger"/>.</param>
        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            var model = _mapper.Map<RecurringTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        /// <summary>
        /// Add a scheduled job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="trigger">The <see cref="ScheduledTrigger"/>.</param>
        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            var model = _mapper.Map<ScheduledTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        /// <summary>
        /// Add an instant job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="trigger">The <see cref="InstantTrigger"/>.</param>
        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            var model = _mapper.Map<InstantTriggerModel>(trigger);

            _triggerService.Add(jobId, model);

            trigger.Id = model.Id;
            trigger.JobId = jobId;
        }

        /// <summary>
        /// Disable job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="triggerId">The trigger ID.</param>
        public void DisableTrigger(long jobId, long triggerId)
        {
            _triggerService.Disable(jobId, triggerId);
        }

        /// <summary>
        /// Enable job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="triggerId">The trigger ID.</param>
        public void EnableTrigger(long jobId, long triggerId)
        {
            _triggerService.Enable(jobId, triggerId);
        }

        /// <summary>
        /// Delete job trigger.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="triggerId">The trigger ID.</param>
        public void DeleteTrigger(long jobId, long triggerId)
        {
            _triggerService.Delete(jobId, triggerId);
        }

        /// <summary>
        /// Update trigger definition.
        /// </summary>
        /// <param name="jobId">Target <see cref="Job"/> ID.</param>
        /// <param name="triggerId">The trigger ID.</param>
        /// <param name="definition">New definition.</param>
        public void UpdateTriggerDefinition(long jobId, long triggerId, string definition)
        {
            _triggerService.Update(jobId, triggerId, definition);
        }

        /// <summary>
        /// Update <see cref="RecurringTrigger"/>.
        /// </summary>
        /// <param name="trigger">The target trigger.</param>
        public void Update(RecurringTrigger trigger)
        {
            var triggerModel = _mapper.Map<RecurringTriggerModel>(trigger);

            _triggerService.Update(triggerModel);
        }

        /// <summary>
        /// Update <see cref="ScheduledTrigger"/>.
        /// </summary>
        /// <param name="trigger">The target trigger.</param>
        public void Update(ScheduledTrigger trigger)
        {
            var triggerModel = _mapper.Map<ScheduledTriggerModel>(trigger);

            _triggerService.Update(triggerModel);
        }

        /// <summary>
        /// Update trigger start time.
        /// </summary>
        /// <param name="jobId">The target job ID.</param>
        /// <param name="triggerId">Target trigger ID.</param>
        /// <param name="startDateTimeUtc">The new start time in UTC.</param>
        public void UpdateTriggerStartTime(long jobId, long triggerId, DateTime startDateTimeUtc)
        {
            _triggerService.Update(jobId, triggerId, startDateTimeUtc);
        }

        /// <summary>
        /// Delete job run.
        /// </summary>
        /// <param name="jobRunId">Target job run ID.</param>
        public void DeleteJobRun(long jobRunId)
        {
            _jobRunService.Delete(jobRunId);
        }

        /// <summary>
        /// Get <see cref="JobArtefact"/>.
        /// </summary>
        /// <param name="jobRunId">Target job run ID.</param>
        /// <returns>List of <see cref="JobArtefact"/>s.</returns>
        public List<JobArtefact> GetArtefactForJob(long jobRunId)
        {
            var artefacts = _jobRunService.GetArtefactsByJobRunId(jobRunId);

            return _mapper.Map<List<JobArtefact>>(artefacts);
        }

        /// <summary>
        /// Get job artifact as a stream.
        /// </summary>
        /// <param name="jobRunId">Target job run ID.</param>
        /// <param name="filename">Filename.</param>
        /// <returns>A <see cref="Stream"/> of the artifact.</returns>
        public Stream GetArtefactAsStream(long jobRunId, string filename)
        {
            return _jobRunService.GetArtefactAsStream(jobRunId, filename);
        }
    }
}