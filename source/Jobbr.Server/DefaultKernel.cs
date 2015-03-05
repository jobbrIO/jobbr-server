using System.Linq;
using System.Reflection;

using Ninject;

namespace Jobbr.Server
{
    /// <summary>
    /// The kernel.
    /// </summary>
    public class DefaultKernel : StandardKernel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultKernel"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public DefaultKernel(JobbrConfiguration configuration)
        {
            this.Bind<IJobbrConfiguration>().ToConstant(configuration);
            this.Bind<IJobRepositoryProvider>().ToConstant(configuration.JobRepositoryProvider);
        }
    }
}
