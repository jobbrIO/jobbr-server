using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Jobbr.ComponentModel.Registration;
using Ninject;

namespace Jobbr.Server.ComponentServices.Registration
{
    /// <summary>
    /// The jobbr dependency resolver.
    /// </summary>
    [SuppressMessage("Design", "CA2213:Disposable fields should be disposed", Justification = "Cannot disopose kernel, because it's an external dependency")]
    public class JobbrServiceProvider : IJobbrServiceProvider
    {
        /// <summary>
        /// The ninject kernel.
        /// </summary>
        private readonly IKernel ninjectKernel;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobbrServiceProvider"/> class.
        /// </summary>
        /// <param name="ninjectKernel">
        /// The ninject kernel.
        /// </param>
        public JobbrServiceProvider(IKernel ninjectKernel)
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

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new List<object>(new[] { this.GetService(serviceType) });
        }

        #pragma warning disable CA1024 // Use properties where appropriate.
        public IJobbrServiceProvider GetChild()
        #pragma warning restore CA1024 // Use properties where appropriate.
        {
            // TODO: This might not be the right way to do
            return new JobbrServiceProvider(this.ninjectKernel);
        }
    }
}