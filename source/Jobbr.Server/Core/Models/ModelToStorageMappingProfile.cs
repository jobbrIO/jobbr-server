using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Core.Models
{
    internal class ModelToStorageMappingProfile : Profile
    {
        public ModelToStorageMappingProfile()
        {
            CreateMap<RecurringTriggerModel, RecurringTrigger>();
            CreateMap<ScheduledTriggerModel, ScheduledTrigger>();
            CreateMap<InstantTriggerModel, InstantTrigger>();

            CreateMap<JobModel, Job>();
        }
    }
}
