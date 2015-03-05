using System;

using Jobbr.Server;
using Jobbr.Server.Common;
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
            var storageProvider = new DapperStorageProvider(@"Data Source=.\SQLEXPRESS;Initial Catalog=JobbrDemo;Integrated Security=True");

            var config = new JobbrConfiguration
                             {
                                 StorageProvider = storageProvider,
                                 JobRunnerExeResolver = () => "Demo.JobRunner.exe"
                             };

            using (var jobserver = new JobbrServer(config))
            {
                jobserver.Start();

                Console.Write("JobServer has started on {0}. Press enter to exit", config.BackendAddress);
                Console.ReadLine();
            }
        }
    }
}
