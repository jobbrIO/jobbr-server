using System;

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
    public class JobRunController : ApiController
    {
        private IJobStorageProvider jobStorageProvider;

        private IArtefactsStorageProvider artefactsStorageProvider;

        public JobRunController(IJobStorageProvider jobStorageProvider, IArtefactsStorageProvider artefactsStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
            this.artefactsStorageProvider = artefactsStorageProvider;
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

            var dto = this.ConvertToDto(jobRun);

            return this.Ok(dto);
        }

        [HttpGet]
        [Route("api/jobRuns/")]
        public IHttpActionResult GetJonRunsByUserId(long userId)
        {
            var jobRuns = this.jobStorageProvider.GetJobRunsForUserId(userId).OrderByDescending(r => r.Id);

            var jobRunDtos = jobRuns.Select(this.ConvertToDto);

            return this.Ok(jobRunDtos);
        }

        [HttpGet]
        [Route("api/jobRuns/")]
        public IHttpActionResult GetJonRunsByTriggerId(long triggerId)
        {
            var jobRuns = this.jobStorageProvider.GetJobRunsByTriggerId(triggerId);

            var jobRunDtos = jobRuns.Select(this.ConvertToDto);

            return this.Ok(jobRunDtos);
        }

        [HttpGet]
        [Route("api/jobRuns/")]
        public IHttpActionResult GetJonRunsByUserName(string userName)
        {
            var jobRuns = this.jobStorageProvider.GetJobRunsForUserName(userName).OrderByDescending(r => r.Id);

            var jobRunDtos = jobRuns.Select(this.ConvertToDto);

            return this.Ok(jobRunDtos);
        }

        [HttpGet]
        [Route("api/jobRuns/{jobRunId}/artefacts/{filename}")]
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
    
        private JobRunDto ConvertToDto(JobRun jobRun)
        {
            var jobParameter = jobRun.JobParameters != null ? JsonConvert.DeserializeObject(jobRun.JobParameters) : null;
            var instanceParameter = jobRun.InstanceParameters != null ? JsonConvert.DeserializeObject(jobRun.InstanceParameters) : null;

            var files = this.artefactsStorageProvider.GetFiles(jobRun.UniqueId);
            var filesList = files.Select(fileInfo => new JobRunArtefactDto() { Filename = fileInfo.Name, Size = fileInfo.Length, }).ToList();

            var job = this.jobStorageProvider.GetJobById(jobRun.JobId);

            var dto = new JobRunDto()
                          {
                              JobRunId = jobRun.Id,
                              JobId = jobRun.JobId,
                              JobName = job.UniqueName,
                              JobTitle = job.UniqueName,
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
                              Artefacts = filesList.Any() ? filesList : null
                          };
            return dto;
        }
    }
}
