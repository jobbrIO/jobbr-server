using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Tests.Integration.Scheduler;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Execution
{
    [TestClass]
    public class JobRunInformationServiceTests : RunningJobbrServerTestBase
    {
        [TestMethod]
        public void RunningServer_GetInfoByRandomId_ReturnsNull()
        {
            var result = this.Services.InformationService.GetByJobRunId(-12);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void RunningServer_GetInfoByRandomGuid_ReturnsNull()
        {
            var result = this.Services.InformationService.GetByUniqueId(new Guid());

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExistingJobWithRun_GetInfoById_MatchesConfiguration()
        {
            var job = this.Services.JobManagementService.AddJob(new Job()
            {
                Title = "TestJob",
                Type = "JobType",
                Parameters = "JobParams",
                UniqueName = "UniqueTestJobName"
            });

            this.Services.JobManagementService.AddTrigger(new InstantTrigger()
            {
                JobId = job.Id,
                Comment = "Comment",
                UserDisplayName = "UserDisplayName",
                UserId = 42,
                UserName = "UserName",
                Parameters = "triggerParams",
                IsActive = true
            });

            WaitFor.HasElements(this.Services.JobStorageProvider.GetJobRuns().ToList, 1500);

            var createdJobRun = this.Services.JobStorageProvider.GetJobRuns().First();

            var result = this.Services.InformationService.GetByJobRunId(createdJobRun.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(job.Id, result.JobId);
            Assert.AreEqual(job.Type, result.Type);
            Assert.AreEqual(job.Parameters, result.JobParameters);
            Assert.AreEqual(createdJobRun.InstanceParameters, result.InstanceParameters);
        }
    }
}
