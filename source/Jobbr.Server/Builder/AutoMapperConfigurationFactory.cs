using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Jobbr.Server.Logging;

namespace Jobbr.Server.Builder
{
    public class AutoMapperConfigurationFactory
    {
        private static ILog Logger = LogProvider.For<AutoMapperConfigurationFactory>();

        public MapperConfiguration GetNew()
        {
            var profileTypes = this.GetType().Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("Jobbr.Server") && typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract);

            var profiles = new List<Profile>();

            Logger.Debug($"Found {profileTypes.Count()} types that need to be registered in internal AutoMapper.");

            foreach (var profileType in profileTypes)
            {
                Logger.Debug($"Activating type '{profileType.Name}' from namespace '{profileType.Namespace}' in assembly '{profileType.Assembly}'");

                // Don't try/catch here, better fail early (in the creation of Jobbr server)
                var profile = (Profile) Activator.CreateInstance(profileType);
                profiles.Add(profile);
            }

            var config = new MapperConfiguration(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            config.AssertConfigurationIsValid();
            return config;
        }
    }
}