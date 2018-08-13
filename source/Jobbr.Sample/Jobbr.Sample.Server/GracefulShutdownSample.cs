using System;
using System.Threading;
using Jobbr.Server.Builder;
using Jobbr.Server.ForkedExecution;
using Jobbr.Server.JobRegistry;

namespace Jobbr.Sample.Server
{
    public class GracefulShutdownSample
    {
        public static void Main(string[] args)
        {
            var jobbrBuilder = new JobbrBuilder();

            jobbrBuilder.AddForkedExecution(config =>
            {
                config.JobRunDirectory = @"C:\temp\";
                config.MaxConcurrentProcesses = 4;
                config.IsRuntimeWaitingForDebugger = false;
                // We will build all the components in one place, because we want to test the server
                // and not the integration between server/runtime (there are other assemblies for that)
                config.JobRunnerExecutable = "Jobbr.Sample.Runtime.exe";
            });

            // Add jobs
            jobbrBuilder.AddJobs(repository =>
            {
                ////repository.Define("TimeoutJob", "Jobbr.Sample.Tasks.TimeoutClass").WithParameter(DateTime.UtcNow.AddMilliseconds(500));
                repository.Define("InTimeJob", "Jobbr.Sample.Jobs.InTimeClass").WithTrigger("* * * * *");
                repository.Define("TimeoutJob1", "Jobbr.Sample.Jobs.TimeoutClass").WithTrigger("* * * * *");
                repository.Define("TimeoutJob2", "Jobbr.Sample.Jobs.TimeoutClass").WithTrigger("* * * * *");
            });

            using (var server = jobbrBuilder.Create())
            {
                server.Start();

                // Jobs will be executed every full minute, so lets just wait
                WaitUntilJobsAreExecute();

                // We will wait 10 seconds, so our InTimeClass can terminate itself normally
                // But we will also have two Timeout-instances, which will not be terminated
                server.GracefulStop(TimeSpan.FromSeconds(10));
            }

            Console.ReadLine();
        }

        private static void WaitUntilJobsAreExecute()
        {
            while (DateTime.Now.Second != 0)
            {
                Thread.Sleep(250);
            }
        }
    }
}
