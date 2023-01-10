using System;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Builder;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMessenger;
using JobRunStates = Jobbr.Server.Core.Models.JobRunStates;

namespace Jobbr.Server.IntegrationTests.Components.JobRunService
{
    [TestClass]
    public class ProgressUpdateTests
    {
        private readonly Server.Core.JobRunService _service;
        private readonly JobbrRepository _repo;
        private readonly TinyMessengerHub _messengerHub;

        public ProgressUpdateTests()
        {
            var autoMapperConfig = new AutoMapperConfigurationFactory(NullLoggerFactory.Instance).GetNew();

            _repo = new JobbrRepository(NullLoggerFactory.Instance, new InMemoryJobStorageProvider());

            _messengerHub = new TinyMessengerHub();

            _service = new Server.Core.JobRunService(NullLoggerFactory.Instance, _messengerHub, _repo, null, autoMapperConfig.CreateMapper());
        }

        [TestMethod]
        public void JobRun_HasStarted_StartDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            _service.UpdateState(jobrun.Id, JobRunStates.Started);

            var fromRepo = _repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualStartDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_EndDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            _service.UpdateState(jobrun.Id, JobRunStates.Completed);
            var fromRepo = _repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasFailed_EndDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            _service.UpdateState(jobrun.Id, JobRunStates.Failed);
            var fromRepo = _repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_MessageIsIssued()
        {
            var jobrun = GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            _messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            _service.UpdateState(jobrun.Id, JobRunStates.Completed);

            Assert.IsNotNull(message);
        }

        [TestMethod]
        public void JobRun_HasFailed_MessageIsIssued()
        {
            var jobrun = GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            _messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            _service.UpdateState(jobrun.Id, JobRunStates.Failed);

            Assert.IsNotNull(message);
        }

        private JobRun GivenAJobRun()
        {
            var job1 = new Job();
            _repo.AddJob(job1);

            var trigger = new InstantTrigger
            {
                JobId = job1.Id,
                IsActive = true
            };

            var jobrun = _repo.SaveNewJobRun(job1, trigger, DateTime.UtcNow);
            return jobrun;
        }
    }
}
