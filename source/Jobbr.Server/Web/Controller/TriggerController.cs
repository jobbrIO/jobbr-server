using System;
using System.Net;
using System.Web.Http;

using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Mapping;

namespace Jobbr.Server.Web.Controller
{
    public class TriggerController : ApiController
    {
        private IJobStorageProvider jobStorageProvider;

        private IJobService jobService;

        public TriggerController(IJobStorageProvider jobStorageProvider, IJobService jobService, IArtefactsStorageProvider artefactsStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
            this.jobService = jobService;
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

            return this.Ok(TriggerMapper.ConvertToDto((dynamic)trigger));
        }

        [HttpPatch]
        [Route("api/triggers/{triggerId}")]
        public IHttpActionResult UpdateTrigger(long triggerId, [FromBody] JobTriggerDtoBase dto)
        {
            var trigger = this.jobStorageProvider.GetTriggerById(triggerId);

            if (trigger == null)
            {
                return this.NotFound();
            }

            bool hadChanges = false;
            if (trigger.IsActive && !dto.IsActive)
            {
                trigger.IsActive = false;
                this.jobService.DisableTrigger(trigger.Id, true);
                hadChanges = true;
            }
            else if (!trigger.IsActive && dto.IsActive)
            {
                trigger.IsActive = true;
                this.jobService.EnableTrigger(trigger.Id);
                hadChanges = true;
            }

            var recurringTriggerDto = dto as RecurringTriggerDto;
            if (recurringTriggerDto != null && !string.IsNullOrEmpty(recurringTriggerDto.Definition) && recurringTriggerDto.Definition != ((RecurringTrigger)trigger).Definition)
            {
                ((RecurringTrigger)trigger).Definition = recurringTriggerDto.Definition;
                this.jobService.UpdateTrigger(trigger.Id, trigger);
                
                hadChanges = true;
            }

            var scheduledTriggerDto = dto as ScheduledTriggerDto;
            if (scheduledTriggerDto != null && scheduledTriggerDto.StartDateTimeUtc >= DateTime.UtcNow && scheduledTriggerDto.StartDateTimeUtc != ((ScheduledTrigger)trigger).StartDateTimeUtc)
            {
                ((ScheduledTrigger)trigger).StartDateTimeUtc = scheduledTriggerDto.StartDateTimeUtc;
                this.jobService.UpdateTrigger(trigger.Id, trigger);

                hadChanges = true;
            }

            if (hadChanges)
            {
                return this.Ok(TriggerMapper.ConvertToDto((dynamic)trigger));
            }
            
            return this.StatusCode(HttpStatusCode.NotModified);
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

            if (triggerDto is InstantTriggerDto)
            {
                triggerDto.IsActive = true;
            }

            var trigger = TriggerMapper.ConvertToTrigger(triggerDto as dynamic);
            ((JobTriggerBase)trigger).JobId = job.Id;

            var triggerId = this.jobService.AddTrigger(trigger);

            return this.Created(string.Format("api/trigger/{0}", triggerId), TriggerMapper.ConvertToDto(trigger));
        }
    }
}
