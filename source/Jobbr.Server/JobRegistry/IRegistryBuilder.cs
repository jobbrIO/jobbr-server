using Jobbr.ComponentModel.JobStorage;
using System;

namespace Jobbr.Server.JobRegistry
{
    /// <summary>
    /// Interface for a registry builder.
    /// </summary>
    public interface IRegistryBuilder
    {
        /// <summary>
        /// Sets HasConfiguration to true and returns the current <see cref="RegistryBuilder"/>.
        /// </summary>
        /// <returns><see cref="RegistryBuilder"/> itself.</returns>
        RegistryBuilder RemoveAll();

        /// <summary>
        /// Sets isSingleSourceOfTruth to true.
        /// </summary>
        /// <returns><see cref="RegistryBuilder"/> itself.</returns>
        RegistryBuilder AsSingleSourceOfTruth();

        /// <summary>
        /// Adds a new <see cref="JobDefinition"/>.
        /// </summary>
        /// <param name="jobType">The job type that is being defined.</param>
        /// <param name="maxConcurrentJobRuns">The maximum amount of concurrent job runs.</param>
        /// <returns>The created <see cref="JobDefinition"/>.</returns>
        JobDefinition Define(Type jobType, int maxConcurrentJobRuns = 0);

        /// <summary>
        /// Adds a new <see cref="JobDefinition"/>.
        /// </summary>
        /// <param name="uniqueName">Name of the job definition.</param>
        /// <param name="typeName">Name of the target type.</param>
        /// <param name="maxConcurrentJobRuns">The maximum amount of concurrent job runs.</param>
        /// <returns>The created <see cref="JobDefinition"/>.</returns>
        JobDefinition Define(string uniqueName, string typeName, int maxConcurrentJobRuns = 0);

        /// <summary>
        /// Applies new <see cref="JobDefinition"/>s if available.
        /// </summary>
        /// <param name="storage">Storage provider where to apply the new <see cref="JobDefinition"/>s.</param>
        /// <returns>The number of changes.</returns>
        int Apply(IJobStorageProvider storage);
    }
}