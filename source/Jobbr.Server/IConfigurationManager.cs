namespace Jobbr.Server
{
    /// <summary>
    /// Interface for configuration managers.
    /// </summary>
    public interface IConfigurationManager
    {
        /// <summary>
        /// Logs the current configurations.
        /// </summary>
        void LogConfiguration();

        /// <summary>
        /// Validates configurations and throws errors if they are not valid.
        /// </summary>
        void ValidateConfigurationAndThrowOnErrors();
    }
}