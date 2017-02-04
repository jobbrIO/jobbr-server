using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Core
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<RecurringTriggerModel, RecurringTrigger>();
        }
    }
}
