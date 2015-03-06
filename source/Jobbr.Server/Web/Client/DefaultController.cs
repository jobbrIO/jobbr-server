using System;
using System.Net;
using System.Web.Http;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Core;

using Newtonsoft.Json;

namespace Jobbr.Server.Web.Client
{
    /// <summary>
    /// The job run controller.
    /// </summary>
    public class JobRunController : ApiController
    {
        private readonly IJobService jobService;

        private readonly IJobbrStorageProvider storageProvider;

        public JobRunController(IJobService jobService, IJobbrStorageProvider storageProvider)
        {
            this.jobService = jobService;
            this.storageProvider = storageProvider;
        }

        [HttpGet]
        [Route("client/jobrun/{jobRunId}")]
        public IHttpActionResult GetJonbRunInfos(long jobRunId)
        {
            var jobRun = this.jobService.GetJobRun(jobRunId);

            if (jobRun == null)
            {
                return this.NotFound();
            }

            var trigger = this.storageProvider.GetTriggerById(jobRun.TriggerId);
            var job = this.storageProvider.GetJobById(jobRun.JobId);

            var infoDto = new JobRunInfoDto()
                              {
                                  JobId = job.Id,
                                  TriggerId = trigger.Id,
                                  JobRunId = jobRunId,
                                  JobName = job.Name,
                                  JobType = job.Type,
                                  TempDir = jobRun.TempDir,
                                  WorkingDir = jobRun.WorkingDir,
                                  UniqueId = new Guid(jobRun.UniqueId),
                                  JobParameter = jobRun.JobParameters != null ? JsonConvert.DeserializeObject(jobRun.JobParameters) : null,
                                  InstanceParameter = jobRun.InstanceParameters != null ? JsonConvert.DeserializeObject(jobRun.InstanceParameters) : null,
                              };
            
            return this.Ok(infoDto);
        }

        [HttpPut]
        [Route("client/jobrun/{jobRunId}")]
        public IHttpActionResult PutJobRunUpdate(long jobRunId, [FromBody] JobRunUpdateDto dto)
        {
            var jobRun = this.jobService.GetJobRun(jobRunId);

            if (jobRun == null)
            {
                return this.NotFound();
            }

            if (dto.State != JobRunState.Null)
            {
                this.jobService.UpdateJobRunState(jobRun, dto.State);
            }

            return this.StatusCode(HttpStatusCode.Accepted);
        }
    }
}
