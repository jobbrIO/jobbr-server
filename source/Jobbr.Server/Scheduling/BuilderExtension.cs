using System;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Extensions for <see cref="IJobbrBuilder"/>.
    /// </summary>
    internal static class BuilderExtension
    {
        /// <summary>
        /// Add a <see cref="DefaultScheduler"/> to the builder without configurations.
        /// </summary>
        /// <param name="builder">Target builder.</param>
        public static void AddDefaultScheduler(this IJobbrBuilder builder)
        {
            AddDefaultScheduler(builder, _ => { });
        }

        /// <summary>
        /// Add a <see cref="DefaultScheduler"/> to the builder with configurations.
        /// </summary>
        /// <param name="builder">Target builder.</param>
        /// <param name="config">Configurations.</param>
        public static void AddDefaultScheduler(this IJobbrBuilder builder, Action<DefaultSchedulerConfiguration> config)
        {
            var defaultConfig = new DefaultSchedulerConfiguration();

            config(defaultConfig);

            builder.Add<DefaultSchedulerConfiguration>(defaultConfig);

            builder.Register<IPeriodicTimer>(typeof(FixedMinuteTimer));
            builder.Register<IDateTimeProvider>(typeof(UtcNowTimeProvider));
            builder.Register<IJobScheduler>(typeof(DefaultScheduler));
        }
    }
}
