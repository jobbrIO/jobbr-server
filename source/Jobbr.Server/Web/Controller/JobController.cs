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
            var job = new Job() { Name = dto.Name, Type = dto.Type, Parameters = JsonConvert.SerializeObject(dto.Parameters), };

            var returnJob = this.service.AddJob(job);

            return this.Ok(this.Map(returnJob));
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/runs")]
        public IHttpActionResult GetJobRunsForJob(long jobId)
        {
            var jobRuns = this.jobStorageProvider.GetJobRuns().Where(jr => jr.JobId == jobId);

            var list = new List<JobRunDto>();

            foreach (var jobRun in jobRuns)
            {
                list.Add(new JobRunDto()
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
                             });
            }

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
    }
}
