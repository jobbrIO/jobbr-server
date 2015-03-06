using System;

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
        public void Run(params string[] args)
        {
            Console.Write("This is the runner started at " + DateTime.UtcNow + " (UTC) with arguments " + args);
            Console.ReadKey();
        }
    }
}
