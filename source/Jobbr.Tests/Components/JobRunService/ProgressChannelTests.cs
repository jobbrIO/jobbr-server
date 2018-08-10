using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Builder;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMessenger;
using JobRunStates = Jobbr.Server.Core.Models.JobRunStates;

namespace Jobbr.Tests.Components.JobRunService
{
    [TestClass]
    public class ProgressUpdateTests
    {
        private readonly Server.Core.JobRunService service;
        private readonly JobbrRepository repo;
        private readonly TinyMessengerHub messengerHub;

        public ProgressUpdateTests()
        {
            var autoMapperConfig = new AutoMapperConfigurationFactory().GetNew();
            
            this.repo = new JobbrRepository(new InMemoryJobStorageProvider());

            this.messengerHub = new TinyMessengerHub();

            this.service = new Server.Core.JobRunService(this.messengerHub, this.repo, null, autoMapperConfig.CreateMapper());
        }

        private JobRun GivenAJobRun()
        {
            var job1 = new Job();
            this.repo.AddJob(job1);

            var trigger = new InstantTrigger
            {
                JobId = job1.Id,
                IsActive = true
            };

            var jobrun = this.repo.SaveNewJobRun(job1, trigger, DateTime.UtcNow);
            return jobrun;
        }

        [TestMethod]
        public void JobRun_HasStarted_StartDateTimeIsStored()
        {
            var jobrun = this.GivenAJobRun();

            this.service.UpdateState(jobrun.Id, JobRunStates.Started);

            var fromRepo = this.repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualStartDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_EndDateTimeIsStored()
        {
            var jobrun = this.GivenAJobRun();

            this.service.UpdateState(jobrun.Id, JobRunStates.Completed);
            var fromRepo = this.repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasFailed_EndDateTimeIsStored()
        {
            var jobrun = this.GivenAJobRun();

            this.service.UpdateState(jobrun.Id, JobRunStates.Failed);
            var fromRepo = this.repo.GetJobRunById(jobrun.Id);

            Assert.IsNotNull(fromRepo.ActualEndDateTimeUtc);
        }

        [TestMethod]
        public void JobRun_HasCompleted_MessageIsIssued()
        {
            var jobrun = this.GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            this.messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            this.service.UpdateState(jobrun.Id, JobRunStates.Completed);

            Assert.IsNotNull(message);
        }

        [TestMethod]
        public void JobRun_HasFailed_MessageIsIssued()
        {
            var jobrun = this.GivenAJobRun();
            JobRunCompletedMessage message = null;

            // Register for message
            this.messengerHub.Subscribe<JobRunCompletedMessage>(m => message = m);

            this.service.UpdateState(jobrun.Id, JobRunStates.Failed);

            Assert.IsNotNull(message);
        }

        [TestMethod]
        public void Should_return_running_jobs()
        {
            this.GivenAJobRun(); // add job, which will not run
            var runningJob = this.GivenAJobRun(); // add job, which is running
            var finishedJob = this.GivenAJobRun(); // add job, which will be finished
            this.service.UpdateState(runningJob.Id, JobRunStates.Processing);
            this.service.UpdateState(finishedJob.Id, JobRunStates.Completed);

            var runningJobs = this.service.GetRunningJobIds().ToList();

            Assert.AreEqual(runningJobs.Count, 1);
            Assert.AreEqual(runningJobs[0], runningJob.Id);
        }
    }
}
