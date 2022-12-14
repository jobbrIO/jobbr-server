using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;
using TinyMessenger;

namespace Jobbr.Server.UnitTests.Core
{
    [TestClass]
    public class JobRunServiceTests
    {
        private readonly IJobRunService _service;
        private readonly Mock<IJobbrRepository> _repositoryMock;
        private readonly Mock<ITinyMessengerHub> _messengerHubMock;
        private readonly Mock<IArtefactsStorageProvider> _storageProviderMock;

        public JobRunServiceTests()
        {
            var profiles = new List<Profile> { new ModelToStorageMappingProfile(), new StorageToModelMappingProfile() };
            var config = new MapperConfiguration(cfg => cfg.AddProfiles(profiles));
            var mapper = config.CreateMapper();

            _repositoryMock = new Mock<IJobbrRepository>();
            _messengerHubMock = new Mock<ITinyMessengerHub>();
            _storageProviderMock = new Mock<IArtefactsStorageProvider>();

            _service = new JobRunService(NullLoggerFactory.Instance, _messengerHubMock.Object, _repositoryMock.Object, _storageProviderMock.Object, mapper);
        }

        [TestMethod]
        public void UpdateProgess_ShouldCallRepository()
        {
            // Arrange
            var jobRunId = 1L;
            var progress = 2D;

            // Act
            _service.UpdateProgress(jobRunId, progress);

            // Assert
            _repositoryMock.Verify(rep => rep.UpdateJobRunProgress(It.Is<long>(x => x == jobRunId), It.Is<double>(x => x == progress)), Times.Once());
        }

        [TestMethod]
        [DataRow(JobRunStates.Started)]
        public void UpdateState_ShouldSetActualStartDateTimeUtc(JobRunStates state)
        {
            // Arrange
            var jobRun = new ComponentModel.JobStorage.Model.JobRun();
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _service.UpdateState(1, state);

            // Assert
            jobRun.ActualStartDateTimeUtc.ShouldNotBeNull();
        }

        [TestMethod]
        [DataRow(JobRunStates.Null)]
        [DataRow(JobRunStates.Scheduled)]
        [DataRow(JobRunStates.Collecting)]
        [DataRow(JobRunStates.Completed)]
        public void UpdateState_ShouldNotChangeActualStartDateTimeUtc(JobRunStates state)
        {
            // Arrange
            var actualStartDateTimeUtc = new DateTime(2022, 12, 14, 1, 0, 15);
            var jobRun = new ComponentModel.JobStorage.Model.JobRun { ActualStartDateTimeUtc = actualStartDateTimeUtc };
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _service.UpdateState(1, state);

            // Assert
            jobRun.ActualStartDateTimeUtc.ShouldBe(actualStartDateTimeUtc);
        }

        [TestMethod]
        [DataRow(JobRunStates.Completed, true)]
        [DataRow(JobRunStates.Failed, false)]
        public void UpdateState_ShouldCallMessengerHubPublish(JobRunStates state, bool isSuccessful)
        {
            // Arrange
            var jobRunId = 1;
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(new ComponentModel.JobStorage.Model.JobRun());

            // Act
            _service.UpdateState(jobRunId, state);

            // Assert
            _messengerHubMock.Verify(hub => hub.Publish(It.Is<JobRunCompletedMessage>(message => message.Id == jobRunId && message.IsSuccessful == isSuccessful)), Times.Once());
        }

        [TestMethod]
        [DataRow(JobRunStates.Completed)]
        [DataRow(JobRunStates.Failed)]
        public void UpdateState_ShoudSetActualEndDateTimeUtc(JobRunStates state)
        {
            // Arrange
            var jobRun = new ComponentModel.JobStorage.Model.JobRun();
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _service.UpdateState(1, state);

            // Assert
            jobRun.ActualEndDateTimeUtc.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetArtefactsByJobRunId_ShouldReturnList()
        {
            // Arrange
            var numberOfArtefacts = 3;
            var artefacts = Enumerable.Repeat(new ComponentModel.ArtefactStorage.Model.JobbrArtefact(), numberOfArtefacts).ToList();
            _storageProviderMock.Setup(provider => provider.GetArtefacts(It.IsAny<string>())).Returns(artefacts);

            // Act
            var result = _service.GetArtefactsByJobRunId(1);

            // Assert
            result.ShouldNotBeNull().ShouldNotBeEmpty();
            result.Count.ShouldBe(numberOfArtefacts);
        }

        [TestMethod]
        public void GetArtefactsByJobRunId_WithException_ShouldReturnList()
        {
            // Arrange
            _storageProviderMock.Setup(provider => provider.GetArtefacts(It.IsAny<string>())).Throws<Exception>();

            // Act
            var result = _service.GetArtefactsByJobRunId(1);

            // Assert
            result.ShouldNotBeNull().ShouldBeEmpty();
        }

        [TestMethod]
        public void GetArtefactAsStream_ShouldReturnStream()
        {
            // Arrange
            var stream = new MemoryStream();
            _storageProviderMock.Setup(provider => provider.Load(It.IsAny<string>(), It.IsAny<string>())).Returns(stream);

            // Act
            var result = _service.GetArtefactAsStream(1, "filename");

            // Assert
            result.ShouldNotBeNull();
        }

        [TestMethod]
        public void GetArtefactAsStream_WithException_ShouldReturnNull()
        {
            // Arrange
            _storageProviderMock.Setup(provider => provider.Load(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            // Act
            var result = _service.GetArtefactAsStream(1, "filename");

            // Assert
            result.ShouldBeNull();
        }


        [TestMethod]
        public void UpdatePid_ShouldSetToProccessId()
        {
            // Arrange
            var proccessId = 2;
            var jobRun = new ComponentModel.JobStorage.Model.JobRun();
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _service.UpdatePid(1, proccessId);

            // Assert
            jobRun.Pid.ShouldNotBeNull().ShouldBe(proccessId);
        }

        [TestMethod]
        public void Delete_ShouldSetDeleted()
        {
            // Arrange
            var jobRun = new ComponentModel.JobStorage.Model.JobRun();
            _repositoryMock.Setup(rep => rep.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _service.Delete(1);

            // Assert
            jobRun.Deleted.ShouldBeTrue();
        }
    }
}
