using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;

namespace Jobbr.Runtime
{
    /// <summary>
    /// The jobbr runtime.
    /// </summary>
    public class JobbrRuntime
    {
        private JobbrRuntimeClient client;

        private CommandlineOptions commandlineOptions;

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Run(string[] args)
        {
            this.ParseArguments(args);

            this.InitializeClient();
            this.client.PublishState(JobRunState.Initializing);

            this.DisplayWelcomeBannerIfEnabled(args);

            this.WaitForDebuggerIfEnabled();

        }

        private void InitializeClient()
        {
            this.client = new JobbrRuntimeClient(this.commandlineOptions.JobServer, this.commandlineOptions.JobRunId);
        }

        private void WaitForDebuggerIfEnabled()
        {
            if (this.commandlineOptions.IsDebug)
            {
                var beginWaitForDebugger = DateTime.Now;
                var endWaitForDebugger = beginWaitForDebugger.AddSeconds(10);
                var pressedEnter = false;

                Console.WriteLine(string.Empty);
                Console.WriteLine(">>> DEBUG-Mode is enabled. You have 10s to attach a Debugger");
                Console.Write("    or press enter to continue. Counting...");

                new TaskFactory().StartNew(
                    () =>
                        {
                            Console.ReadLine();
                            pressedEnter = true;
                        });

                while (!(pressedEnter || Debugger.IsAttached || endWaitForDebugger < DateTime.Now))
                {
                    Thread.Sleep(500);
                }
            }

            Debugger.Break();
        }

        private void DisplayWelcomeBannerIfEnabled(string[] args)
        {
            if (this.commandlineOptions.IsChatty)
            {
                Console.Write(
                    "This is the runner started at " + DateTime.UtcNow + " (UTC) with arguments " + string.Join(" ", args));

                Console.WriteLine();
                Console.WriteLine("JobRunId:  " + this.commandlineOptions.JobRunId);
                Console.WriteLine("JobServer: " + this.commandlineOptions.JobServer);
                Console.WriteLine("IsDebug:   " + this.commandlineOptions.IsDebug);
            }
        }

        private void ParseArguments(string[] args)
        {
            this.commandlineOptions = new CommandlineOptions();
            CommandLine.Parser.Default.ParseArguments(args, this.commandlineOptions);
        }
    }
}
