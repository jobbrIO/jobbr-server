using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Jobbr.Server.Common;
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
        private readonly IJobStorageProvider jobStorageProvider;

        public JobController(IJobStorageProvider jobStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobStorageProvider.GetJobs());
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
    }
}
