using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Dto;

using Newtonsoft.Json;

namespace Jobbr.Server.Web.Api
{
    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        private readonly IJobStorageProvider jobStorageProvider;

        private readonly IArtefactsStorageProvider artefactsStorageProvider;

        public JobController(IJobStorageProvider jobStorageProvider, IArtefactsStorageProvider artefactsStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
            this.artefactsStorageProvider = artefactsStorageProvider;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobStorageProvider.GetJobs());
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            return this.Ok(this.jobStorageProvider.GetTriggers(jobId));
        }

        [HttpPost]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult AddTrigger(long jobId, [FromBody] JobTriggerBase trigger)
        {
            return this.Ok();
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

        [HttpGet]
        [Route("api/jobRuns/{jobRunId}")]
        public IHttpActionResult GetJonRun(long jobRunId)
        {
            var jobRun = this.jobStorageProvider.GetJobRuns().FirstOrDefault(jr => jr.Id == jobRunId);

            if (jobRun == null)
            {
                return this.NotFound();
            }

            var jobParameter = jobRun.JobParameters != null ? JsonConvert.DeserializeObject(jobRun.JobParameters) : null;
            var instanceParameter = jobRun.InstanceParameters != null ? JsonConvert.DeserializeObject(jobRun.InstanceParameters) : null;

            var files = this.artefactsStorageProvider.GetFiles(jobRun.UniqueId);
            var filesList = new List<JobRunArtefactDto>();

            foreach (var fileInfo in files)
            {
                filesList.Add(new JobRunArtefactDto()
                                  {
                                      Filename = fileInfo.Name,
                                      Size = fileInfo.Length,
                                  });

            }

            var dto = new JobRunDto()
                          {
                              JobRunId = jobRun.Id,
                              JobId = jobRun.JobId,
                              TriggerId = jobRun.TriggerId,
                              UniqueId = new Guid(jobRun.UniqueId),
                              JobParameter = jobParameter,
                              InstanceParameter = instanceParameter,
                              State = jobRun.State.ToString(),
                              Progress = jobRun.Progress,
                              PlannedStartUtc = jobRun.PlannedStartDateTimeUtc,
                              AuctualStartUtc = jobRun.ActualStartDateTimeUtc,
                              EstimatedEndtUtc = jobRun.EstimatedEndDateTimeUtc,
                              AuctualEndUtc = jobRun.ActualEndDateTimeUtc,
                              Artefacts = filesList
                          };
            
            return this.Ok(dto);
        }

        [HttpGet]
        [Route("api/jobRuns/{jobRunId}/{filename}")]
        public IHttpActionResult GetArtefact(long jobRunId, string filename)
        {
            var jobRun = this.jobStorageProvider.GetJobRuns().FirstOrDefault(jr => jr.Id == jobRunId);

            if (jobRun == null)
            {
                return this.NotFound();
            }

            var fileStream = this.artefactsStorageProvider.Load(jobRun.UniqueId, filename);

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);

            result.Content = new StreamContent(fileStream);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            return this.ResponseMessage(result);
        }

    }
}
