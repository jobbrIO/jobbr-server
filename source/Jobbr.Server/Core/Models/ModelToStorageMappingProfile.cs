using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// AutoMapper profile for model classes.
    /// </summary>
    internal class ModelToStorageMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModelToStorageMappingProfile"/> class.
        /// </summary>
        public ModelToStorageMappingProfile()
        {
            CreateMap<RecurringTriggerModel, RecurringTrigger>();
            CreateMap<ScheduledTriggerModel, ScheduledTrigger>();
            CreateMap<InstantTriggerModel, InstantTrigger>();
            CreateMap<JobModel, Job>();
        }
    }
}
