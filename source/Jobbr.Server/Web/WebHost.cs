using System;

using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Shared;

using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Services;
using Microsoft.Owin.Hosting.Starter;

namespace Jobbr.Server.Web
{
    /// <summary>
    /// The web host.
    /// </summary>
    public class WebHost
    {
        /// <summary>
        /// The logger.
        /// </summary>
        private static readonly ILog Logger = LogProvider.For<WebHost>();

        /// <summary>
        /// The dependency resolver.
        /// </summary>
        private readonly IJobbrDependencyResolver dependencyResolver;

        /// <summary>
        /// The web.
        /// </summary>
        private IDisposable web;

        /// <summary>
        /// The configuration.
        /// </summary>
        private IJobbrConfiguration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebHost"/> class.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
        public WebHost(IJobbrDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;

            this.configuration = (IJobbrConfiguration)this.dependencyResolver.GetService(typeof(IJobbrConfiguration));
        }

        /// <summary>
        /// The start.
        /// </summary>
        public void Start()
        {
            var services = (ServiceProvider)ServicesFactory.Create();
            var options = new StartOptions()
                              {
                                  Urls = {
                                            this.configuration.BackendAddress 
                                         }, 
                                  AppStartup = typeof(Startup).FullName
                              };

            services.Add(typeof(IJobbrDependencyResolver), () => this.dependencyResolver);

            var hostingStarter = services.GetService<IHostingStarter>();
            this.web = hostingStarter.Start(options); // constructs Startup instance internally

            Logger.InfoFormat("Started OWIN-Host (WebEndpoint) with for '{0}' at '{1}'", options.AppStartup, this.configuration.BackendAddress);
        }

        /// <summary>
        /// The stop.
        /// </summary>
        public void Stop()
        {
            Logger.InfoFormat("Stopping OWIN-Host for Web-Endpoints'");

            if (this.web != null)
            {
                this.web.Dispose();
            }

            this.web = null;
        }
    }
}