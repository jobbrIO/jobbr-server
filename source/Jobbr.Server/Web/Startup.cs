using System;
using System.Linq;
using System.Web.Http;

using Jobbr.Common;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Owin;

namespace Jobbr.Server.Web
{
    /// <summary>
    /// The api startup.
    /// </summary>
    public class Startup
    {
        private readonly IJobbrDependencyResolver dependencyResolver;

        public Startup(IJobbrDependencyResolver dependencyResolver)
        {
            if (dependencyResolver == null)
            {
                throw new ArgumentException("Please provide a dependency resolver. See http://servercoredump.com/question/27246240/inject-current-user-owin-host-web-api-service for details", "dependencyResolver");
            }

            this.dependencyResolver = dependencyResolver;
        }

        /// <summary>
        /// The configuration.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        public void Configuration(IAppBuilder app)
        {
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };

            // Remove XML
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.DependencyResolver = new DependencyResolverAdapter(this.dependencyResolver);

            app.UseWebApi(config);
        }
    }
}
