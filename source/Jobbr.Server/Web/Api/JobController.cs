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
        private readonly IJobStorageProvider storageProvider;

        public JobController(IJobStorageProvider storageProvider)
        {
            this.storageProvider = storageProvider;
        }

        [HttpGet]
        [Route("api/jobs")]
        public IHttpActionResult AllJobs()
        {
            return this.Ok(this.storageProvider.GetJobs());
        }

        [HttpGet]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult GetTriggersForJob(long jobId)
        {
            return this.Ok(this.storageProvider.GetTriggers(jobId));
        }

        [HttpPost]
        [Route("api/jobs/{jobId}/trigger")]
        public IHttpActionResult AddTrigger(long jobId, [FromBody] JobTriggerBase trigger)
        {
            return this.Ok();
        }
    }
}
