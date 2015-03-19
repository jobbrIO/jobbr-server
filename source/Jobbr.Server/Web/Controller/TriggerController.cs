using System.Net;
using System.Web.Http;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Dto;
using Jobbr.Server.Web.Mapping;

namespace Jobbr.Server.Web.Controller
{
    public class TriggerController : ApiController
    {
        private readonly TriggerMapper triggerMapper;

        private IJobStorageProvider jobStorageProvider;

        private IJobService jobService;

        public TriggerController(IJobStorageProvider jobStorageProvider, IJobService jobService, IArtefactsStorageProvider artefactsStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
            this.jobService = jobService;
            this.triggerMapper = new TriggerMapper();
        }

        [HttpGet]
        [Route("api/triggers/{triggerId}")]
        public IHttpActionResult GetTriggerById(long triggerId)
        {
            var trigger = this.jobStorageProvider.GetTriggerById(triggerId);

            if (trigger == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.triggerMapper.ConvertToDto((dynamic)trigger));
        }

        [HttpGet]
        [Route("api/jobs/{jobId:long}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            var job = this.jobStorageProvider.GetJobById(jobId);

            if (job == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.jobStorageProvider.GetTriggersByJobId(jobId));
        }

        [HttpGet]
        [Route("api/jobs/{uniqueName}/trigger")]
        public IHttpActionResult GetTriggersForJob(string uniqueName)
        {
            var job = this.jobStorageProvider.GetJobByUniqueName(uniqueName);

            if (job == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.jobStorageProvider.GetTriggersByJobId(job.Id));
        }

        [HttpPost]
        [Route("api/jobs/{jobId:long}/trigger")]
        public IHttpActionResult AddTriggerForJobId(long jobId, [FromBody] JobTriggerDtoBase triggerDto)
        {
            var job = this.jobStorageProvider.GetJobById(jobId);

            if (job == null)
            {
                return this.NotFound();
            }

            return this.AddTrigger(triggerDto, job);
        }

        [Route("api/jobs/{uniqueName}/trigger")]
        public IHttpActionResult AddTriggerForJobUniqueName(string uniqueName, [FromBody] JobTriggerDtoBase triggerDto)
        {
            var job = this.jobStorageProvider.GetJobByUniqueName(uniqueName);

            if (job == null)
            {
                return this.NotFound();
            }

            return this.AddTrigger(triggerDto, job);
        }

        private IHttpActionResult AddTrigger(JobTriggerDtoBase triggerDto, Job job)
        {
            if (triggerDto == null)
            {
                return this.StatusCode(HttpStatusCode.BadRequest);
            }

            var trigger = this.triggerMapper.ConvertToTrigger(triggerDto as dynamic);
            ((JobTriggerBase)trigger).JobId = job.Id;

            var triggerId = this.jobService.AddTrigger(trigger);

            return this.Created(string.Format("api/trigger/{0}", triggerId), this.triggerMapper.ConvertToDto(trigger));
        }
    }
}
