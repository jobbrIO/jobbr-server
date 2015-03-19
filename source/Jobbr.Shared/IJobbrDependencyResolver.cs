using System;
using System.Collections.Generic;

namespace Jobbr.Shared
{
    /// <summary>
    /// Represents an interface for the range of the dependencies.
    /// </summary>
    public interface IJobbrDependencyResolver
    {
        /// <summary>
        /// Retrieves a service from the scope.
        /// </summary>
        /// <param name="serviceType">The service to be retrieved.</param>
        /// <returns>The retrieved service.</returns>
        object GetService(Type serviceType);
        
        /// <summary>
        /// Retrieves a collection of services from the scope.
        /// </summary>
        /// <param name="serviceType"> The collection of services to be retrieved.</param>
        /// <returns>The retrieved collection of services.</returns>
        IEnumerable<object> GetServices(Type serviceType);
    }
}
