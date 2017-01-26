using Jobbr.Server.Common;
using Jobbr.Server.Repository;

namespace Jobbr.Server.Configuration
{
    public class DefaultJobbrConfiguration : IJobbrConfiguration
    {
        public virtual void OnRepositoryCreating(RepositoryBuilder repositoryBuilder)
        {
        }
    }
}