namespace Jobbr.Server.IntegrationTests.Integration
{
    public class RunningJobbrServerTestBase : JobbrServerTestBase
    {
        public RunningJobbrServerTestBase() : base(GivenAStartedServer)
        {
        }
    }
}