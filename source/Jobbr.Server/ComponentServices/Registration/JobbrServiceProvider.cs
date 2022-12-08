using System;
using System.Collections.Generic;
using Jobbr.ComponentModel.Registration;
using SimpleInjector;

namespace Jobbr.Server.ComponentServices.Registration
{
    /// <summary>
    /// The Jobbr dependency resolver.
    /// </summary>
    public class JobbrServiceProvider : IJobbrServiceProvider
    {
        /// <summary>
        /// The dependency injection container.
        /// </summary>
        private readonly Container _container;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServiceProvider"/> class.
        /// </summary>
        /// <param name="container">
        /// The dependency injection container.
        /// </param>
        public JobbrServiceProvider(Container container)
        {
            _container = container;
        }

        /// <summary>
        /// Gets services based on the type.
        /// </summary>
        /// <param name="serviceType">Target service type.</param>
        /// <returns>An instance of the service as a generic object.</returns>
        public object GetService(Type serviceType)
        {
            return _container.GetInstance(serviceType);
        }

        /// <summary>
        /// Gets a service wrapped inside a list.
        /// </summary>
        /// <param name="serviceType">Target service type.</param>
        /// <returns>An instance of the service within a generic object list.</returns>
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>(new[] { GetService(serviceType) });
        }

        /// <summary>
        /// Creates a new provider.
        /// </summary>
        /// <returns>A new instance of <see cref="JobbrServiceProvider"/>.</returns>
        public IJobbrServiceProvider GetChild()
        {
            // If you need a request scoped container, please file a issue in GitHub.
            // Because the WebAPI is not request aware, we decided to keep this implementation
            return new JobbrServiceProvider(_container);
        }
    }
}