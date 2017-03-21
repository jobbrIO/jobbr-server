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

            this.CreateMap<ComponentModel.JobStorage.Model.RecurringTrigger, RecurringTrigger>();
            this.CreateMap<ComponentModel.JobStorage.Model.ScheduledTrigger, ScheduledTrigger>();
            this.CreateMap<ComponentModel.JobStorage.Model.InstantTrigger, InstantTrigger>();

            this.CreateMap<ComponentModel.JobStorage.Model.RecurringTrigger, IJobTrigger>().ConstructUsing(u => new RecurringTrigger());
            this.CreateMap<ComponentModel.JobStorage.Model.ScheduledTrigger, IJobTrigger>().ConstructUsing(u => new ScheduledTrigger());
            this.CreateMap<ComponentModel.JobStorage.Model.InstantTrigger, IJobTrigger>().ConstructUsing(u => new InstantTrigger());

            this.CreateMap<ComponentModel.JobStorage.Model.JobRun, JobRun>();
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
