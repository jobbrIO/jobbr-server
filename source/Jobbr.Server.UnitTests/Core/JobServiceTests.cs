using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Shouldly;

namespace Jobbr.Server.UnitTests.Core
{
    [TestClass]
    public class JobServiceTests
    {
        private readonly IJobService service;
        private readonly Mock<IJobbrRepository> repositoryMock;

        public JobServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ModelToStorageMappingProfile>());

            var mapper = config.CreateMapper();

            repositoryMock = new Mock<IJobbrRepository>();

            service = new JobService(repositoryMock.Object, mapper);
        }

        [TestMethod]
        public void Add_ShouldAddOrUpdateModelId()
        {
            // Arrange
            var model = new JobModel { Title = "Test" };
            var jobId = 155L;
            repositoryMock.Setup(rep => rep.AddJob(It.IsAny<Job>())).Callback<Job>(job => job.Id = jobId);

            // Act
            var result = service.Add(model);

            // Assert
            result.ShouldNotBeNull();
            result.Title.ShouldBe("Test");
            result.Id.ShouldBe(jobId);
        }
    }
}
