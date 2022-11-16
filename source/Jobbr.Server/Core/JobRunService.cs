using System;
using System.Collections.Generic;
using System.IO;
using AutoMapper;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Service for managing the job states, artifacts and IDs.
    /// </summary>
    public class JobRunService
    {
        private readonly ILogger<JobRunService> _logger;
        private readonly ITinyMessengerHub _messengerHub;
        private readonly IJobbrRepository _jobbrRepository;
        private readonly IArtefactsStorageProvider _artefactsStorageProvider;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunService"/> class.
        /// </summary>
        public JobRunService(ILogger<JobRunService> logger, ITinyMessengerHub messengerHub, IJobbrRepository jobbrRepository, IArtefactsStorageProvider artefactsStorageProvider, IMapper mapper)
        {
            _logger = logger;
            _messengerHub = messengerHub;
            _jobbrRepository = jobbrRepository;
            _artefactsStorageProvider = artefactsStorageProvider;
            _mapper = mapper;
        }

        /// <summary>
        /// Update the progress of a job.
        /// </summary>
        /// <param name="jobRunId">ID for the job run.</param>
        /// <param name="progress">How far the job has progressed.</param>
        public void UpdateProgress(long jobRunId, double progress)
        {
            _jobbrRepository.UpdateJobRunProgress(jobRunId, progress);
        }

        /// <summary>
        /// Update the state of a job.
        /// </summary>
        /// <param name="jobRunId">The ID of the job run.</param>
        /// <param name="state">The new state for the job.</param>
        public void UpdateState(long jobRunId, JobRunStates state)
        {
            _logger.LogInformation("[{jobRunId}] The JobRun with id: {jobRunId} has switched to the '{state}'-State", jobRunId, jobRunId, state);

            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);
            jobRun.State = _mapper.Map<ComponentModel.JobStorage.Model.JobRunStates>(state);

            if (state == JobRunStates.Started)
            {
                jobRun.ActualStartDateTimeUtc = DateTime.UtcNow;
            }

            if (state is JobRunStates.Completed or JobRunStates.Failed)
            {
                jobRun.ActualEndDateTimeUtc = DateTime.UtcNow;
            }

            _jobbrRepository.Update(jobRun);

            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                _messengerHub.Publish(new JobRunCompletedMessage(this) { Id = jobRunId, IsSuccessful = state == JobRunStates.Completed });
            }
        }

        /// <summary>
        /// Gets job artifacts.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <returns>A list of <see cref="JobArtefactModel"/>s. List is empty if none are found or an error is thrown in the process.</returns>
        public List<JobArtefactModel> GetArtefactsByJobRunId(long jobRunId)
        {
            try
            {
                var artefacts = _artefactsStorageProvider.GetArtefacts(jobRunId.ToString());
                return _mapper.Map<List<JobArtefactModel>>(artefacts);
            }
            catch (Exception)
            {
                // ignored
            }

            return new List<JobArtefactModel>();
        }

        /// <summary>
        /// Gets a <see cref="Stream"/> of artifacts for the job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="filename">Target file to stream to.</param>
        /// <returns>An artifact <see cref="Stream"/> pointed towards the file. Null if none are found or error is thrown in the process.</returns>
        public Stream GetArtefactAsStream(long jobRunId, string filename)
        {
            try
            {
                return _artefactsStorageProvider.Load(jobRunId.ToString(), filename);
            }
            catch (Exception)
            {
                // ignored
            }

            return null;
        }

        /// <summary>
        /// Adds an artifact to a job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="fileName">Filename of the file containing an artifact.</param>
        /// <param name="result">Result <see cref="Stream"/>.</param>
        public void AddArtefact(long jobRunId, string fileName, Stream result)
        {
            _artefactsStorageProvider.Save(jobRunId.ToString(), fileName, result);
        }

        /// <summary>
        /// Updates the process ID of the job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        /// <param name="processId">New process ID.</param>
        public void UpdatePid(long jobRunId, int processId)
        {
            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);
            jobRun.Pid = processId;

            _jobbrRepository.Update(jobRun);
        }

        /// <summary>
        /// Deletes a job.
        /// </summary>
        /// <param name="jobRunId">ID of the job.</param>
        public void Delete(long jobRunId)
        {
            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);

            jobRun.Deleted = true;

            _jobbrRepository.Update(jobRun);
        }
    }
}