using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

using Dapper;

using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Dapper
{
    /// <summary>
    /// The jobbr dapper provider to store jobserver repository, queue and status information
    /// </summary>
    public class DapperStorageProvider : IJobStorageProvider
    {
        private string connectionString;

        private readonly string schemaName;

        public DapperStorageProvider(string connectionString, string schemaName = "Jobbr")
        {
            this.connectionString = connectionString;
            this.schemaName = schemaName;
        }

        public override string ToString()
        {
            return string.Format("[{0}, Schema: '{1}', Connection: '{2}']", this.GetType().Name, this.schemaName, this.connectionString);
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
                        @"INSERT INTO {0}.Jobs ([UniqueName],[Title],[Type],[Parameters],[CreatedDateTimeUtc]) VALUES (@UniqueName, @Title, @Type, @Parameters, @UtcNow)
                          SELECT CAST(SCOPE_IDENTITY() as int)", 
                        this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<int>(sql, new { job.UniqueName, job.Title, job.Type, job.Parameters, DateTime.UtcNow, }).Single();
            }
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            var sql = string.Format("SELECT TOP 1 * FROM {0}.JobRuns WHERE [TriggerId] = @TriggerId ORDER BY [PlannedStartDateTimeUtc] DESC", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobRuns = connection.Query<JobRun>(sql, new { TriggerId = triggerId }).ToList();

                return jobRuns.Any() ? jobRuns.FirstOrDefault() : null;
            }
        }

        public JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            var sql = string.Format("SELECT * FROM {0}.JobRuns WHERE [TriggerId] = @TriggerId AND PlannedStartDateTimeUtc >= @DateTimeNowUtc AND State = @State ORDER BY [PlannedStartDateTimeUtc] ASC", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var jobRuns = connection.Query<JobRun>(sql, new { TriggerId = triggerId, DateTimeNowUtc = DateTime.UtcNow, State = JobRunState.Scheduled.ToString() }).ToList();

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
                                    this.schemaName, 
                                    "JobRuns");

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(
                    sql, 
                    new {
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

        public Job GetJobById(long id)
        {
            var sql = string.Format("SELECT TOP 1 * FROM {0}.Jobs WHERE [Id] = @Id", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<Job>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public Job GetJobByUniqueName(string identifier)
        {
            var sql = string.Format("SELECT TOP 1 * FROM {0}.Jobs WHERE [UniqueName] = @UniqueName", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<Job>(sql, new { UniqueName = identifier }).FirstOrDefault();
            }
        }

        public JobRun GetJobRunById(long id)
        {
            var sql = string.Format("SELECT * FROM {0}.JobRuns WHERE [Id] = @Id", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<JobRun>(sql, new { Id = id }).FirstOrDefault();
            }
        }

        public List<JobRun> GetJobRunsForUserId(long userId)
        {
            var sql = string.Format("SELECT jr.* FROM {0}.JobRuns AS jr LEFT JOIN {0}.Triggers AS tr ON tr.Id = jr.TriggerId WHERE tr.UserId = @Id", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<JobRun>(sql, new { Id = userId }).ToList();
            }
        }

        public List<JobRun> GetJobRunsForUserName(string userName)
        {
            var sql = string.Format("SELECT jr.* FROM {0}.JobRuns AS jr LEFT JOIN {0}.Triggers AS tr ON tr.Id = jr.TriggerId WHERE tr.UserName = @UserName", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<JobRun>(sql, new { UserName = userName }).ToList();
            }
        }

        public bool Update(Job job)
        {
            var fromDb = this.GetJobById(job.Id);

            if (fromDb == null)
            {
                return false;
            }

            var sql = string.Format(
                                    @"UPDATE {0}.{1} SET
                                        [UniqueName] = @UniqueName,
                                        [Title] = @Title,
                                        [Type] = @Type,
                                        [Parameters] = @Parameters,
                                        [UpdatedDateTimeUtc] = @UtcNow
                                    WHERE [Id] = @Id",
                                    this.schemaName,
                                    "Jobs");

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(
                    sql, 
                    new {
                        job.Id,
                        job.UniqueName,
                        job.Title,
                        job.Type,
                        job.Parameters,
                        DateTime.UtcNow
                    });

                return true;
            }
        }

        public bool Update(InstantTrigger trigger)
        {
            return this.UpdateTrigger(trigger, delayedInMinutes: trigger.DelayedMinutes);
        }

        public bool Update(ScheduledTrigger trigger)
        {
            return this.UpdateTrigger(trigger, startDateTimeUtc: trigger.StartDateTimeUtc, endDateTimeUtc: trigger.StartDateTimeUtc);
        }

        public bool Update(RecurringTrigger trigger)
        {
            return this.UpdateTrigger(trigger, trigger.Definition);
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            var sql = string.Format("SELECT * FROM {0}.JobRuns WHERE [TriggerId] = @TriggerId", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                return connection.Query<JobRun>(sql, new { TriggerId = triggerId }).ToList();
            }
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            return this.InsertTrigger(trigger, InstantTrigger.TypeName, delayedInMinutes: trigger.DelayedMinutes);
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            return this.InsertTrigger(trigger, ScheduledTrigger.TypeName, startDateTimeUtc: trigger.StartDateTimeUtc, endDateTimeUtc: trigger.StartDateTimeUtc);
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            return this.InsertTrigger(trigger, RecurringTrigger.TypeName, trigger.Definition);
        }

        public bool DisableTrigger(long triggerId)
        {
            var sql = string.Format("UPDATE {0}.Triggers SET [IsActive] = @IsActive WHERE [Id] = @TriggerId", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(sql, new { TriggerId = triggerId, IsActive = false });

                return true;
            }
        }

        public bool EnableTrigger(long triggerId)
        {
            var sql = string.Format("UPDATE {0}.Triggers SET [IsActive] = @IsActive WHERE [Id] = @TriggerId", this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Execute(sql, new { TriggerId = triggerId, IsActive = true });

                return true;
            }
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            var sql = string.Format(
                @"SELECT * FROM {0}.Triggers where TriggerType = '{1}' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = '{2}' AND JobId = @JobId
                  SELECT * FROM {0}.Triggers where TriggerType = '{3}' AND JobId = @JobId",
                this.schemaName,
                InstantTrigger.TypeName, 
                RecurringTrigger.TypeName, 
                ScheduledTrigger.TypeName);
                

            using (var connection = new SqlConnection(this.connectionString))
            {
                using (var multi = connection.QueryMultiple(sql, new { JobId = jobId }))
                {
                    var instantTriggers = multi.Read<InstantTrigger>().ToList();
                    var cronTriggers = multi.Read<RecurringTrigger>().ToList();
                    var dateTimeTriggers = multi.Read<ScheduledTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(cronTriggers);
                    result.AddRange(dateTimeTriggers);

                    return result.OrderBy(t => t.Id).ToList();
                }
            }
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            var sql = string.Format(
                    @"SELECT * FROM {0}.Triggers where TriggerType = '{1}' AND Id = @Id
                      SELECT * FROM {0}.Triggers where TriggerType = '{2}' AND Id = @Id
                      SELECT * FROM {0}.Triggers where TriggerType = '{3}' AND Id = @Id",
                this.schemaName, 
                InstantTrigger.TypeName, 
                RecurringTrigger.TypeName, 
                ScheduledTrigger.TypeName);

            var param = new { Id = triggerId };

            return this.ExecuteSelectTriggerQuery(sql, param).FirstOrDefault();
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            var sql = string.Format(
                @"SELECT * FROM {0}.Triggers where TriggerType = '{1}' AND IsActive = 1
                  SELECT * FROM {0}.Triggers where TriggerType = '{2}' AND IsActive = 1
                  SELECT * FROM {0}.Triggers where TriggerType = '{3}' AND IsActive = 1",
                this.schemaName, 
                InstantTrigger.TypeName, 
                RecurringTrigger.TypeName, 
                ScheduledTrigger.TypeName);

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
                    var recurringTriggers = multi.Read<RecurringTrigger>().ToList();
                    var scheduledTriggers = multi.Read<ScheduledTrigger>().ToList();

                    var result = new List<JobTriggerBase>();

                    result.AddRange(instantTriggers);
                    result.AddRange(recurringTriggers);
                    result.AddRange(scheduledTriggers);

                    return result.ToList();
                }
            }
        }

        private long InsertTrigger(JobTriggerBase trigger, string type, string definition = "", DateTime? startDateTimeUtc = null, DateTime? endDateTimeUtc = null, int delayedInMinutes = 0)
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var sql = string.Format(
                @"INSERT INTO {0}.Triggers([JobId],[TriggerType],[Definition],[StartDateTimeUtc],[EndDateTimeUtc],[DelayedInMinutes],[IsActive],[UserId],[UserName],[UserDisplayName],[Parameters],[Comment],[CreatedDateTimeUtc])
                  VALUES (@JobId,@TriggerType,@Definition,@StartDateTimeUtc,@EndDateTimeUtc,@DelayedInMinutes,1,@UserId,@UserName,@UserDisplayName,@Parameters,@Comment,@UtcNow)
                  SELECT CAST(SCOPE_IDENTITY() as int)",
                this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var triggerObject =
                    new
                    {
                        trigger.JobId,
                        TriggerType = type,
                        Definition = definition,
                        StartDateTimeUtc = startDateTimeUtc,
                        EndDateTimeUtc = endDateTimeUtc,
                        DelayedInMinutes = delayedInMinutes,
                        trigger.IsActive,
                        trigger.UserId,
                        trigger.UserName,
                        trigger.UserDisplayName,
                        trigger.Parameters,
                        trigger.Comment,
                        UtcNow = dateTimeUtcNow
                    };
                
                var id = connection.Query<int>(sql, triggerObject).Single();
                trigger.CreatedDateTimeUtc = dateTimeUtcNow;
                trigger.Id = id;

                return id;
            }
        }

        private bool UpdateTrigger(JobTriggerBase trigger, string definition = "", DateTime? startDateTimeUtc = null, DateTime? endDateTimeUtc = null, int delayedInMinutes = 0)
        {
            var dateTimeUtcNow = DateTime.UtcNow;

            var sql = string.Format(
                @"UPDATE {0}.[Triggers]
                  SET [Definition] = @Definition
                     ,[StartDateTimeUtc] = @StartDateTimeUtc
                     ,[EndDateTimeUtc] = @EndDateTimeUtc
                     ,[DelayedInMinutes] = @DelayedInMinutes
                     ,[IsActive] = @IsActive
                     ,[UserId] = @UserId
                     ,[UserName] = @UserName
                     ,[UserDisplayName] = @UserDisplayName
                     ,[Parameters] = @Parameters
                     ,[Comment] = @Comment
                 WHERE Id = @Id",
                this.schemaName);

            using (var connection = new SqlConnection(this.connectionString))
            {
                var triggerObject =
                    new
                    {
                        trigger.Id,
                        Definition = definition,
                        StartDateTimeUtc = startDateTimeUtc,
                        EndDateTimeUtc = endDateTimeUtc,
                        DelayedInMinutes = delayedInMinutes,
                        trigger.IsActive,
                        trigger.UserId,
                        trigger.UserName,
                        trigger.UserDisplayName,
                        trigger.Parameters,
                        trigger.Comment,
                    };

                connection.Execute(sql, triggerObject);

                return true;
            }
        }
    }
}
