using AutoMapper;
using Jobbr.ComponentModel.ArtefactStorage.Model;

namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// AutoMapper profile for artifacts.
    /// </summary>
    internal class StorageToModelMappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StorageToModelMappingProfile"/> class.
        /// </summary>
        public StorageToModelMappingProfile()
        {
            CreateMap<JobbrArtefact, JobArtefactModel>();
        }
    }
}
