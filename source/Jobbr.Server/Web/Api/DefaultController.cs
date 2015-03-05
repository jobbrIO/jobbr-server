using System.Web.Http;

using Jobbr.Server.Common;

namespace Jobbr.Server.Web.Api
{
    /// <summary>
    /// The default controller.
    /// </summary>
    public class DefaultController: ApiController
    {
        private readonly IJobbrConfiguration configuration;

        public DefaultController(IJobbrConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The are you fine.
        /// </summary>
        /// <returns>
        /// The <see cref="IHttpActionResult"/>.
        /// </returns>
        [HttpGet]
        [Route("api/status")]
        public IHttpActionResult AreYouFine()
        {
            return this.Ok("Fine");
        }

        [HttpGet]
        [Route("api/configuration")]
        public IHttpActionResult GetConfiguration()
        {
            return this.Ok(this.configuration);
        }

    }
}
