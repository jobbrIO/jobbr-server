using System.Web.Http;

using Jobbr.Common;
using Jobbr.Server.Core;

namespace Jobbr.Server.Web.Client
{
    /// <summary>
    /// The job run controller.
    /// </summary>
    public class JobRunController : ApiController
    {
        private readonly IJobService jobService;

        public JobRunController(IJobService jobService)
        {
            this.jobService = jobService;
        }

        [HttpPut]
        [Route("client/jobrun/{jobRunId}")]
        public IHttpActionResult PutJobRunUpdate(long jobRunId, [FromBody] JobRunUpdateDto dto)
        {
            var jobRun = this.jobService.GetJobRun(jobRunId);

            if (jobRun == null)
            {
                return this.NotFound();
            }

            if (dto.State != null)
            {
                this.jobService.UpdateJobRunState(jobRun, dto.State);
            }

            return this.Ok();
        }
    }
}
