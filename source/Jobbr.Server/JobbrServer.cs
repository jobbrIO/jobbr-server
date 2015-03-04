using System;

using Jobbr.Server.Web;

using Microsoft.Owin.Hosting;

namespace Jobbr.Server
{
    /// <summary>
    /// The jobber job server.
    /// </summary>
    public class JobbrServer : IDisposable
    {
        private readonly JobbrConfiguration configuration;

        private IDisposable web;

        public JobbrServer(JobbrConfiguration configuration)
        {
            Configuration = configuration;
        }

        internal static JobbrConfiguration Configuration { get; set; }

        public void Start()
        {
            this.web = WebApp.Start<Startup>(Configuration.BackendAddress);
        }

        public void Stop()
        {
            this.web.Dispose();
        }

        public void Dispose()
        {
            this.Stop();

            this.web = null;
        }
    }
}
