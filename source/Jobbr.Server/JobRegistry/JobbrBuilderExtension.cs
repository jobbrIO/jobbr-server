using System;
using Jobbr.ComponentModel.Registration;
using Microsoft.Extensions.Logging;

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
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="repository">Typed <see cref="Action"/> that contains the jobs.</param>
        /// <returns>The original <see cref="IJobbrBuilder"/> with the added jobs.</returns>
        public static IJobbrBuilder AddJobs(this IJobbrBuilder builder, ILoggerFactory loggerFactory, Action<RegistryBuilder> repository)
        {
            var repoBuilder = new RegistryBuilder(loggerFactory);

            repository(repoBuilder);

            builder.Add<IRegistryBuilder>(repoBuilder);

            return builder;
        }
    }
}
