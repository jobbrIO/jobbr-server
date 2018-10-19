using System;
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

            this.repository.AddJob(entity);
            model.Id = entity.Id;

            return model;
        }

        public void Update(JobModel model)
        {
            var entity = this.mapper.Map<Job>(model);

            var fromDb = this.repository.GetJob(model.Id);

            fromDb.Parameters = entity.Parameters;
            fromDb.Title = entity.Title;
            fromDb.UpdatedDateTimeUtc = DateTime.UtcNow;

            throw new NotImplementedException();
        }

        public void Delete(long id)
        {
            var job = this.repository.GetJob(id);
            job.Deleted = true;
        }
    }
}