using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Jobbr.Server.UnitTests.Storage
{
    [TestClass]
    public class JobbrRepositoryTests
    {
        private readonly IJobbrRepository _repository;
        private readonly Mock<IJobStorageProvider> _storageProviderMock;

        public JobbrRepositoryTests()
        {
            _storageProviderMock = new Mock<IJobStorageProvider>();

            _repository = new JobbrRepository(NullLoggerFactory.Instance, _storageProviderMock.Object);
        }

        [TestMethod]
        public void SetPidForJobRun_ShouldSetAndPropagate()
        {
            // Arrange
            var id = 1;
            var job = new JobRun { Id = id };

            // Act
            _repository.SetPidForJobRun(job, id);

            // Assert
            job.Pid.ShouldBe(id);
            _storageProviderMock.Verify(provider => provider.Update(It.Is<JobRun>(x => x.Id == id && x.Pid == id)), Times.Once());
        }

        [TestMethod]
        public void UpdatePlannedStartDateTimeUtc_ShouldSetPlannedStartDateTimeUtc()
        {
            // Arrange
            var plannedStartDateTimeUtc = new DateTime(2022, 12, 14, 13, 59, 22);
            var jobRun = new JobRun();
            _storageProviderMock.Setup(provider => provider.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _repository.UpdatePlannedStartDateTimeUtc(1, plannedStartDateTimeUtc);

            // Assert
            jobRun.PlannedStartDateTimeUtc.ShouldBe(plannedStartDateTimeUtc);
        }

        [TestMethod]
        public void SaveUpdateTrigger_InstantTrigger_ShouldSetValuesAndPropagate()
        {
            // Arrange
            var createdDateTimeUtc = new DateTime(2022, 12, 14, 14, 17, 0, DateTimeKind.Utc);
            var trigger = new InstantTrigger { IsActive = false, CreatedDateTimeUtc = createdDateTimeUtc };
            var triggerUpdate = new InstantTrigger { IsActive = true, CreatedDateTimeUtc = createdDateTimeUtc.AddDays(1) };

            SetupStorageProviderGetTriggerById(trigger);

            // Act
            _repository.SaveUpdateTrigger(1, triggerUpdate, out bool hasChanges);

            // Assert
            hasChanges.ShouldBeTrue();
            triggerUpdate.CreatedDateTimeUtc.ShouldBe(createdDateTimeUtc);
            _storageProviderMock.Verify(provider => provider.Update(It.IsAny<long>(), It.IsAny<InstantTrigger>()), Times.Once());
        }

        [TestMethod]
        public void SaveUpdateTrigger_InstantTriggerNoChanges_ShouldNotSetValuesAndPropagate()
        {
            // Arrange
            var createdDateTimeUtc = new DateTime(2022, 12, 14, 14, 17, 0, DateTimeKind.Utc);
            var trigger = new InstantTrigger { CreatedDateTimeUtc = createdDateTimeUtc.AddHours(-1) };
            var triggerUpdate = new InstantTrigger { CreatedDateTimeUtc = createdDateTimeUtc };

            SetupStorageProviderGetTriggerById(trigger);

            // Act
            var result = _repository.SaveUpdateTrigger(1, triggerUpdate, out bool hasChanges);

            // Assert
            hasChanges.ShouldBeFalse();
            result.ShouldBe(trigger);

            _storageProviderMock.Verify(provider => provider.Update(It.IsAny<long>(), It.IsAny<InstantTrigger>()), Times.Never());

            // TODO: Check if it is intendet to reverse the CreatedDateTimeUtc update?
            triggerUpdate.CreatedDateTimeUtc.ShouldBe(createdDateTimeUtc);
        }

        [TestMethod]
        public void SaveUpdateTrigger_ScheduledTriggerOtherChanges_ShouldSetValuesAndPropagate()
        {
            // Assert
            var startDateTimeUtc = new DateTime(2022, 12, 14, 14, 17, 0, DateTimeKind.Utc);
            var startDateTimeUtcUpdate = startDateTimeUtc.AddHours(4);

            var trigger = new ScheduledTrigger { StartDateTimeUtc = startDateTimeUtc };
            var triggerUpdate = new ScheduledTrigger { StartDateTimeUtc = startDateTimeUtcUpdate };

            SetupStorageProviderGetTriggerById(trigger);

            // Act
            var result = _repository.SaveUpdateTrigger(1, triggerUpdate, out bool hasChanges);

            // Assert
            hasChanges.ShouldBeTrue();
            result.ShouldBe(trigger);

            trigger.StartDateTimeUtc.ShouldBe(startDateTimeUtcUpdate);
            triggerUpdate.StartDateTimeUtc.ShouldBe(startDateTimeUtcUpdate);

            _storageProviderMock.Verify(provider => provider.Update(It.IsAny<long>(), It.IsAny<ScheduledTrigger>()), Times.Once());
        }

        [TestMethod]
        public void SaveUpdateTrigger_ScheduledTriggerBaseChanges_ShouldSetValuesAndPropagate()
        {
            // Assert
            var comment = "Scheduled trigger base changes";
            var commentUpdate = $"{comment} (update)";

            var trigger = new ScheduledTrigger { Comment = comment };
            var triggerUpdate = new ScheduledTrigger { Comment = commentUpdate };

            SetupStorageProviderGetTriggerById(trigger);

            // Act
            var result = _repository.SaveUpdateTrigger(1, triggerUpdate, out bool hasChanges);

            // Assert
            hasChanges.ShouldBeTrue();
            result.ShouldBe(trigger);
            _storageProviderMock.Verify(provider => provider.Update(It.IsAny<long>(), It.IsAny<ScheduledTrigger>()), Times.Once());
        }

        [TestMethod]
        public void SaveUpdateTrigger_InstantTriggerBaseChanges_ShouldNotUpdateBaseValues()
        {
            // Assert
            var comment = "Scheduled trigger base changes";
            var commentUpdate = $"{comment} (update)";

            var trigger = new InstantTrigger { Comment = comment };
            var triggerUpdate = new InstantTrigger { Comment = commentUpdate };

            SetupStorageProviderGetTriggerById(trigger);

            // Act
            var result = _repository.SaveUpdateTrigger(1, triggerUpdate, out bool hasChanges);

            // Assert
            hasChanges.ShouldBeFalse();
            result.ShouldBe(trigger);
            trigger.Comment.ShouldBe(comment);
            _storageProviderMock.Verify(provider => provider.Update(It.IsAny<long>(), It.IsAny<ScheduledTrigger>()), Times.Never());
        }

        [TestMethod]
        public void SaveNewJobRun_ShouldReturnNewInstance()
        {
            // Arrange
            var job = new Job { Parameters = "{ key: 1, value: \"job-1\" }" };
            var trigger = new JobTriggerBase();
            var plannedStartDateTimeUtc = new DateTime(2022, 12, 14, 14, 59, 0, DateTimeKind.Utc);

            // Act
            var result = _repository.SaveNewJobRun(job, trigger, plannedStartDateTimeUtc);

            // Assert
            result.ShouldNotBeNull();
            result.JobParameters.ShouldBe(job.Parameters);
            result.PlannedStartDateTimeUtc.ShouldBe(plannedStartDateTimeUtc);

            _storageProviderMock.Verify(provider => provider.AddJobRun(It.Is<JobRun>(x => x == result)), Times.Once());
        }

        [TestMethod]
        public void SaveNewJobRun_MissingReference_ShouldThrow()
        {
            // Arrange
            var job = new Job();
            var trigger = new JobTriggerBase();
            var plannedStartDateTimeUtc = new DateTime(2022, 12, 14, 15, 3, 0, DateTimeKind.Utc);

            // Act & Assert
            Should.Throw<NullReferenceException>(() => _repository.SaveNewJobRun(null, trigger, plannedStartDateTimeUtc));
            Should.Throw<NullReferenceException>(() => _repository.SaveNewJobRun(job, null, plannedStartDateTimeUtc));
        }

        [TestMethod]
        public void Delete_ExistingJobRun_ShouldSetDeleteState()
        {
            // Arrange
            var jobRun = new JobRun { State = JobRunStates.Processing };
            var jobRunDelete = new JobRun { State = JobRunStates.Processing };

            _storageProviderMock.Setup(provider => provider.GetJobRunById(It.IsAny<long>())).Returns(jobRun);

            // Act
            _repository.Delete(jobRun);

            // Assert
            jobRun.State.ShouldBe(JobRunStates.Deleted);
            jobRunDelete.State.ShouldBe(JobRunStates.Processing);
            _storageProviderMock.Verify(provider => provider.Update(It.Is<JobRun>(x => x == jobRun)), Times.Once());
        }

        [TestMethod]
        public void GetJobRunsByStateRange_InvalidMinMaxStateParams_ShouldThrow()
        {
            // Arrange
            JobRunStates minState = JobRunStates.Completed, maxState = JobRunStates.Starting;

            // Act && Assert
            Should.Throw<ArgumentOutOfRangeException>(() => _repository.GetJobRunsByStateRange(minState, maxState).ToList());
        }

        private void SetupStorageProviderGetTriggerById<T>(T trigger)
            where T : JobTriggerBase
        {
            _storageProviderMock.Setup(provider => provider.GetTriggerById(It.IsAny<long>(), It.IsAny<long>())).Returns(trigger);
        }
    }
}
