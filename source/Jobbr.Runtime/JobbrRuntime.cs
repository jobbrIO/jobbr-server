using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Jobbr.Runtime
{
    /// <summary>
    /// The jobbr runtime.
    /// </summary>
    public class JobbrRuntime
    {
        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Run(string[] args)
        {
            var options = new CommandlineOptions();
            CommandLine.Parser.Default.ParseArguments(args, options);

            if (options.IsChatty)
            {
                Console.Write("This is the runner started at " + DateTime.UtcNow + " (UTC) with arguments " + string.Join(" ", args));

                Console.WriteLine();
                Console.WriteLine("JobRunId:  " + options.JobRunId);
                Console.WriteLine("JobServer: " + options.JobServer);
                Console.WriteLine("IsDebug:   " + options.IsDebug);
            }

            if (options.IsDebug)
            {
                var beginWaitForDebugger = DateTime.Now;
                var endWaitForDebugger = beginWaitForDebugger.AddSeconds(10);
                var pressedEnter = false;

                Console.WriteLine(string.Empty);
                Console.WriteLine(">>> DEBUG-Mode is enabled. You have 10s to attach a Debugger\\n     or press enter to continue. Counting...");

                new TaskFactory().StartNew(
                    () =>
                        {
                            Console.ReadLine();
                            pressedEnter = true;
                        });

                while (!pressedEnter || (!Debugger.IsAttached && endWaitForDebugger < DateTime.Now))
                {
                    Thread.Sleep(500);
                }
            }
        }
    }
}
