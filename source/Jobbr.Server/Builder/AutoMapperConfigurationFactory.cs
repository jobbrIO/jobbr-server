using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server.Builder
{
    /// <summary>
    /// Uses reflection to fetch AutoMapper configurations and initializes them.
    /// </summary>
    public class AutoMapperConfigurationFactory
    {
        private readonly ILogger<AutoMapperConfigurationFactory> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoMapperConfigurationFactory"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public AutoMapperConfigurationFactory(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<AutoMapperConfigurationFactory>();
        }

        /// <summary>
        /// Creates a new <see cref="MapperConfiguration"/> for AutoMapper.
        /// </summary>
        /// <returns><see cref="MapperConfiguration"/> containing all types that are in the 'Jobbr.Server' namespace.</returns>
        public MapperConfiguration GetNew()
        {
            var foundAutoMapperProfiles = new List<Profile>();

            var profileTypes = GetType().Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("Jobbr.Server") && typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract);
            var profileTypesList = profileTypes.ToList();

            _logger.LogDebug("Found {count} types that need to be registered in internal AutoMapper.", profileTypesList.Count);

            foreach (var profileType in profileTypesList)
            {
                _logger.LogDebug("Activating type '{name}' from namespace '{namespace}' in assembly '{assembly}'", profileType.Name, profileType.Namespace, profileType.Assembly);

                // Don't try/catch here, better fail early (in the creation of Jobbr server)
                var profile = (Profile)Activator.CreateInstance(profileType);
                foundAutoMapperProfiles.Add(profile);
            }

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in foundAutoMapperProfiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            config.AssertConfigurationIsValid();

            return config;
        }
    }
}