using System;

using Jobbr.Common;
using Jobbr.Server.Web;

using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Starter;

namespace Jobbr.Server
{
    /// <summary>
    /// The jobber job server.
    /// </summary>
    public class JobbrServer : IDisposable
    {
        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly JobbrConfiguration configuration;

        /// <summary>
        /// The web.
        /// </summary>
        private IDisposable web;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServer"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public JobbrServer(JobbrConfiguration configuration)
        {
            this.configuration = configuration;
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            var kernel = new DefaultKernel(this.configuration);

            var dependencyResolver = new JobbrDependencyResolver(kernel);
            
            var services = (ServiceProvider)ServicesFactory.Create();
            var options = new StartOptions()
                              {
                                  Urls = { this.configuration.BackendAddress }, 
                                  AppStartup = typeof(Startup).FullName
                              };

            services.Add(typeof(IJobbrDependencyResolver), () => dependencyResolver);

            var starter = services.GetService<IHostingStarter>();
            this.web = starter.Start(options); // constructs Startup instance internally
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            this.web.Dispose();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Stop();

            this.web = null;
        }
    }
}
