using AutoMapper;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Models;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class ComponentToCoreMappingProfile : Profile
    {
        public ComponentToCoreMappingProfile()
        {
            this.CreateMap<RecurringTrigger, RecurringTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<ScheduledTrigger, ScheduledTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<InstantTrigger, InstantTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());
        }
    }
}
