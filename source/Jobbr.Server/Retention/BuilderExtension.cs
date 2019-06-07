using System;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Retention
{
    public static class BuilderExtension
    {
        public static void AddRetention(this IJobbrBuilder builder)
        {
            builder.AddRetention(config => { });
        }

        public static void AddRetention(this IJobbrBuilder builder, Action<RetentionConfiguration> config)
        {
            var defaultConfig = new RetentionConfiguration();
            config(defaultConfig);
            builder.Add<RetentionConfiguration>(defaultConfig);
            builder.Register<IJobbrComponent>(typeof(RetentionComponent));
        }
    }
}