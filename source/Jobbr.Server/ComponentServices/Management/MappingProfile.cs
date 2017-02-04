using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Core;

namespace Jobbr.Server.ComponentServices.Management
{
    class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<RecurringTriggerModel, RecurringTrigger>();
        }
    }
}
