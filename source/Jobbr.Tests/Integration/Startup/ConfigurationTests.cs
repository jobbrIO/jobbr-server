using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Startup
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoStorageProvider_IssuesWarnInLog()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var config = new CompleteJobberConfiguration();
            //config.JobStorageProvider = null;

            //var jobbr = new JobbrServer(config);

            //jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoArtefactsProvider_IssuesWarnInLog()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var config = new CompleteJobberConfiguration();
            //config.ArtefactStorageProvider = null;

            //var jobbr = new JobbrServer(config);

            //jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoExecutor_IssuesWarnInLog()
        {
            Assert.Fail("This test needs to be implemented!");

            //var config = new CompleteJobberConfiguration();
        }

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoJobRunDirectory_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.JobRunDirectory = null;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoJobRunnerExeResolver_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.JobRunnerExeResolver = null;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoBackendUrl_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.BackendAddress = string.Empty;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}
    }
}
