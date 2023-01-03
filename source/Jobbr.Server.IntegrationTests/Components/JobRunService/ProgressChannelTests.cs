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
        private readonly Server.Core.JobRunService service;
        private readonly JobbrRepository repo;
        private readonly TinyMessengerHub messengerHub;

        public ProgressUpdateTests()
        {
            var autoMapperConfig = new AutoMapperConfigurationFactory(new NullLoggerFactory()).GetNew();

            repo = new JobbrRepository(new NullLoggerFactory(), new InMemoryJobStorageProvider());

            messengerHub = new TinyMessengerHub();

            service = new Server.Core.JobRunService(new NullLoggerFactory(), messengerHub, repo, null, autoMapperConfig.CreateMapper());
        }

        private JobRun GivenAJobRun()
        {
            var job1 = new Job();
            repo.AddJob(job1);

            var trigger = new InstantTrigger
            {
                JobId = job1.Id,
                IsActive = true
            };

            var jobrun = repo.SaveNewJobRun(job1, trigger, DateTime.UtcNow);
            return jobrun;
        }

        [TestMethod]
        public void JobRun_HasStarted_StartDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            service.UpdateState(jobrun.Id, JobRunStates.Started);

            var fromRepo = repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualStartDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_EndDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            service.UpdateState(jobrun.Id, JobRunStates.Completed);
            var fromRepo = repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasFailed_EndDateTimeIsStored()
        {
            var jobrun = GivenAJobRun();

            service.UpdateState(jobrun.Id, JobRunStates.Failed);
            var fromRepo = repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_MessageIsIssued()
        {
            var jobrun = GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            service.UpdateState(jobrun.Id, JobRunStates.Completed);

            Assert.IsNotNull(message);
        }

        [TestMethod]
        public void JobRun_HasFailed_MessageIsIssued()
        {
            var jobrun = GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            service.UpdateState(jobrun.Id, JobRunStates.Failed);

            Assert.IsNotNull(message);
        }
    }
}
