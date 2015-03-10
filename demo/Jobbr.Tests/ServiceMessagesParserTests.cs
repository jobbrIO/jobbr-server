using Jobbr.Server.ServiceMessaging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests
{
    /// <summary>
    /// Based on: https://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity
    /// <code> 
    ///     ##jobbr[<messageName> name1='value1' name2='value2']
    /// </code>
    /// </summary>
    [TestClass]
    public class ServiceMessagesParserTests
    {
        [TestMethod]
        public void CanParseProgressMessage()
        {
            var parser = new ServiceMessageParser();

            var raw = "##jobbr[progress percent='55.34']";

            var message = (ProgressServiceMessage)parser.Parse(raw);

            Assert.IsNotNull(message);
            Assert.AreEqual(55.34, message.Percent);
        }
    }
}
