using System;
using System.Linq;
using System.Threading;
using Jobbr.Client;
using Jobbr.Server.WebAPI.Model;

namespace Jobbr.Sample.Client
{
    public class Program
    {
        static void Main(string[] args)
        {
            var jobbrClient = new JobbrClient("http://localhost:1337");

            Thread.Sleep(1000);

            var allJobs = jobbrClient.GetAllJobs();

            if (allJobs.Count == 0)
            {
                Console.WriteLine("At least one job is required to run this demo. Press enter to quit...");
                Console.ReadLine();
                return;
            }

            var jobId = allJobs.First().Id;

            var trigger = jobbrClient.AddTrigger(jobId, new InstantTriggerDto { IsActive = true, UserDisplayName = "userName" });
            Console.WriteLine("Got Trigger with Id:" + trigger.Id);

            var jobRuns = jobbrClient.GetJobRunsByTriggerId(jobId, trigger.Id);
            Console.WriteLine("There are {0} jobruns assigned to this trigger id.", jobRuns.Count);

            var jobRun = jobbrClient.GetJobRunById(jobRuns[0].JobRunId);
            Console.WriteLine("Current State: " + jobRun.State);
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.ReadLine();
        }
    }
}
