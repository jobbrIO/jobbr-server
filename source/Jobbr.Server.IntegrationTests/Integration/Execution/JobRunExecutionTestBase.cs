using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using InstantTrigger = Jobbr.ComponentModel.Management.Model.InstantTrigger;
using Job = Jobbr.ComponentModel.Management.Model.Job;

namespace Jobbr.Server.IntegrationTests.Integration.Execution
{
    public class JobRunExecutionTestBase : RunningJobbrServerTestBase
    {
        protected JobRun TriggerNewJobRun(InstantTrigger trigger)
        {
            Services.JobManagementService.AddTrigger(trigger.JobId, trigger);

            WaitFor.HasElements(Services.JobStorageProvider.GetJobRuns().Items.Where(jr => jr.Trigger.Id == trigger.Id).ToList, 1500);

            var createdJobRun = Services.JobStorageProvider.GetJobRuns().Items.First(jr => jr.Trigger.Id == trigger.Id);
            return createdJobRun;
        }

        protected static InstantTrigger CreateInstantTrigger(Job job)
        {
            return new InstantTrigger()
            {
                JobId = job.Id,
                Comment = "Comment",
                UserDisplayName = "UserDisplayName",
                UserId = "42",
                Parameters = "triggerParams",
                IsActive = true
            };
        }

        protected Job CreateTestJob()
        {
            var job = new Job()
            {
                Title = "TestJob",
                Type = "JobType",
                Parameters = "JobParams",
                UniqueName = "UniqueTestJobName"
            };

            Services.JobManagementService.AddJob(job);

            return job;
        }
    }
}