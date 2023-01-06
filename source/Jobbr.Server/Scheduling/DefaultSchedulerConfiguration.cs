using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Configuration for <see cref="DefaultScheduler"/>.
    /// </summary>
    public class DefaultSchedulerConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Gets or sets the allow changes before start in sec.
        /// </summary>
        public int AllowChangesBeforeStartInSec { get; set; } = 5;
    }
}
