using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;

using Jobbr.Server.Logging;
using Jobbr.Server.Web.Dto;
using Jobbr.Shared;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Owin;

namespace Jobbr.Server.Web
{
    /// <summary>
    /// The OWIN startup class.
    /// </summary>
    public class Startup
    {
        private static readonly ILog Logger = LogProvider.For<WebHost>();

        /// <summary>
        /// The dependency resolver.
        /// </summary>
        private readonly IJobbrDependencyResolver dependencyResolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="dependencyResolver">
        /// The dependency resolver.
        /// </param>
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
            // Ensure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();
            var jsonSerializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver(), NullValueHandling = NullValueHandling.Ignore };

            jsonSerializerSettings.Converters.Add(new JsonTypeConverter<JobTriggerDtoBase>("TriggerType", this.JobTriggerTypeResolver));

            config.Formatters.JsonFormatter.SerializerSettings = jsonSerializerSettings;

            // Remove XML Responses
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            config.DependencyResolver = new DependencyResolverAdapter(this.dependencyResolver);

            config.Services.Add(typeof(IExceptionLogger), new TraceSourceExceptionLogger(Logger));

            app.UseWebApi(config);
        }

        private Type JobTriggerTypeResolver(List<Type> types, string typeValue)
        {
            if (typeValue.ToLowerInvariant() == RecurringTriggerDto.TypeName.ToLowerInvariant())
            {
                return typeof(RecurringTriggerDto);
            }

            if (typeValue.ToLowerInvariant() == ScheduledTriggerDto.TypeName.ToLowerInvariant())
            {
                return typeof(ScheduledTriggerDto);
            }

            if (typeValue.ToLowerInvariant() == InstantTriggerDto.TypeName.ToLowerInvariant())
            {
                return typeof(InstantTriggerDto);
            }

            return null;
        }
    }
}
