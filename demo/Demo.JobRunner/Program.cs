using System;
using System.Reflection;

using Jobbr.Runtime;

namespace Demo.JobRunner
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var jobbrRuntime = new JobbrRuntime(typeof(MyJobs.MinimalJob).Assembly);
            
            jobbrRuntime.Run(args);
        }
    }
}
