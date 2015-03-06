using System.Web.Http;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Web.Api
{
    /// <summary>
    /// The job controller.
    /// </summary>
    public class JobController : ApiController
    {
        private readonly IJobStorageProvider jobStorageProvider;

        public JobController(IJobStorageProvider jobStorageProvider)
        {
            this.jobStorageProvider = jobStorageProvider;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.jobStorageProvider.GetJobs());
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            return this.Ok(this.jobStorageProvider.GetTriggers(jobId));
        }

        [HttpPost]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult AddTrigger(long jobId, [FromBody] JobTriggerBase trigger)
        {
            return this.Ok();
        }
    }
}
