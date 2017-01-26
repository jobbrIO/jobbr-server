using System;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.JobRegistry
{
    public static class JobbrBuilderExtension
    {
        public static IJobbrBuilder AddJobs(this IJobbrBuilder builder, Action<RegistryBuilder> repository)
        {
            var repoBuilder = new RegistryBuilder();

            repository(repoBuilder);

            builder.Add<RegistryBuilder>(repoBuilder);

            return builder;
        }
    }
}
