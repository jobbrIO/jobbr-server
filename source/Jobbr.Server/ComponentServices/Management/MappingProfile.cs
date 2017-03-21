using AutoMapper;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Core.Models;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.AddMappingFromComponentToInternalModel();

            this.AddMappingFromStorageToComponentModel();
        }

        private void AddMappingFromStorageToComponentModel()
        {
            this.CreateMap<ComponentModel.JobStorage.Model.Job, Job>();
        }

        private void AddMappingFromComponentToInternalModel()
        {
            this.CreateMap<RecurringTrigger, RecurringTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<ScheduledTrigger, ScheduledTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<InstantTrigger, InstantTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<Job, JobModel>();
        }
    }
}
