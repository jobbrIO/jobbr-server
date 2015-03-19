using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

using Jobbr.Shared;

namespace Jobbr.Server.Web
{
    public class DependencyResolverAdapter : IDependencyResolver
    {
        private readonly IJobbrDependencyResolver dependencyResolver;


        public DependencyResolverAdapter(IJobbrDependencyResolver dependencyResolver)
        {
            this.dependencyResolver = dependencyResolver;
        }

        public void Dispose()
        {
        }

        public object GetService(Type serviceType)
        {
            return this.dependencyResolver.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.dependencyResolver.GetServices(serviceType);
        }

        public IDependencyScope BeginScope()
        {
            return this;
        }
    }
}