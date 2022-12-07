using System;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server.Builder;
using Jobbr.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Startup
{
    /// <summary>
    /// Tests that makes sure that the builder issues warnings if not all required components are registered
    /// </summary>
    [TestClass]
    public class SetupValidationTests
    {
        [TestMethod]
        public void ConsoleCapturer_WhenActive_ContainsConsoleOut()
        {
            using (var capture = new ConsoleCapturer())
            {
                Console.WriteLine("Hello World");

                var allLogEntries = capture.GetLines().ToList();

                Assert.IsTrue(allLogEntries.Any());
                Assert.AreEqual("Hello World", allLogEntries[0]);
                Assert.AreEqual(2, allLogEntries.Count);
            }
        }

        [TestMethod]
        public void CreateJobber_WithConsoleCapturer_CaptureMessagesFromConsole()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder(new NullLoggerFactory());

                builder.Create();

                var allLogEntries = capture.GetLines();
                Assert.IsTrue(allLogEntries.Any());
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoStorageProvider_IssuesError()
        {
            using (var capture = new ConsoleCapturer())
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder.AddConsole();
                });

                var builder = new JobbrBuilder(loggerFactory);

                // Register only Artefacts and Executor
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                builder.Create();

                var storageWarnLogs = capture.GetLines("ERROR", "JobStorageProvider").ToList();

                Assert.IsTrue(storageWarnLogs.Any());

                Assert.AreEqual(1, storageWarnLogs.Count);

                loggerFactory.Dispose();
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoArtefactsProvider_IssuesWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder.AddConsole();
                });

                var builder = new JobbrBuilder(loggerFactory);

                // Register only Executor and JobStorage
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

                builder.Create();

                var artefactsWarnings = capture.GetLines("warn", "Artefacts").ToList();

                Assert.IsTrue(artefactsWarnings.Any());
                Assert.AreEqual(2, artefactsWarnings.Count);

                loggerFactory.Dispose();
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoExecutor_IssuesError()
        {
            using (var capture = new ConsoleCapturer())
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder.AddConsole();
                });

                var builder = new JobbrBuilder(loggerFactory);

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

                builder.Create();

                var artefactsWarnings = capture.GetLines("error", "Executor").ToList();

                Assert.IsTrue(artefactsWarnings.Any());
                Assert.AreEqual(1, artefactsWarnings.Count);

                loggerFactory.Dispose();
            }
        }

        [TestMethod]
        public void CreateJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder.AddConsole();
                });

                var builder = new JobbrBuilder(loggerFactory);

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                builder.Create();

                var errors = capture.GetLines("ERROR").ToList();
                var warnings = capture.GetLines("WARN").ToList();

                Assert.IsFalse(errors.Any());
                Assert.IsFalse(warnings.Any());

                loggerFactory.Dispose();
            }
        }

        [TestMethod]
        public void StartJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var loggerFactory = LoggerFactory.Create(builder =>
                {
                    _ = builder.AddConsole();
                });

                var builder = new JobbrBuilder(loggerFactory);

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                var server = builder.Create();

                server.Start(20000);
                
                var errors = capture.GetLines("ERROR").ToList();
                var warnings = capture.GetLines("WARN").ToList();
                var fatals = capture.GetLines("FATAL").ToList();

                Assert.IsFalse(fatals.Any(), "Got too many fatals: \n\n * " + string.Join("\n * ", fatals));
                Assert.IsFalse(errors.Any(), "Got too many errors: \n\n * " + string.Join("\n * ", errors));
                Assert.IsFalse(warnings.Any(), "Got too many warnings: \n\n * " + string.Join("\n * ", warnings));

                loggerFactory.Dispose();
            }
        }
    }
}