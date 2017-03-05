using System;
using System.Threading;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Builder;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyMessenger;
using JobRunStates = Jobbr.Server.Core.Models.JobRunStates;

namespace Jobbr.Tests.Components.JobRunService
{
    [TestClass]
    public class ProgressUpdateTests
    {
        private Server.Core.JobRunService service;
        private JobbrRepository repo;

        public ProgressUpdateTests()
        {
            var autoMapperConfig = new AutoMapperConfigurationFactory().GetNew();
            
            this.repo = new JobbrRepository(new InMemoryJobStorageProvider());

            this.service = new Server.Core.JobRunService(new TinyMessengerHub(), this.repo, autoMapperConfig.CreateMapper());
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

        }
    }
}
