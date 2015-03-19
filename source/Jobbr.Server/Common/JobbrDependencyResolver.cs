using System;
using System.Collections.Generic;

using Jobbr.Shared;

using Ninject;

namespace Jobbr.Server.Common
{
    /// <summary>
    /// The jobbr dependency resolver.
    /// </summary>
    public class JobbrDependencyResolver : IJobbrDependencyResolver
    {
        /// <summary>
        /// The ninject kernel.
        /// </summary>
        private readonly IKernel ninjectKernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrDependencyResolver"/> class.
        /// </summary>
        /// <param name="ninjectKernel">
        /// The ninject kernel.
        /// </param>
        public JobbrDependencyResolver(DefaultKernel ninjectKernel)
        {
            this.ninjectKernel = ninjectKernel;
        }

        /// <summary>
        /// The get service.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            return this.ninjectKernel.TryGet(serviceType);
        }

        /// <summary>
        /// The get services.
        /// </summary>
        /// <param name="serviceType">
        /// The service type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>(new[] { this.GetService(serviceType) });
        }
    }
}