using System;

using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Scheduling
{
    internal static class BuilderExtension
    {
        public static void AddDefaultScheduler(this IJobbrBuilder builder)
        {
            AddForkedExecution(builder, configuration => { });
        }

        public static void AddForkedExecution(this IJobbrBuilder builder, Action<DefaultSchedulerConfiguration> config)
        {
            var defaultConfig = new DefaultSchedulerConfiguration();

            config(defaultConfig);

            builder.Add<DefaultSchedulerConfiguration>(defaultConfig);

            builder.Register<IJobScheduler>(typeof(DefaultScheduler));
        }
    }
}
