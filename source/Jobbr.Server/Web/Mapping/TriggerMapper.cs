using Jobbr.Server.Model;
using Jobbr.Server.Web.Dto;

using Newtonsoft.Json;

namespace Jobbr.Server.Web.Mapping
{
    public class TriggerMapper
    {
        internal static ScheduledTriggerDto ConvertToDto(ScheduledTrigger trigger)
        {
            var dto = new ScheduledTriggerDto { StartDateTimeUtc = trigger.StartDateTimeUtc};
            return (ScheduledTriggerDto)AddBaseInfos(trigger, dto);
        }

        internal static InstantTriggerDto ConvertToDto(InstantTrigger trigger)
        {
            var dto = new InstantTriggerDto { DelayedMinutes = trigger.DelayedMinutes };
            return (InstantTriggerDto)AddBaseInfos(trigger, dto);
        }

        internal static RecurringTriggerDto ConvertToDto(RecurringTrigger trigger)
        {
            var dto = new RecurringTriggerDto { StartDateTimeUtc = trigger.StartDateTimeUtc, EndDateTimeUtc = trigger.EndDateTimeUtc, Definition = trigger.Definition, };
            return (RecurringTriggerDto)AddBaseInfos(trigger, dto);
        }

        internal static RecurringTrigger ConvertToTrigger(RecurringTriggerDto dto)
        {
            var trigger = new RecurringTrigger() { TriggerType = RecurringTrigger.TypeName, Definition = dto.Definition, StartDateTimeUtc = dto.StartDateTimeUtc, EndDateTimeUtc = dto.EndDateTimeUtc };
            return (RecurringTrigger)AddBaseInfos(dto, trigger);
        }

        internal static ScheduledTrigger ConvertToTrigger(ScheduledTriggerDto dto)
        {
            var trigger = new ScheduledTrigger { TriggerType = ScheduledTrigger.TypeName, StartDateTimeUtc = dto.StartDateTimeUtc };
            return (ScheduledTrigger)AddBaseInfos(dto, trigger);
        }

        internal static InstantTrigger ConvertToTrigger(InstantTriggerDto dto)
        {
            var trigger = new InstantTrigger() { TriggerType = InstantTrigger.TypeName, DelayedMinutes = dto.DelayedMinutes };
            return (InstantTrigger)AddBaseInfos(dto, trigger);
        }

        internal static JobTriggerDtoBase AddBaseInfos(JobTriggerBase trigger, JobTriggerDtoBase dto)
        {
            dto.Id = trigger.Id;
            dto.Comment = trigger.Comment;
            dto.IsActive = trigger.IsActive;
            dto.Parameters = trigger.Parameters != null ? JsonConvert.DeserializeObject(trigger.Parameters) : null;
            dto.TriggerType = trigger.TriggerType;
            dto.UserDisplayName = trigger.UserDisplayName;
            dto.UserId = trigger.UserId;
            dto.UserName = trigger.UserName;

            return dto;
        }

        internal static JobTriggerBase AddBaseInfos(JobTriggerDtoBase dto, JobTriggerBase trigger)
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