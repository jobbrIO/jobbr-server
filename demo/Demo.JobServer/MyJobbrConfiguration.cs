using System;

using Jobbr.Server.Common;
using Jobbr.Server.Configuration;
using Jobbr.Server.Dapper;

namespace Demo.JobServer
{
    public class MyJobbrConfiguration : DefaultJobbrConfiguration
    {
        public MyJobbrConfiguration()
        {
            var storageProvider = new DapperStorageProvider(@"Data Source=.\SQLEXPRESS;Initial Catalog=JobbrDemo;Integrated Security=True");

            this.JobStorageProvider = storageProvider;
            this.ArtefactStorageProvider = new FileSystemArtefactsStorageProvider("data");
            this.JobRunnerExeResolver = () => @"..\..\..\Demo.JobRunner\bin\Debug\Demo.JobRunner.exe";
            this.BeChatty = true;
        }

        public override void OnRepositoryCreating(RepositoryBuilder repositoryBuilder)
        {
            base.OnRepositoryCreating(repositoryBuilder);

            repositoryBuilder.Define("MinimalJobId", "Demo.MyJobs.MinimalJob")
                .WithTrigger(new DateTime(2015, 3, 20, 12, 00, 00))
                .WithTrigger("* 15 * * *");
        }
    }
}