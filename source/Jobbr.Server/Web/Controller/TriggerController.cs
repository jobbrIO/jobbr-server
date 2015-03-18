using System.Net;
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
        [Route("api/triggers/{triggerId}")]
        public IHttpActionResult GetTriggerById(long triggerId)
        {
            var trigger = this.jobStorageProvider.GetTriggerById(triggerId);

            if (trigger == null)
            {
                return this.NotFound();
            }

            return this.Ok(this.ConvertToDto((dynamic)trigger));
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

            var trigger = this.ConvertToTrigger(triggerDto as dynamic);
            ((JobTriggerBase)trigger).JobId = job.Id;

            var triggerId = this.jobService.AddTrigger(trigger);

            return this.Created(string.Format("api/trigger/{0}", triggerId), this.ConvertToDto(trigger));
        }

        private ScheduledTriggerDto ConvertToDto(ScheduledTrigger trigger)
        {
            var dto = new ScheduledTriggerDto { StartDateTimeUtc = trigger.StartDateTimeUtc};
            return (ScheduledTriggerDto)this.AddBaseInfos(trigger, dto);
        }

        private InstantTriggerDto ConvertToDto(InstantTrigger trigger)
        {
            var dto = new InstantTriggerDto { DelayedMinutes = trigger.DelayedMinutes };
            return (InstantTriggerDto)this.AddBaseInfos(trigger, dto);
        }

        private RecurringTriggerDto ConvertToDto(RecurringTrigger trigger)
        {
            var dto = new RecurringTriggerDto { StartDateTimeUtc = trigger.StartDateTimeUtc, EndDateTimeUtc = trigger.EndDateTimeUtc, Definition = trigger.Definition, };
            return (RecurringTriggerDto)this.AddBaseInfos(trigger, dto);
        }

        private RecurringTrigger ConvertToTrigger(RecurringTriggerDto dto)
        {
            var trigger = new RecurringTrigger() { TriggerType = RecurringTrigger.TypeName, Definition = dto.Definition, StartDateTimeUtc = dto.StartDateTimeUtc, EndDateTimeUtc = dto.EndDateTimeUtc };
            return (RecurringTrigger)this.AddBaseInfos(dto, trigger);
        }

        private ScheduledTrigger ConvertToTrigger(ScheduledTriggerDto dto)
        {
            var trigger = new ScheduledTrigger { TriggerType = ScheduledTrigger.TypeName, StartDateTimeUtc = dto.StartDateTimeUtc };
            return (ScheduledTrigger)this.AddBaseInfos(dto, trigger);
        }

        private InstantTrigger ConvertToTrigger(InstantTriggerDto dto)
        {
            var trigger = new InstantTrigger() { TriggerType = InstantTrigger.TypeName, DelayedMinutes = dto.DelayedMinutes };
            return (InstantTrigger)this.AddBaseInfos(dto, trigger);
        }

        private JobTriggerDtoBase AddBaseInfos(JobTriggerBase trigger, JobTriggerDtoBase dto)
        {
            dto.Id = trigger.Id;
            dto.Comment = trigger.Comment;
            dto.IsActive = trigger.IsActive;
            dto.Parameters = JsonConvert.DeserializeObject(trigger.Parameters);
            dto.TriggerType = trigger.TriggerType;
            dto.UserDisplayName = trigger.UserDisplayName;
            dto.UserId = trigger.UserId;
            dto.UserName = trigger.UserName;

            return dto;
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
    }
}
