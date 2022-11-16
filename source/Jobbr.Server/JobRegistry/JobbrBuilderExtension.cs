using System;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.JobRegistry
{
    /// <summary>
    /// Extensions for <see cref="IJobbrBuilder"/>.
    /// </summary>
    public static class JobbrBuilderExtension
    {
        /// <summary>
        /// Extract jobs from given <see cref="Action"/> and add to a <see cref="IJobbrBuilder"/>.
        /// </summary>
        /// <param name="builder"><see cref="IJobbrBuilder"/> where the jobs are added.</param>
        /// <param name="repository">Typed <see cref="Action"/> that contains the jobs.</param>
        /// <returns>The original <see cref="IJobbrBuilder"/> with the added jobs.</returns>
        public static IJobbrBuilder AddJobs(this IJobbrBuilder builder, Action<RegistryBuilder> repository)
        {
            var repoBuilder = new RegistryBuilder(null);

            repository(repoBuilder);

            builder.Add<RegistryBuilder>(repoBuilder);

            return builder;
        }
    }
}
