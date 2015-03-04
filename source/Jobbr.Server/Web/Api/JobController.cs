using System.Web.Http;

namespace Jobbr.Server.Web.Api
{
    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            var repo = JobbrServer.Configuration.JobRepositoryProvider;

            return this.Ok(repo.GetJobs());
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            var repo = JobbrServer.Configuration.JobRepositoryProvider;

            return this.Ok(repo.GetTriggers(jobId));
        }
    }
}
