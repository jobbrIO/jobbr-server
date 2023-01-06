using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration
{
    public abstract class JobbrServerTestBase
    {
        protected JobbrServerTestBase(Func<JobbrServer> creator)
        {
            JobbrServer = creator();
        }

        protected ExposeAllServicesComponent Services => ExposeAllServicesComponent.Instance;

        protected JobbrServer JobbrServer { get; }

        [TestCleanup]
        public void CleanUp()
        {
            JobbrServer?.Stop();
            JobbrServer?.Dispose();
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
}