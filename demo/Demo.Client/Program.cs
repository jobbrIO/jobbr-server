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
            Console.WriteLine("------------------------------------------------------------------------------");
            Console.ReadLine();

            var createdTrigger = jobbrClient.TriggerJob(jobId, new ScheduledTriggerDto { IsActive = true, UserName = "userName", StartDateTimeUtc = DateTime.UtcNow.AddMinutes(30), TriggerType = ScheduledTriggerDto.TypeName });
            Console.WriteLine("Created FutureTrigger with Id:" + trigger.Id + ", IsActive: " + createdTrigger.IsActive);

            var futureTrigger = jobbrClient.GetTriggerById<ScheduledTriggerDto>(createdTrigger.Id);
            Console.WriteLine("Got FutureTrigger by Id:" + trigger.Id + ", IsActive: " + createdTrigger.IsActive);

            var disableTriggerInfo = new ScheduledTriggerDto() { Id = futureTrigger.Id, IsActive = false, TriggerType = ScheduledTriggerDto.TypeName };
            var dectivatedTrigger = jobbrClient.UpdateTrigger(disableTriggerInfo);

            Console.WriteLine("Disabled FutureTrigger width Id:" + trigger.Id + ", IsActive: " + dectivatedTrigger.IsActive);

            Console.ReadLine();

        }
    }
}
