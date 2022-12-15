using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
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
    public class TriggerServiceTests
    {
        private readonly ITriggerService _service;
        private readonly Mock<IJobbrRepository> _repositoryMock;
        private readonly Mock<ITinyMessengerHub> _messengerHubMock;

        public TriggerServiceTests()
        {
            _repositoryMock = new Mock<IJobbrRepository>();
            _messengerHubMock = new Mock<ITinyMessengerHub>();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<ModelToStorageMappingProfile>());
            var mapper = config.CreateMapper();

            _service = new TriggerService(NullLoggerFactory.Instance, _repositoryMock.Object, _messengerHubMock.Object, mapper);
        }

        [TestMethod]
        public void Add_RecurringTrigger_ShouldSetModelKeys()
        {
            long x = 1, y = 2;

            var z = x + y;

            // Arrange
            var triggerModel = new RecurringTriggerModel { Id = 1, JobId = 2 };
            _repositoryMock
                .Setup(rep => rep.SaveAddTrigger(It.IsAny<long>(), It.IsAny<RecurringTrigger>()))
                .Callback<long, RecurringTrigger>((id, trigger) =>
                {
                    trigger.Id = 3;
                    trigger.JobId = 4;
                });

            // Act
            _service.Add(1, triggerModel);

            // Assert
            triggerModel.Id.ShouldBe(3);
            triggerModel.JobId.ShouldBe(4);
        }

        [TestMethod]
        public void Add_RecurringTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var model = new RecurringTriggerModel { Id = 1, JobId = 2 };

            // Act
            _service.Add(1L, model);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerAddedMessage>(message => message.TriggerId == 1 && message.JobId == 2)), Times.Once());
        }

        [TestMethod]
        public void Add_ScheduledTrigger_ShouldSetModelKeys()
        {
            // Arrange
            var triggerModel = new ScheduledTriggerModel { Id = 1, JobId = 2 };
            _repositoryMock
                .Setup(rep => rep.SaveAddTrigger(It.IsAny<long>(), It.IsAny<ScheduledTrigger>()))
                .Callback<long, ScheduledTrigger>((id, trigger) =>
                {
                    trigger.Id = 3;
                    trigger.JobId = 4;
                });

            // Act
            _service.Add(1, triggerModel);

            // Assert
            triggerModel.Id.ShouldBe(3);
            triggerModel.JobId.ShouldBe(4);
        }

        [TestMethod]
        public void Add_ScheduledTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var model = new ScheduledTriggerModel { Id = 1, JobId = 2 };

            // Act
            _service.Add(1L, model);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerAddedMessage>(message => message.TriggerId == 1 && message.JobId == 2)), Times.Once());
        }

        [TestMethod]
        public void Add_InstantTrigger_ShouldSetModelKeys()
        {
            // Arrange
            var triggerModel = new InstantTriggerModel { Id = 1, JobId = 2 };
            _repositoryMock
                .Setup(rep => rep.SaveAddTrigger(It.IsAny<long>(), It.IsAny<InstantTrigger>()))
                .Callback<long, InstantTrigger>((id, trigger) =>
                {
                    trigger.Id = 3;
                    trigger.JobId = 4;
                });

            // Act
            _service.Add(1, triggerModel);

            // Assert
            triggerModel.Id.ShouldBe(3);
            triggerModel.JobId.ShouldBe(4);
        }

        [TestMethod]
        public void Add_InstantTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var model = new InstantTriggerModel { Id = 1, JobId = 2 };

            // Act
            _service.Add(1L, model);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerAddedMessage>(message => message.TriggerId == 1 && message.JobId == 2)), Times.Once());
        }

        [TestMethod]
        public void Disable_AnyTrigger_ShouldCallRepository()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Disable(jobId, triggerId);

            // Assert
            _repositoryMock.Verify(rep => rep.DisableTrigger(It.Is<long>(x => x == jobId), It.Is<long>(x => x == triggerId)), Times.Once());
        }


        [TestMethod]
        public void Disable_AnyTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Disable(jobId, triggerId);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerStateChangedMessage>(message => message.TriggerId == triggerId && message.JobId == jobId)), Times.Once());
        }

        [TestMethod]
        public void Enable_AnyTrigger_ShouldCallRepository()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Enable(jobId, triggerId);

            // Assert
            _repositoryMock.Verify(rep => rep.EnableTrigger(It.Is<long>(x => x == jobId), It.Is<long>(x => x == triggerId)), Times.Once());
        }

        [TestMethod]
        public void Enable_AnyTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Enable(jobId, triggerId);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerStateChangedMessage>(message => message.TriggerId == triggerId && message.JobId == jobId)), Times.Once());
        }

        [TestMethod]
        public void Delete_AnyTrigger_ShouldCallRepository()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Delete(jobId, triggerId);

            // Assert
            _repositoryMock.Verify(rep => rep.DeleteTrigger(It.Is<long>(x => x == jobId), It.Is<long>(x => x == triggerId)), Times.Once());
        }

        [TestMethod]
        public void Delete_AnyTrigger_ShouldCallMessengerHubPublish()
        {
            // Arrange
            var jobId = 1L;
            var triggerId = 2L;

            // Act
            _service.Delete(jobId, triggerId);

            // Assert
            _messengerHubMock.Verify(hub => hub.PublishAsync(It.Is<TriggerStateChangedMessage>(message => message.TriggerId == triggerId && message.JobId == jobId)), Times.Once());
        }
    }
}
