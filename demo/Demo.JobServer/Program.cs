using System;

using Jobbr.Server;
using Jobbr.Server.Dapper;

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
            var dapperProvider = new JobbrDapperProvider(@"Data Source=.\SQLEXPRESS;Initial Catalog=JobbrDemo;Integrated Security=True");

            var config = new JobbrConfiguration
                             {
                                 JobQueueProvider = dapperProvider, 
                                 JobRepositoryProvider = dapperProvider,
                                 JobRunnerExeResolver = () => "Demo.JobRunner.exe"
                             };

            using (var jobserver = new JobbrServer(config))
            {
                jobserver.Start();

                Console.Write("JobServer has started. Press enter to exit");
                Console.ReadLine();
            }
        }
    }
}
