﻿using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.Core.Models
{
    internal class ModelToStorageMappingProfile : Profile
    {
        public ModelToStorageMappingProfile()
        {
            this.CreateMap<RecurringTriggerModel, RecurringTrigger>();
            this.CreateMap<ScheduledTriggerModel, ScheduledTrigger>();
            this.CreateMap<InstantTriggerModel, InstantTrigger>();

            this.CreateMap<JobModel, Job>();
        }
    }
}
