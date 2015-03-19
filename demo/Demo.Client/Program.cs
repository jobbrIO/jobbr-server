using System;
using System.Linq;

using Jobbr.Client;
using Jobbr.Common.Model;

namespace Demo.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var jobbrClient = new JobbrClient("http://localhost:80/jobbr");

            var allJobs = jobbrClient.GetAllJobs();

            var jobId = allJobs.First().Id;

            var trigger = jobbrClient.TriggerJob(jobId, new InstantTriggerDto() { IsActive = true, UserName = "userName", TriggerType = InstantTriggerDto.TypeName });

            Console.WriteLine("Got Trigger with Id:" + trigger.Id);

            var jobRuns = jobbrClient.GetJobRunsByTriggerId(trigger.Id);

            Console.WriteLine("There are {0} jobruns assigned to this trigger id.", jobRuns.Count);

            var jobRun = jobbrClient.GetJobRunById(jobRuns[0].JobRunId);

            Console.WriteLine("Current State: " + jobRun.State);

            Console.ReadLine();

        }
    }
}
