using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Core
{
    class ModelToStorageMappingProfile : Profile
    {
        public ModelToStorageMappingProfile()
        {
            this.CreateMap<RecurringTriggerModel, RecurringTrigger>();
            this.CreateMap<ScheduledTriggerModel, ScheduledTrigger>();
            this.CreateMap<InstantTriggerModel, InstantTrigger>()
                .ForMember(d => d.CreatedAtUtc, cgf => cgf.MapFrom(s => s.CreatedDateTimeUtc));
        }
    }
}
