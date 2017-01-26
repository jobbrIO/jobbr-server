using Jobbr.ComponentModel.Registration;
using Jobbr.Server;
using Jobbr.Server.Builder;
using Jobbr.Server.JobRegistry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Scheduler
{
    public class RunningJobbrServerTestBase
    {
        private JobbrServer jobbrServer;

        public RunningJobbrServerTestBase()
        {
            this.jobbrServer = GivenAStartedServer();
        }

        protected ExposeAllServicesComponent Services => ExposeAllServicesComponent.Instance;

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

        private static JobbrServer GivenAStartedServer()
        {
            var builder = new JobbrBuilder();

            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            var server = builder.Create();
            server.Start();

            return server;
        }
    }
}