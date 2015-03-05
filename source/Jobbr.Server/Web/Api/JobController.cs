using System.Web.Http;

using Jobbr.Server.Model;

namespace Jobbr.Server.Web.Api
{
    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        private readonly IJobRepositoryProvider jobRepository;

        public JobController(IJobRepositoryProvider jobRepository)
        {
            this.jobRepository = jobRepository;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobRepository.GetJobs());
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            return this.Ok(this.jobRepository.GetTriggers(jobId));
        }

        [HttpPost]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult AddTrigger(long jobId, [FromBody] JobTriggerBase trigger)
        {
            return this.Ok();
        }
    }
}
