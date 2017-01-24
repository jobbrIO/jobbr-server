using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server.Configuration;

namespace Jobbr.Server.Common
{
    // TODO: Split into different configuration classes

    /// <summary>
    /// The JobbrConfiguration interface.
    /// </summary>
    public interface IJobbrConfiguration
    {
        /// <summary>
        /// Gets or sets the allow changes before start in sec.
        /// </summary>
        int AllowChangesBeforeStartInSec { get; set; }

        void OnRepositoryCreating(RepositoryBuilder repositoryBuilder);
    }
}