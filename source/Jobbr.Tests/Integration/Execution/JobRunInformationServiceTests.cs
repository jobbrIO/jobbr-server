using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Execution
{
    [TestClass]
    public class JobRunInformationServiceTests : JobRunExecutionTestBase
    {
        [TestMethod]
        public void RunningServer_GetInfoByRandomId_ReturnsNull()
        {
            var result = this.Services.InformationService.GetByJobRunId(-12);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ExistingJobWithFirstRun_GetInfoById_MatchesConfiguration()
        {
            var job = this.CreateTestJob();

            var trigger = CreateInstantTrigger(job);

            var createdJobRun = this.TriggerNewJobbRun(trigger);

            var result = this.Services.InformationService.GetByJobRunId(createdJobRun.Id);

            Assert.IsNotNull(result);
            Assert.AreEqual(job.Id, result.JobId);
            Assert.AreEqual(job.Type, result.Type);
            Assert.AreEqual(job.Parameters, result.JobParameters);

            Assert.AreEqual(trigger.Parameters, result.InstanceParameters);
            Assert.AreEqual(trigger.UserId, result.UserId);
            Assert.AreEqual(trigger.UserName, result.Username);

            Assert.AreEqual(job.Parameters, result.JobParameters);
            Assert.AreEqual(createdJobRun.InstanceParameters, result.InstanceParameters);
        }
    }
}
