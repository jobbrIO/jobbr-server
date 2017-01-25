using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jobbr.Server
{
    public class ConfigurationManager
    {
        class IgnoreDelegatesFromSerializationContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (typeof(MulticastDelegate).IsAssignableFrom(property.PropertyType.BaseType))
                {
                    property.ShouldSerialize = o => false;
                }

                return property;
            }
        }

        private static readonly ILog Logger = LogProvider.For<ConfigurationManager>();

        private readonly List<IConfigurationValidator> configurationValidators;
        private readonly JobbrServiceProvider jobbrServiceProvider;
        private readonly List<IFeatureConfiguration> featureConfigurations;

        public ConfigurationManager(List<IConfigurationValidator> configurationValidators, JobbrServiceProvider jobbrServiceProvider, List<IFeatureConfiguration> featureConfigurations)
        {
            this.configurationValidators = configurationValidators;
            this.jobbrServiceProvider = jobbrServiceProvider;
            this.featureConfigurations = featureConfigurations;
        }

        public void LogConfiguration()
        {
            if (this.featureConfigurations == null || !this.featureConfigurations.Any())
            {
                Logger.Debug("Skipping printing configurations because there are feature configurations available.");
                return;
            }

            foreach (var config in this.featureConfigurations)
            {
                var jsonSettings = new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore, ContractResolver = new IgnoreDelegatesFromSerializationContractResolver() };
                var serialized = JsonConvert.SerializeObject(config, jsonSettings);
                
                serialized = serialized.Replace("{", "[");
                serialized = serialized.Replace("}", "]");
                serialized = serialized.Replace(",\"", ", ");
                serialized = serialized.Replace("\":", ": ");
                serialized = serialized.Replace("[\"", "[");

                try
                {
                    Logger.Info($"{config.GetType().Name} = " + serialized);
                }
                catch
                {
                    Logger.Info($"{config.GetType().Name} = " + "NOT DISPLAYABLE!");
                }
            }
        }

        public void ValidateConfigurationAndThrowOnErrors()
        {
            if (this.configurationValidators == null || !this.configurationValidators.Any())
            {
                Logger.Debug("Skipping validating configuration because there are no validators available.");
                return;
            }

            Logger.Debug("Validating the configuration...");

            var results = new Dictionary<Type, bool>();

            foreach (var validator in this.configurationValidators)
            {
                var forType = validator.ConfigurationType;

                var config = this.jobbrServiceProvider.GetService(forType);

                if (config == null)
                {
                    Logger.Warn($"Unable to use Validator '{validator.GetType().FullName}' because there are no compatible configurations (of Type '{forType.FullName}') registered.");
                    continue;
                }

                Logger.Debug($"Validating configuration of Type '{config.GetType()}'");

                try
                {
                    var result = validator.Validate(config);

                    if (result)
                    {
                        Logger.Info($"Configuration '{config.GetType().Name}' has been validated successfully");
                    }
                    else
                    {
                        Logger.Warn($"Validation for Configuration '{config.GetType().Name}' failed.");
                    }

                    results.Add(forType, result);
                }
                catch (Exception e)
                {
                    Logger.ErrorException($"Validator '{validator.GetType().FullName}' has failed while validation!", e);
                    results.Add(forType, false);
                }
            }

            if (!results.Values.All(r => r))
            {
                throw new Exception("Configuration failed for one or more configurations");
            }

            // TODO: Move validation to API Feature
            //if (string.IsNullOrEmpty(this.configuration.BackendAddress))
            //{
            //    throw new ArgumentException("Please provide a backend address to host the api!");
            //}
        }
    }
}