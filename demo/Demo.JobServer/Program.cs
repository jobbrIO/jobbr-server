using System;

using Jobbr.Server;

namespace Demo.JobServer
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
            var config = new MyJobbrConfiguration();

            using (var jobbrServer = new JobbrServer(config))
            {
                jobbrServer.Start();

                Console.ReadLine();

                jobbrServer.Stop();
            }
        }
    }
}
