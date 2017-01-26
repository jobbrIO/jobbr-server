using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Scheduling
{
    public class DefaultSchedulerConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Gets or sets the allow changes before start in sec.
        /// </summary>
        public int AllowChangesBeforeStartInSec { get; set; } = 5;

    }
}
