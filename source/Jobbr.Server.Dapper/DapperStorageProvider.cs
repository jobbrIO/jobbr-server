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

        public DapperStorageProvider(string connectionString, string schemaName = "Jobbr")
        {
            this.connectionString = connectionString;
            this.schemaName = schemaName;
        }

        public List<Job> GetJobs()
        {
            var sql = string.Format("SELECT * FROM {0}.Jobs", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobs = connection.Query<Job>(sql);

                return jobs.ToList();
            }
        }

        public long AddJob(Job job)
        {
            var sql = string.Format(
                        @"INSERT INTO {0}.Job ([Name],[Type],[CreatedDateTimeUtc]) VALUES (@Name, @Type, @UtcNow)
                          SELECT CAST(SCOPE_IDENTITY() as int)", 
                        this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<int>(sql, new { job.Name, job.Type, DateTime.UtcNow, }).Single();
            }
        }

        public List<JobTriggerBase> GetTriggers(long jobId)
        {
            var sql = string.Format(
                @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND JobId = @JobId",
                this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                using (var multi = connection.QueryMultiple(sql, new { JobId = jobId }))
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

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            // TODO: Deserialize Json Params
            var sql = string.Format("SELECT TOP 1 * FROM {0}.JobRuns WHERE [TriggerId] = @TriggerId ORDER BY [PlannedStartDateTimeUtc] DESC", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobRuns = connection.Query<JobRun>(sql, new { TriggerId = triggerId }).ToList();

                return jobRuns.Any() ? jobRuns.FirstOrDefault() : null;
            }
        }

        public JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            // TODO: Deserialize Json Params
            var sql = string.Format("SELECT * FROM {0}.JobRuns WHERE [TriggerId] = @TriggerId AND PlannedStartDateTimeUtc >= @DateTimeNowUtc ORDER BY [PlannedStartDateTimeUtc] ASC", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobRuns = connection.Query<JobRun>(sql, new { TriggerId = triggerId, DateTimeNowUtc = DateTime.UtcNow }).ToList();

                return jobRuns.Any() ? jobRuns.FirstOrDefault() : null;
            }
        }

        public int AddJobRun(JobRun jobRun)
        {
            var sql = string.Format(
                        @"INSERT INTO {0}.JobRuns ([JobId],[TriggerId],[UniqueId],[JobParameters],[InstanceParameters],[PlannedStartDateTimeUtc],[State])
                          VALUES (@JobId,@TriggerId,@UniqueId,@JobParameters,@InstanceParameters,@PlannedStartDateTimeUtc,@State)
                          SELECT CAST(SCOPE_IDENTITY() as int)",
                        this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobRunObject =
                    new
                        {
                            jobRun.JobId,
                            jobRun.TriggerId,
                            jobRun.UniqueId,
                            jobRun.JobParameters,
                            jobRun.InstanceParameters,
                            jobRun.PlannedStartDateTimeUtc,
                            State = jobRun.State.ToString()
                        };

                var id = connection.Query<int>(sql, jobRunObject).Single();

                return id;
            }
        }

        public List<JobRun> GetJobRuns()
        {
            var sql = string.Format("SELECT * FROM {0}.JobRuns", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<JobRun>(sql).ToList();
            }
        }

        public bool Update(JobRun jobRun)
        {
            var fromDb = this.GetJobRuns().FirstOrDefault(jr => jr.Id == jobRun.Id);
            
            if (fromDb == null)
            {
                return false;
            }

            var sql = string.Format(
                                    @"UPDATE {0}.{1} SET
                                        [JobParameters] = @JobParameters,
                                        [InstanceParameters] = @InstanceParameters,
                                        [PlannedStartDateTimeUtc] = @PlannedStartDateTimeUtc,
                                        [ActualStartDateTimeUtc] = @ActualStartDateTimeUtc,
                                        [EstimatedEndDateTimeUtc] = @EstimatedEndDateTimeUtc,
                                        [ActualEndDateTimeUtc] = @ActualEndDateTimeUtc,
                                        [Progress] = @Progress,
                                        [State] = @State,
                                        [Pid] = @Pid,
                                        [WorkingDir] = @WorkingDir,
                                        [TempDir] = @TempDir
                                    WHERE [Id] = @Id",
                                    this.schemaName, "JobRuns");

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(sql, new
                                            {
                                                jobRun.Id,
                                                jobRun.JobParameters,
                                                jobRun.InstanceParameters,
                                                jobRun.PlannedStartDateTimeUtc,
                                                jobRun.ActualStartDateTimeUtc,
                                                jobRun.EstimatedEndDateTimeUtc,
                                                jobRun.ActualEndDateTimeUtc,
                                                jobRun.Progress,
                                                State = jobRun.State.ToString(),
                                                jobRun.Pid,
                                                jobRun.WorkingDir,
                                                jobRun.TempDir,
                                            });

                return true;
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
            var sql = string.Format("UPDATE {0}.Triggers SET [IsActive] = @IsActive WHERE [TriggerId] = @TriggerId", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(sql, new { TriggerId = triggerId, IsActive = false });

                return true;
            }
        }

        public bool EnableTrigger(long triggerId)
        {
            var sql = string.Format("UPDATE {0}.Contacts SET [IsActive] = @IsActive WHERE [JobId] = @JobId", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(sql, new { TriggerId = triggerId, IsActive = true });

                return true;
            }
        }

        public JobTriggerBase GetTriggerByJobId(long jobId)
        {
            var sql = string.Format(
                @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND JobId = @JobId",
                this.schemaName);

            var param = new { JobId = jobId };

            return this.ExecuteSelectTriggerQuery(sql, param).FirstOrDefault();
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            var sql = string.Format(
                    @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND Id = @Id
                      SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND Id = @Id
                      SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND Id = @Id",
                    this.schemaName);

            var param = new { Id = triggerId };

            return this.ExecuteSelectTriggerQuery(sql, param).FirstOrDefault();
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            var sql = string.Format(
                @"SELECT * FROM {0}.Triggers where TriggerType = 'Instant' AND IsActive = 1
                  SELECT * FROM {0}.Triggers where TriggerType = 'Cron' AND IsActive = 1
                  SELECT * FROM {0}.Triggers where TriggerType = 'DateTime' AND IsActive = 1",
                this.schemaName);

            var param = new { };

            return this.ExecuteSelectTriggerQuery(sql, param);
        }

        private List<JobTriggerBase> ExecuteSelectTriggerQuery(string sql, object param)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                using (var multi = connection.QueryMultiple(sql, param))
                {
                    var instantTriggers = multi.Read<InstantTrigger>().ToList();
                    var cronTriggers = multi.Read<CronTrigger>().ToList();
                    var dateTimeTriggers = multi.Read<StartDateTimeUtcTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(cronTriggers);
                    result.AddRange(dateTimeTriggers);

                    return result.ToList();
                }
            }
        }

        private long InsertTrigger(JobTriggerBase trigger, string cron, string definition = "", DateTime? dateTimeUtc = null, int delayedMinutes = 0)
        {
            var sql = string.Format(
                @"INSERT INTO {0}.Trigger([JobId],[TriggerType],[Definition],[StartDateTimeUtc],[DelayInMinutes],[IsActive],[UserId],[UserName],[UserDisplayName],[Parameter],[Comment],[CreatedDateTimeUtc])
                  VALUES (@JobId,@TriggerType,@Definition,@StartDateTimeUtc,@DelayInMinutes,1,@UserId,@UserName,@UserDisplayName,@Parameter,@Comment,@UtcNowd)
                  SELECT CAST(SCOPE_IDENTITY() as int)",
                this.schemaName);

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

                return connection.Query<int>(sql, triggerObject).Single();
            }
        }
    }
}
