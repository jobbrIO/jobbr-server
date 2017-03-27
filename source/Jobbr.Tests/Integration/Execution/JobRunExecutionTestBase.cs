using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using InstantTrigger = Jobbr.ComponentModel.Management.Model.InstantTrigger;
using Job = Jobbr.ComponentModel.Management.Model.Job;

namespace Jobbr.Tests.Integration.Execution
{
    public class JobRunExecutionTestBase : RunningJobbrServerTestBase
    {
        protected JobRun TriggerNewJobbRun(InstantTrigger trigger)
        {
            this.Services.JobManagementService.AddTrigger(trigger);

            WaitFor.HasElements(this.Services.JobStorageProvider.GetJobRuns().Where(jr => jr.TriggerId == trigger.Id).ToList, 1500);

            var createdJobRun = this.Services.JobStorageProvider.GetJobRuns().First(jr => jr.TriggerId == trigger.Id);
            return createdJobRun;
        }

        protected static InstantTrigger CreateInstantTrigger(Job job)
        {
            return new InstantTrigger()
            {
                JobId = job.Id,
                Comment = "Comment",
                UserDisplayName = "UserDisplayName",
                UserId = 42,
                UserName = "UserName",
                Parameters = "triggerParams",
                IsActive = true
            };
        }

        protected Job CreateTestJob()
        {
            return this.Services.JobManagementService.AddJob(new Job()
            {
                Title = "TestJob",
                Type = "JobType",
                Parameters = "JobParams",
                UniqueName = "UniqueTestJobName"
            });
        }
    }
}