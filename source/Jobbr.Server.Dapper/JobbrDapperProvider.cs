using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

using Jobbr.Server.Model;

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

        private string SelectTriggersForJobQuery;

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

            this.SelectTriggersForJobQuery =
                string.Format(
                    @"Select * from {0}.Triggers where TriggerType = 'Instant' AND JobId = @id
                      Select * from {0}.Triggers where TriggerType = 'Cron' AND JobId = @id
                      Select * from {0}.Triggers where TriggerType = 'DateTime' AND JobId = @id",
                    this.schemaName);
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

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                using (var multi = connection.QueryMultiple(this.SelectTriggersForJobQuery, new { id = jobId }))
                {
                    var instantTriggers = multi.Read<InstantTrigger>().ToList();
                    var cronTriggers = multi.Read<CronTrigger>().ToList();
                    var dateTimeTriggers = multi.Read<DateTimeUtcTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(cronTriggers);
                    result.AddRange(dateTimeTriggers);

                    return result;
                }
            }
        }
    }
}
