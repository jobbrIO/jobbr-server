using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

namespace Jobbr.Server.Dapper
{
    /// <summary>
    /// The jobbr dapper provider to store jobserver repository, queue and status information
    /// </summary>
    public class JobbrDapperProvider : IJobQueueProvider, IJobRepositoryProvider
    {
        private string connectionString;

        private readonly string schemaName;

        private string SelectAllJobsQuery;

        private string AddJobQuery;

        public JobbrDapperProvider(string connectionString, string schemaName = "Jobbr")
        {
            this.connectionString = connectionString;
            this.schemaName = schemaName;

            this.DefineQueries();
        }

        private void DefineQueries()
        {
            this.SelectAllJobsQuery = string.Format("SELECT * FROM {0}.Jobs", this.schemaName);
            this.AddJobQuery = string.Format("INSERT INTO {0}.Contacts([Name],[Type],[CreatedDateTimeUtc]) VALUES (@Name,@Type,@UtcNow)", this.schemaName);
        }

        public List<Job> GetJobs()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobs = connection.Query<Job>(this.SelectAllJobsQuery);

                return jobs.ToList();
            }
        }

        public long AddJob(Job job)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Execute(this.AddJobQuery, new { job.Name, job.Type, DateTime.UtcNow, });
            }
        }
    }
}
