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

            var config = new DefaultJobbrConfiguration
                             {
                                 StorageProvider = storageProvider,
                                 JobRunnerExeResolver = () => @"..\..\..\Demo.JobRunner\bin\Debug\Demo.JobRunner.exe"
                             };

            using (var jobbrServer = new JobbrServer(config))
            {
                jobbrServer.Start();

                Console.Write("JobServer has started on {0}. Press enter to quit", config.BackendAddress);
                Console.ReadLine();

                Console.Write("Shutting down. Please wait...");
                jobbrServer.Stop();
            }
        }
    }
}
