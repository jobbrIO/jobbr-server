using System;
using System.Linq;
using System.Web.Http;

using Jobbr.Common.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Mapping;

using Newtonsoft.Json;

namespace Jobbr.Server.Web.Controller
{
    using System.Net;

    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        private readonly IStateService service;

        private readonly IJobManagementService jobManagementService;

        public JobController(IStateService service, IJobManagementService jobManagementService)
        {
            this.service = service;
            this.jobManagementService = jobManagementService;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobManagementService.GetAllJobs().Select(JobMapper.Map));
        }

        [HttpGet]
        [Route("api/jobs/{jobId}")]
        public IHttpActionResult SingleJob(long jobId)
        {
            var job = this.jobManagementService.GetJobById(jobId);

            var triggers = this.jobManagementService.GetTriggers(jobId);
            
            var jobDto = JobMapper.Map(job);
            jobDto.Trigger = triggers.Select(t => TriggerMapper.ConvertToDto(t as dynamic)).Cast<JobTriggerDtoBase>().ToList();

            return this.Ok(jobDto);
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

            var existingJob = this.jobManagementService.GetJobByUniqueName(identifier);

            if (existingJob != null)
            {
                return this.Conflict();
            }

            var job = new Job() { UniqueName = dto.UniqueName, Title = dto.Title, Type = dto.Type, Parameters = dto.Parameters != null ? JsonConvert.SerializeObject(dto.Parameters) : null, };
            var returnJob = this.jobManagementService.AddJob(job);

            return this.Created("/api/jobs/" + job.Id, JobMapper.Map(returnJob));
        }

        [HttpPost]
        [Route("api/jobs/{jobId:long}")]
        public IHttpActionResult UpdateJob(long jobId, [FromBody] JobDto dto)
        {
            var existingJob = this.jobManagementService.GetJobById(jobId);

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
            var jobRuns = this.jobManagementService.GetJobRuns().Where(jr => jr.JobId == jobId);

            var list = jobRuns.Select(JobMapper.Map).ToList();

            return this.Ok(list);
        }

        [HttpGet]
        [Route("api/jobs/{uniqueName}/runs")]
        public IHttpActionResult GetJobRunsForJobByUniqueName(string uniqueName)
        {
            var job = this.jobManagementService.GetJobByUniqueName(uniqueName);

            var jobRuns = this.jobManagementService.GetJobRuns().Where(jr => jr.JobId == job.Id);

            var list = jobRuns.Select(JobMapper.Map).ToList();

            return this.Ok(list);
        }
    }
}
