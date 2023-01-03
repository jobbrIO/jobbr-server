using AutoMapper;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.ComponentServices.Execution
{
    internal class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Job, JobRunInfo>()
                .ForMember(d => d.JobId, c => c.MapFrom(s => s.Id))
                .ForMember(d => d.JobParameters, c => c.MapFrom(s => s.Parameters))

                // Mapped from JobRun
                .ForMember(d => d.Id, c => c.Ignore())
                .ForMember(d => d.InstanceParameters, c => c.Ignore())
                .ForMember(d => d.TriggerId, c => c.Ignore())
                .ForMember(d => d.UserDisplayName, c => c.Ignore())

                // Mapped from trigger
                .ForMember(d => d.UserId, c => c.Ignore());

            CreateMap<JobRun, JobRunInfo>()
                .ForMember(d => d.InstanceParameters, c => c.MapFrom(s => s.InstanceParameters))

                // Mapped from the Job
                .ForMember(d => d.Type, c => c.Ignore())
                .ForMember(d => d.UniqueName, c => c.Ignore())

                .ForMember(d => d.UserId, c => c.Ignore())
                .ForMember(d => d.UserDisplayName, c => c.Ignore())
                ;

            CreateMap<JobTriggerBase, JobRunInfo>()

                .ForMember(d => d.TriggerId, c => c.MapFrom(s => s.Id))

                // Mapped from the Job
                .ForMember(d => d.Type, c => c.Ignore())
                .ForMember(d => d.UniqueName, c => c.Ignore())
                .ForMember(d => d.JobParameters, c => c.Ignore())

                // Mapped from JobRun
                .ForMember(d => d.InstanceParameters, c => c.Ignore())
                .ForMember(d => d.Type, c => c.Ignore());
        }
    }
}
