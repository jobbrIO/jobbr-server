using System;
using System.Collections.Generic;
using Demo.Common;
using Demo.MyJobs;
using Jobbr.Runtime;
using Jobbr.Shared;
using Ninject;

namespace Demo.JobRunner
{
    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main(string[] args)
        {
            var container = new DemoContainerWithNInject();

            var jobbrRuntime = new JobbrRuntime(typeof(MinimalJob).Assembly, container);
            
            jobbrRuntime.Run(args);
        }
    }

    public class UserResolver : IUserResolver
    {
        private readonly RuntimeContext context;

        public UserResolver(RuntimeContext context)
        {
            this.context = context;
        }

        public string GetUserName()
        {
            return this.context.UserName;
        }
    }

    public class DemoContainerWithNInject : IJobbrDependencyRegistrator
    {
        private readonly StandardKernel kernel;

        public DemoContainerWithNInject()
        {
            this.kernel = new StandardKernel();

            this.kernel.Bind<MinimalJob>().ToSelf();
            this.kernel.Bind<ParameterizedlJob>().ToSelf();
            this.kernel.Bind<ProgressJob>().ToSelf();
            this.kernel.Bind<UserSpecificJob>().ToSelf();

            this.kernel.Bind<IUserResolver>().To<UserResolver>();
        }

        public object GetService(Type serviceType)
        {
            return this.kernel.Get(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return this.kernel.GetAll(serviceType);
        }

        public void RegisterInstance<T>(T instance)
        {
            this.kernel.Bind<T>().ToConstant(instance);
        }
    }
}
