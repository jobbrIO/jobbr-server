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
    public class JobRunService : IJobRunService
    {
        private readonly ILogger<JobRunService> _logger;
        private readonly ITinyMessengerHub _messengerHub;
        private readonly IJobbrRepository _jobbrRepository;
        private readonly IArtefactsStorageProvider _artefactsStorageProvider;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunService"/> class.
        /// </summary>
        public JobRunService(ILoggerFactory loggerFactory, ITinyMessengerHub messengerHub, IJobbrRepository jobbrRepository, IArtefactsStorageProvider artefactsStorageProvider, IMapper mapper)
        {
            _logger = loggerFactory.CreateLogger<JobRunService>();
            _messengerHub = messengerHub;
            _jobbrRepository = jobbrRepository;
            _artefactsStorageProvider = artefactsStorageProvider;
            _mapper = mapper;
        }

        /// <inheritdoc/>
        public void UpdateProgress(long jobRunId, double progress)
        {
            _jobbrRepository.UpdateJobRunProgress(jobRunId, progress);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void AddArtefact(long jobRunId, string fileName, Stream result)
        {
            _artefactsStorageProvider.Save(jobRunId.ToString(), fileName, result);
        }

        /// <inheritdoc/>
        public void UpdatePid(long jobRunId, int processId)
        {
            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);
            jobRun.Pid = processId;

            _jobbrRepository.Update(jobRun);
        }

        /// <inheritdoc/>
        public void Delete(long jobRunId)
        {
            var jobRun = _jobbrRepository.GetJobRunById(jobRunId);

            jobRun.Deleted = true;

            _jobbrRepository.Update(jobRun);
        }
    }
}