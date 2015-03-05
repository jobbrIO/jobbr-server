using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

using Newtonsoft.Json;

namespace Jobbr.Server.Dapper
{
    /// <summary>
    /// The jobbr dapper provider to store jobserver repository, queue and status information
    /// </summary>
    public class DapperStorageProvider : IJobbrStorageProvider
    {
        private string connectionString;

        private readonly string schemaName;

        private string SelectAllJobsQuery;

        private string AddJobQuery;

        private string SelectTriggersForJobQuery;

        private string InsertTriggerQuery;

        private string UpdateTriggerQuery;

        public DapperStorageProvider(string connectionString, string schemaName = "Jobbr")
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
                    @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND JobId = @id
                      SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND JobId = @id
                      SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND JobId = @id",
                    this.schemaName);

            this.SelectActiveTriggersQuery =
                string.Format(
                    @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND IsActive = 1
                      SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND IsActive = 1
                      SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND IsActive = 1",
                    this.schemaName);

            this.InsertTriggerQuery = 
                string.Format(
                    @"INSERT INTO {0}.Trigger([JobId],[TriggerType],[Definition],[StartDateTimeUtc],[DelayInMinutes],[IsActive],[UserId],[UserName],[UserDisplayName],[Parameter],[Comment],[CreatedDateTimeUtc])
                      VALUES (@JobId,@TriggerType,@Definition,@StartDateTimeUtc,@DelayInMinutes,1,@UserId,@UserName,@UserDisplayName,@Parameter,@Comment,@UtcNowd)",
                this.schemaName);

            this.AddJobQuery = string.Format("INSERT INTO {0}.Contacts([Name],[Type],[CreatedDateTimeUtc]) VALUES (@Name,@Type,@UtcNow)", this.schemaName);
            this.AddJobQuery = string.Format("INSERT INTO {0}.Contacts([Name],[Type],[CreatedDateTimeUtc]) VALUES (@Name,@Type,@UtcNow)", this.schemaName);

            this.UpdateTriggerQuery = string.Format("UPDATE {0}.Contacts SET [IsActive] = @IsActive WHERE [JobId] = @JobId", this.schemaName);
        }

        private string SelectActiveTriggersQuery;

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
                    var dateTimeTriggers = multi.Read<StartDateTimeUtcTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(cronTriggers);
                    result.AddRange(dateTimeTriggers);

                    return result.OrderBy(t => t.Id).ToList();
                }
            }
        }

        public long AddTrigger(CronTrigger trigger)
        {
            return this.InsertTrigger(trigger, "Cron", trigger.Definition);
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            return this.InsertTrigger(trigger, "Instant", delayedMinutes: trigger.DelayedMinutes);
        }

        public long AddTrigger(StartDateTimeUtcTrigger trigger)
        {
            return this.InsertTrigger(trigger, "DateTime", dateTimeUtc: trigger.DateTimeUtc);
        }

        public bool DisableTrigger(long triggerId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(this.UpdateTriggerQuery, new { TriggerId = triggerId, IsActive = false });

                return true;
            }
        }

        public bool EnableTrigger(long triggerId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(this.UpdateTriggerQuery, new { TriggerId = triggerId, IsActive = true });

                return true;
            }
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                using (var multi = connection.QueryMultiple(this.SelectActiveTriggersQuery))
                {
                    var instantTriggers = multi.Read<InstantTrigger>().ToList();
                    var cronTriggers = multi.Read<CronTrigger>().ToList();
                    var dateTimeTriggers = multi.Read<StartDateTimeUtcTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(cronTriggers);
                    result.AddRange(dateTimeTriggers);

                    return result.OrderBy(t => t.Id).ToList();
                }
            }
        }

        private long InsertTrigger(JobTriggerBase trigger, string cron, string definition = "", DateTime? dateTimeUtc = null, int delayedMinutes = 0)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                object nullValue = null;
                string parameter = JsonConvert.SerializeObject(trigger.Parameters);

                var triggerObject =
                    new
                    {
                        trigger.JobId,
                        TriggerType = cron,
                        Definition = definition,
                        StartDateDateTimeUtc = dateTimeUtc,
                        DelayInMinutes = delayedMinutes,
                        trigger.IsActive,
                        trigger.UserId,
                        trigger.UserName,
                        trigger.UserDisplayName,
                        Parameter = parameter,
                        trigger.Comment,
                        DateTime.UtcNow
                    };

                return connection.Execute(this.InsertTriggerQuery, triggerObject);
            }
        }
    }
}
