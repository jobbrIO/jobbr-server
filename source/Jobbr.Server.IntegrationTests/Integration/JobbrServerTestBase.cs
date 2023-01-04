using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration
{
    public abstract class JobbrServerTestBase
    {
        private readonly JobbrServer _jobbrServer;

        protected JobbrServerTestBase(Func<JobbrServer> creator)
        {
            _jobbrServer = creator();
        }

        protected ExposeAllServicesComponent Services => ExposeAllServicesComponent.Instance;

        protected JobbrServer JobbrServer
        {
            get { return _jobbrServer; }
        }

        [TestCleanup]
        public void CleanUp()
        {
            _jobbrServer?.Stop();
            _jobbrServer?.Dispose();
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
            var builder = new JobbrBuilder(new NullLoggerFactory());

            builder.RegisterForCollection<IJobbrComponent>(typeof(ExposeAllServicesComponent));

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