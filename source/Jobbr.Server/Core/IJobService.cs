using Jobbr.Server.Core.Models;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Interface for <see cref="JobModel"/> services.
    /// </summary>
    public interface IJobService
    {
        /// <summary>
        /// Adds a job to the repository.
        /// </summary>
        /// <param name="model">Model that is used for the creation of the job.</param>
        /// <returns>Model that has the job ID.</returns>
        JobModel Add(JobModel model);

        /// <summary>
        /// Updates a job. Throws NotImplementedException.
        /// </summary>
        /// <param name="model">Model for the updated job.</param>
        void Update(JobModel model);

        /// <summary>
        /// Deletes a job from the repository.
        /// </summary>
        /// <param name="id">Job ID.</param>
        void Delete(long id);
    }
}