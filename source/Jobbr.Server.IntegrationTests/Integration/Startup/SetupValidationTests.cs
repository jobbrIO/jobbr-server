using System;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.Server.Builder;
using Jobbr.Server.IntegrationTests.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Jobbr.Server.IntegrationTests.Integration.Startup
{
    /// <summary>
    /// Tests that makes sure that the builder issues warnings if not all required components are registered.
    /// </summary>
    [TestClass]
    public class SetupValidationTests
    {
        private Mock<ILoggerFactory> _loggerFactoryMock;
        private Mock<ILogger<JobbrBuilder>> _loggerMock;

        [TestInitialize]
        public void Initialize()
        {
            _loggerMock = new Mock<ILogger<JobbrBuilder>>();
            _loggerMock.Setup(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<object>(), It.IsAny<Exception>(), It.IsAny<Func<object, Exception, string>>()));

            _loggerFactoryMock = new Mock<ILoggerFactory>();
            _loggerFactoryMock.Setup(m => m.CreateLogger(It.IsAny<string>())).Returns(() => _loggerMock.Object);
        }

        [TestMethod]
        public void CreateJobber_LogsProcess()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);

            // Act
            builder.Create();

            // Assert
            _loggerMock.Verify(m => m.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }

        [TestMethod]
        public void CreateJobbr_WithNoStorageProvider_IssuesError()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);
            builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
            builder.Register<IJobExecutor>(typeof(PseudoExecutor));

            // Act
            builder.Create();

            // Assert
            _loggerMock.Verify(m => m.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("There was no JobStorageProvider registered. Will continue building with an in-memory version, which does not support production scenarios.")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void CreateJobbr_WithNoArtefactsProvider_IssuesWarn()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);
            builder.Register<IJobExecutor>(typeof(PseudoExecutor));
            builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

            // Act
            builder.Create();

            // Assert
            _loggerMock.Verify(m => m.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("There was no ArtefactsStorageProvider registered. Adding a default InMemoryArtefactStorage, which stores artefacts in memory. Please register a proper ArtefactStorage for production use.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void CreateJobbr_WithNoExecutor_IssuesError()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);
            builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
            builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

            // Act
            builder.Create();

            // Assert
            _loggerMock.Verify(m => m.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("There was no JobExecutor registered. Adding a Non-Operational JobExecutor")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [TestMethod]
        public void CreateJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);
            builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
            builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
            builder.Register<IJobExecutor>(typeof(PseudoExecutor));

            // Act
            builder.Create();

            // Assert
            _loggerMock.Verify(m => m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
            _loggerMock.Verify(m => m.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }

        [TestMethod]
        public void StartJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            // Arrange
            var builder = new JobbrBuilder(_loggerFactoryMock.Object);
            builder.Register<IArtefactsStorageProvider>(typeof(PseudoArtefactsStorageProvider));
            builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
            builder.Register<IJobExecutor>(typeof(PseudoExecutor));

            // Act
            var server = builder.Create();

            server.Start(20000);

            // Assert
            _loggerMock.Verify(m => m.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
            _loggerMock.Verify(m => m.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }
    }
}