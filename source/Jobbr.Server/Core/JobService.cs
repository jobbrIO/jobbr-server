using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Storage;

namespace Jobbr.Server.Core
{
    public class JobService
    {
        private readonly IJobbrRepository repository;
        private readonly IMapper mapper;

        public JobService(IJobbrRepository repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public JobModel Add(JobModel model)
        {
            var entity = this.mapper.Map<Job>(model);

            var id = this.repository.AddJob(entity);

            model.Id = id;

            return model;
        }
    }
}