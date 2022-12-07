using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Jobbr.ComponentModel.Registration;
using Microsoft.Extensions.Logging;

namespace Jobbr.Server
{
    /// <summary>
    /// Manages configuration for the Jobbr server.
    /// </summary>
    public class ConfigurationManager : IConfigurationManager
    {
        private readonly ILogger<ConfigurationManager> _logger;
        private readonly Collection<IConfigurationValidator> _configurationValidators;
        private readonly IJobbrServiceProvider _jobbrServiceProvider;
        private readonly Collection<IFeatureConfiguration> _featureConfigurations;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationManager"/> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="configurationValidators">Collection of validators for the configurations.</param>
        /// <param name="jobbrServiceProvider">Jobbr dependency resolver.</param>
        /// <param name="featureConfigurations">Configurations for the Jobbr server.</param>
        public ConfigurationManager(ILoggerFactory loggerFactory, Collection<IConfigurationValidator> configurationValidators, IJobbrServiceProvider jobbrServiceProvider, Collection<IFeatureConfiguration> featureConfigurations)
        {
            _logger = loggerFactory.CreateLogger<ConfigurationManager>();
            _configurationValidators = configurationValidators;
            _jobbrServiceProvider = jobbrServiceProvider;
            _featureConfigurations = featureConfigurations;
        }

        /// <inheritdoc/>
        public void LogConfiguration()
        {
            if (_featureConfigurations == null || !_featureConfigurations.Any())
            {
                _logger.LogDebug("Skipping printing configurations because there are feature configurations available.");
                return;
            }

            foreach (var config in _featureConfigurations)
            {
                var jsonSettings = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                var serialized = JsonSerializer.Serialize(config, jsonSettings);

                serialized = serialized.Replace("{", "[", StringComparison.CurrentCulture);
                serialized = serialized.Replace("}", "]", StringComparison.CurrentCulture);
                serialized = serialized.Replace(",\"", ", ", StringComparison.CurrentCulture);
                serialized = serialized.Replace("\":", ": ", StringComparison.CurrentCulture);
                serialized = serialized.Replace("[\"", "[", StringComparison.CurrentCulture);

                try
                {
                    _logger.LogInformation("{typeName} = {serialized}", config.GetType().Name, serialized);
                }
                catch (NullReferenceException)
                {
                    _logger.LogInformation("{typeName} = NOT DISPLAYABLE!", config.GetType().Name);
                }
            }
        }

        /// <inheritdoc/>
        public void ValidateConfigurationAndThrowOnErrors()
        {
            if (_configurationValidators == null || !_configurationValidators.Any())
            {
                _logger.LogDebug("Skipping validating configuration because there are no validators available.");
                return;
            }

            _logger.LogDebug("Validating the configuration...");

            var results = new Dictionary<Type, bool>();

            foreach (var validator in _configurationValidators)
            {
                var forType = validator.ConfigurationType;

                var config = _jobbrServiceProvider.GetService(forType);

                if (config == null)
                {
                    _logger.LogWarning("Unable to use validator '{validatorName}' because there are no compatible configurations (of Type '{configurationTypeName}') registered.", validator.GetType().FullName, forType.FullName);
                    continue;
                }

                _logger.LogDebug("Validating configuration of type '{configType}'", config.GetType());

                try
                {
                    var result = validator.Validate(config);

                    if (result)
                    {
                        _logger.LogInformation("Configuration '{configName}' has been validated successfully", config.GetType().Name);
                    }
                    else
                    {
                        _logger.LogWarning("Validation for Configuration '{configTypeName}' failed.", config.GetType().Name);
                    }

                    results.Add(forType, result);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Validator '{validatorName}' has failed while validation!", validator.GetType().FullName);
                    results.Add(forType, false);
                }
            }

            if (!results.Values.All(r => r))
            {
                throw new ArgumentNullException("Configuration failed for one or more configurations");
            }
        }
    }
}