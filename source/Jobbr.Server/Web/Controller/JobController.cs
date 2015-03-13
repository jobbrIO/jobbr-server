using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Dto;

using Newtonsoft.Json;

namespace Jobbr.Server.Web.Controller
{
    using System.Net;

    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        private readonly IJobService service;

        private readonly IJobStorageProvider jobStorageProvider;

        public JobController(IJobService service, IJobStorageProvider jobStorageProvider)
        {
            this.service = service;
            this.jobStorageProvider = jobStorageProvider;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobStorageProvider.GetJobs().Select(this.Map));
        }

        [HttpPost]
        [Route("api/jobs")]
        public IHttpActionResult AddJob([FromBody] JobDto dto)
        {
            var identifier = dto.UniqueName;

            if (string.IsNullOrEmpty(dto.UniqueName))
            {
                return this.StatusCode(HttpStatusCode.NotAcceptable);
            }

            var existingJob = this.jobStorageProvider.GetJobByUniqueName(identifier);

            if (existingJob != null)
            {
                return this.Conflict();
            }

            var job = new Job() { UniqueName = dto.UniqueName, Title = dto.Title, Type = dto.Type, Parameters = JsonConvert.SerializeObject(dto.Parameters), };
            var returnJob = this.service.AddJob(job);

            return this.Created("/api/jobs/" + job.Id, this.Map(returnJob));
        }

        [HttpPost]
        [Route("api/jobs/{jobId:long}")]
        public IHttpActionResult UpdateJob(long jobId, [FromBody] JobDto dto)
        {
            var existingJob = this.jobStorageProvider.GetJobById(jobId);

            if (existingJob == null)
            {
                return this.NotFound();
            }

            return this.InternalServerError(new NotImplementedException());
        }

        [HttpGet]
        [Route("api/jobs/{jobId:long}/runs")]
        public IHttpActionResult GetJobRunsForJobById(long jobId)
        {
            var jobRuns = this.jobStorageProvider.GetJobRuns().Where(jr => jr.JobId == jobId);

            var list = jobRuns.Select(MapJobRunForOverview).ToList();

            return this.Ok(list);
        }

        [HttpGet]
        [Route("api/jobs/{uniqueName}/runs")]
        public IHttpActionResult GetJobRunsForJobByUniqueName(string uniqueName)
        {
            var job = this.jobStorageProvider.GetJobByUniqueName(uniqueName);

            var jobRuns = this.jobStorageProvider.GetJobRuns().Where(jr => jr.JobId == job.Id);

            var list = jobRuns.Select(MapJobRunForOverview).ToList();

            return this.Ok(list);
        }

        private JobDto Map(Job job)
        {
            return new JobDto()
                       {
                           Id = job.Id,
                           UniqueName = job.UniqueName,
                           Title = job.Title,
                           Parameters = JsonConvert.DeserializeObject(job.Parameters),
                           Type = job.Type,
                           UpdatedDateTimeUtc = job.UpdatedDateTimeUtc,
                           CreatedDateTimeUtc = job.CreatedDateTimeUtc
                       };
        }

        private static JobRunDto MapJobRunForOverview(JobRun jobRun)
        {
            return new JobRunDto()
            {
                JobRunId = jobRun.Id,
                JobId = jobRun.JobId,
                TriggerId = jobRun.TriggerId,
                UniqueId = new Guid(jobRun.UniqueId),
                State = jobRun.State.ToString(),
                Progress = jobRun.Progress,
                PlannedStartUtc = jobRun.PlannedStartDateTimeUtc,
                AuctualStartUtc = jobRun.ActualStartDateTimeUtc,
                AuctualEndUtc = jobRun.ActualEndDateTimeUtc,
            };
        }
    }
}
