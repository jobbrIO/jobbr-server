namespace Jobbr.Server.IntegrationTests.Integration
{
    public class InitializedJobbrServerTestBase : JobbrServerTestBase
    {
        public InitializedJobbrServerTestBase() : base(GivenAServerInstance)
        {
        }
    }
}