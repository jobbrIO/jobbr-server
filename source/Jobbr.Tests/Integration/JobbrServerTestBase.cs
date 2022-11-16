using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration
{
    public abstract class JobbrServerTestBase
    {
        private readonly JobbrServer jobbrServer;

        protected JobbrServerTestBase(Func<JobbrServer> creator)
        {
            this.jobbrServer = creator();
        }

        protected ExposeAllServicesComponent Services => ExposeAllServicesComponent.Instance;

        protected JobbrServer JobbrServer
        {
            get { return this.jobbrServer; }
        }

        [TestCleanup]
        public void CleanUp()
        {
            this.jobbrServer?.Stop();
            this.jobbrServer?.Dispose();
        }

        [TestMethod]
        public void Infrastructure_Check()
        {
            Assert.IsNotNull(ExposeAllServicesComponent.Instance, "The component should have been constructed");
        }

        protected static JobbrServer GivenAStartedServer()
        {
            var server = GivenAServerInstance();
            server.Start();

            return server;
        }

        protected static JobbrServer GivenAServerInstance()
        {
            var builder = new JobbrBuilder(null);

            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            var server = builder.Create();
            return server;
        }
    }

    public class RunningJobbrServerTestBase : JobbrServerTestBase
    {
        public RunningJobbrServerTestBase() : base(GivenAStartedServer)
        {
        }
    }

    public class InitializedJobbrServerTestBase : JobbrServerTestBase
    {
        public InitializedJobbrServerTestBase() : base(GivenAServerInstance)
        {
        }
    }
}