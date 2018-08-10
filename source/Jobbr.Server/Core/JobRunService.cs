using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using TinyMessenger;
using JobRunStates = Jobbr.Server.Core.Models.JobRunStates;

namespace Jobbr.Server.Core
{
    public class JobRunService
    {
        private static readonly ILog Logger = LogProvider.For<JobRunService>();

        private readonly ITinyMessengerHub messengerHub;
        private readonly IJobbrRepository repository;
        private readonly IArtefactsStorageProvider artefactsStorageProvider;
        private readonly IMapper mapper;

        public JobRunService(ITinyMessengerHub messengerHub, IJobbrRepository repository, IArtefactsStorageProvider artefactsStorageProvider, IMapper mapper)
        {
            this.messengerHub = messengerHub;
            this.repository = repository;
            this.artefactsStorageProvider = artefactsStorageProvider;
            this.mapper = mapper;
        }

        public void UpdateProgress(long jobRunId, double progress)
        {
            this.repository.UpdateJobRunProgress(jobRunId, progress);
        }

        public void UpdateState(long jobRunId, JobRunStates state)
        {
            Logger.InfoFormat("[{0}] The JobRun with id: {0} has switched to the '{1}'-State", jobRunId, state);

            var jobRun = this.repository.GetJobRunById(jobRunId);
            jobRun.State = this.mapper.Map<ComponentModel.JobStorage.Model.JobRunStates>(state);

            if (state == JobRunStates.Started)
            {
                jobRun.ActualStartDateTimeUtc = DateTime.UtcNow;
            }

            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                jobRun.ActualEndDateTimeUtc = DateTime.UtcNow;
            }

            this.repository.Update(jobRun);

            if (state == JobRunStates.Completed || state == JobRunStates.Failed)
            {
                this.messengerHub.Publish(new JobRunCompletedMessage(this) { Id = jobRunId, IsSuccessful = state == JobRunStates.Completed });
            }
        }

        public List<JobArtefactModel> GetArtefactsByJobRunId(long jobRunId)
        {
            try
            {
                var artefacts = this.artefactsStorageProvider.GetArtefacts(jobRunId.ToString());
                return this.mapper.Map<List<JobArtefactModel>>(artefacts);
            }
            catch
            {
                // ignored
            }

            return new List<JobArtefactModel>();
        }

        public Stream GetArtefactAsStream(long jobRunId, string filename)
        {
            try
            {
                return this.artefactsStorageProvider.Load(jobRunId.ToString(), filename);
            }
            catch
            {
            }

            return null;
        }

        public void AddArtefact(long jobRunId, string fileName, Stream result)
        {
            this.artefactsStorageProvider.Save(jobRunId.ToString(), fileName, result);
        }

        public void UpdatePid(long jobRunId, string host, int pid)
        {
            var jobRun = this.repository.GetJobRunById(jobRunId);
            jobRun.Pid = pid;

            this.repository.Update(jobRun);
        }

        /// <summary>
        /// Returns all <returns>running</returns> jobs
        /// </summary>
        public IEnumerable<long> GetRunningJobIds()
        {
            return this.repository.GetRunningJobs().Select(s => s.Id);
        }
    }
}