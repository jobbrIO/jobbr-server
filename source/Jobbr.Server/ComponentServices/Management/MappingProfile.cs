using System;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.ComponentModel.Management.Model;
using Jobbr.Server.Core.Models;
using JobRun = Jobbr.ComponentModel.Management.Model.JobRun;

namespace Jobbr.Server.ComponentServices.Management
{
    /// <summary>
    /// MappingProfiles for AutoMapper.
    /// </summary>
    internal class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            AddMappingFromComponentToInternalModel();
            AddMappingFromStorageToComponentModel();
            AddMappingsFromInternalToComponentModel();
        }

        private void AddMappingFromStorageToComponentModel()
        {
            CreateMap<ComponentModel.JobStorage.Model.Job, ComponentModel.Management.Model.Job>();
            CreateMap<JobTriggerBase, IJobTrigger>();

            CreateMap<ComponentModel.JobStorage.Model.RecurringTrigger, ComponentModel.Management.Model.RecurringTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();
            CreateMap<ComponentModel.JobStorage.Model.ScheduledTrigger, ComponentModel.Management.Model.ScheduledTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();
            CreateMap<ComponentModel.JobStorage.Model.InstantTrigger, ComponentModel.Management.Model.InstantTrigger>().IncludeBase<JobTriggerBase, IJobTrigger>();

            CreateMap<ComponentModel.JobStorage.Model.JobRun, JobRun>()
                .ForMember(m => m.Id, o => o.MapFrom(m => m.Id))
                .ForMember(m => m.TriggerType, o => o.MapFrom(m => m.Trigger.GetType().Name))
                .ForMember(m => m.JobType, o => o.MapFrom(m => m.Job.Type))
                .ForMember(m => m.ActualStartDateTimeUtc, o => o.MapFrom(m => m.ActualStartDateTimeUtc))
                .ForMember(m => m.ActualEndDateTimeUtc, o => o.MapFrom(m => m.ActualEndDateTimeUtc))
                .ForMember(m => m.Comment, o => o.MapFrom(m => m.Trigger.Comment))
                .ForMember(m => m.EstimatedEndDateTimeUtc, o => o.MapFrom(m => m.EstimatedEndDateTimeUtc))
                .ForMember(m => m.InstanceParameters, o => o.MapFrom(m => m.InstanceParameters))
                .ForMember(m => m.JobParameters, o => o.MapFrom(m => m.JobParameters))
                .ForMember(m => m.JobId, o => o.MapFrom(m => m.Job.Id))
                .ForMember(m => m.JobName, o => o.MapFrom(m => m.Job.UniqueName))
                .ForMember(m => m.PlannedStartDateTimeUtc, o => o.MapFrom(m => m.PlannedStartDateTimeUtc))
                .ForMember(m => m.Progress, o => o.MapFrom(m => m.Progress))
                .ForMember(m => m.State, o => o.MapFrom(r => (ComponentModel.Management.Model.JobRunStates)Enum.Parse(typeof(ComponentModel.Management.Model.JobRunStates), r.State.ToString())))
                .ForMember(m => m.TriggerId, o => o.MapFrom(m => m.Trigger.Id))
                .ForMember(m => m.UserId, o => o.MapFrom(m => m.Trigger.UserId))
                .ForMember(m => m.UserDisplayName, o => o.MapFrom(m => m.Trigger.UserDisplayName))
                .ForMember(m => m.Definition, o => o.MapFrom(r => (r.Trigger as ComponentModel.JobStorage.Model.RecurringTrigger).Definition));
        }

        private void AddMappingFromComponentToInternalModel()
        {
            CreateMap<ComponentModel.Management.Model.RecurringTrigger, RecurringTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, c => c.Ignore());

            CreateMap<ComponentModel.Management.Model.ScheduledTrigger, ScheduledTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, c => c.Ignore());

            CreateMap<ComponentModel.Management.Model.InstantTrigger, InstantTriggerModel>()
                .ForMember(d => d.CreatedDateTimeUtc, c => c.Ignore());

            CreateMap<ComponentModel.Management.Model.Job, JobModel>();
        }

        private void AddMappingsFromInternalToComponentModel()
        {
            CreateMap<JobArtefactModel, JobArtefact>()
                .ForMember(d => d.Type, cfg => cfg.MapFrom(s => s.MimeType));
        }
    }
}
