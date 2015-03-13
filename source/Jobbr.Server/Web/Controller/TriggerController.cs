using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

using Jobbr.Server.Common;
using Jobbr.Server.Core;
using Jobbr.Server.Model;
using Jobbr.Server.Web.Dto;

using Newtonsoft.Json;

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
        [Route("api/jobs/{jobId:long}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            var job = this.jobStorageProvider.GetJobById(jobId);

            if (job == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.jobStorageProvider.GetTriggers(jobId));
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

            return this.Ok(this.jobStorageProvider.GetTriggers(job.Id));
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

            if (triggerDto == null)
            {
                return this.StatusCode(HttpStatusCode.BadRequest);
            }

            var trigger = this.ConvertToTrigger(triggerDto as dynamic);
            ((JobTriggerBase)trigger).JobId = jobId;

            this.jobService.AddTrigger(trigger);

            return this.Ok();
        }

        [Route("api/jobs/{uniqueName}/trigger")]
        public IHttpActionResult AddTriggerForJobUniqueName(string uniqueName, [FromBody] JobTriggerDtoBase triggerDto)
        {
            var job = this.jobStorageProvider.GetJobByUniqueName(uniqueName);

            if (job == null)
            {
                return this.NotFound();
            }

            if (triggerDto == null)
            {
                return this.StatusCode(HttpStatusCode.BadRequest);
            }

            var trigger = this.ConvertToTrigger(triggerDto as dynamic);
            ((JobTriggerBase)trigger).JobId = job.Id;

            this.jobService.AddTrigger(trigger);

            return this.Ok();
        }

        private JobTriggerBase AddBaseInfos(JobTriggerDtoBase dto, JobTriggerBase trigger)
        {
            trigger.Comment = dto.Comment;
            trigger.IsActive = dto.IsActive;
            trigger.Parameters = JsonConvert.SerializeObject(dto.Parameters);
            trigger.TriggerType = dto.TriggerType;
            trigger.UserDisplayName = dto.UserDisplayName;
            trigger.UserId = dto.UserId;
            trigger.UserName = dto.UserName;

            return trigger;
        }

        private RecurringTrigger ConvertToTrigger(RecurringTriggerDto dto)
        {
            var trigger = new RecurringTrigger() { TriggerType = RecurringTrigger.TypeName, Definition = dto.Definition, StartDateTimeUtc = dto.StartDateTimeUtc, EndDateTimeUtc = dto.EndDateTimeUtc };
            return (RecurringTrigger)this.AddBaseInfos(dto, trigger);
        }

        private ScheduledTrigger ConvertToTrigger(ScheduledTriggerDto dto)
        {
            var trigger = new ScheduledTrigger { TriggerType = ScheduledTrigger.TypeName, DateTimeUtc = dto.DateTimeUtc };
            return (ScheduledTrigger)this.AddBaseInfos(dto, trigger);
        }

        private InstantTrigger ConvertToTrigger(InstantTriggerDto dto)
        {
            var trigger = new InstantTrigger() { TriggerType = InstantTrigger.TypeName, DelayedMinutes = dto.DelayedMinutes };
            return (InstantTrigger)this.AddBaseInfos(dto, trigger);
        }
    }
}
