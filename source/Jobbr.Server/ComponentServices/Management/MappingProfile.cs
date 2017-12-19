using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
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

            this.AddMappingsFromInternalToComponentModel();
        }

        private void AddMappingFromStorageToComponentModel()
        {
            this.CreateMap<ComponentModel.JobStorage.Model.Job, ComponentModel.Management.Model.Job>();
            this.CreateMap<JobTriggerBase, IJobTrigger>();

            this.CreateMap<ComponentModel.JobStorage.Model.RecurringTrigger, ComponentModel.Management.Model.RecurringTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();
            this.CreateMap<ComponentModel.JobStorage.Model.ScheduledTrigger, ComponentModel.Management.Model.ScheduledTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();
            this.CreateMap<ComponentModel.JobStorage.Model.InstantTrigger, ComponentModel.Management.Model.InstantTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();

            this.CreateMap<ComponentModel.JobStorage.Model.JobRun, ComponentModel.Management.Model.JobRun>();
        }

        private void AddMappingFromComponentToInternalModel()
        {
            this.CreateMap<ComponentModel.Management.Model.RecurringTrigger, RecurringTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<ComponentModel.Management.Model.ScheduledTrigger, ScheduledTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<ComponentModel.Management.Model.InstantTrigger, InstantTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, cgf => cgf.Ignore());

            this.CreateMap<ComponentModel.Management.Model.Job, JobModel>();
        }

        private void AddMappingsFromInternalToComponentModel()
        {
            this.CreateMap<JobArtefactModel, JobArtefact>()
                .ForMember(d => d.Type, cfg => cfg.ResolveUsing(s => s.MimeType));
        }
    }
}
