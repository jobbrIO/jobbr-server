using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server.Configuration;
using Jobbr.Server.Repository;

namespace Jobbr.Server.Common
{
    // TODO: Split into different configuration classes

    /// <summary>
    /// The JobbrConfiguration interface.
    /// </summary>
    public interface IJobbrConfiguration
    {
        void OnRepositoryCreating(RepositoryBuilder repositoryBuilder);
    }
}