using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;

namespace Jobbr.Server.Core.Models
{
    internal class StorageToModelMappingProfile : Profile
    {
        public StorageToModelMappingProfile()
        {
            this.CreateMap<JobbrArtefact, JobArtefactModel>();
        }
    }
}
