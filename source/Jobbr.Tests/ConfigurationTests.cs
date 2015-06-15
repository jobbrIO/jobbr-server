using System;

using Jobbr.Server;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NewJobbr_WithoutConfiguration_FailsOnCtor()
        {
            var jobbr = new JobbrServer(null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoStorageProvider_FailsOnStart()
        {
            var config = new CompleteJobberConfiguration();
            config.JobStorageProvider = null;

            var jobbr = new JobbrServer(config);

            jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoArtefactsProvider_FailsOnStart()
        {
            var config = new CompleteJobberConfiguration();
            config.ArtefactStorageProvider = null;

            var jobbr = new JobbrServer(config);

            jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoJobRunDirectory_FailsOnStart()
        {
            var config = new CompleteJobberConfiguration();
            config.JobRunDirectory = null;

            var jobbr = new JobbrServer(config);

            jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoJobRunnerExeResolver_FailsOnStart()
        {
            var config = new CompleteJobberConfiguration();
            config.JobRunnerExeResolver = null;

            var jobbr = new JobbrServer(config);

            jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoBackendUrl_FailsOnStart()
        {
            var config = new CompleteJobberConfiguration();
            config.BackendAddress = string.Empty;

            var jobbr = new JobbrServer(config);

            jobbr.Start(1000);
        }
    }
}
