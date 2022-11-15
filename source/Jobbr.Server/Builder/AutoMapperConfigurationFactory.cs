using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Jobbr.Server.Logging;

namespace Jobbr.Server.Builder
{
    public class AutoMapperConfigurationFactory
    {
        private static readonly ILog Logger = LogProvider.For<AutoMapperConfigurationFactory>();

        public MapperConfiguration GetNew()
        {
            var foundAutoMapperProfiles = new List<Profile>();

            var profileTypes = this.GetType().Assembly.GetTypes().Where(t => t.Namespace != null && t.Namespace.StartsWith("Jobbr.Server") && typeof(Profile).IsAssignableFrom(t) && !t.IsAbstract);
            var profileTypesList = profileTypes.ToList();

            Logger.Debug($"Found {profileTypesList.Count} types that need to be registered in internal AutoMapper.");

            foreach (var profileType in profileTypesList)
            {
                Logger.Debug($"Activating type '{profileType.Name}' from namespace '{profileType.Namespace}' in assembly '{profileType.Assembly}'");

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