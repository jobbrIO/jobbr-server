using AutoMapper;
using Jobbr.Server.Core;
using Jobbr.Server.Core.Models;
using Jobbr.Tests.Infrastructure.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.UnitTests.Core
{
    [TestClass]
    public class JobServiceTests
    {
        private readonly IJobService service;

        public JobServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<ModelToStorageMappingProfile>());
            var mapper = config.CreateMapper();

            service = new JobService(new JobbrRepositoryMock(), mapper);
        }

        [TestMethod]
        public void Add_ShouldReturnModelWithId()
        {
            // Arrange
            var model = new JobModel() { Title = "Test" };

            // Act
            var result = service.Add(model);

            // AssertGVt
            Assert.IsNotNull(result);
            Assert.AreEqual("Test", result.Title);
            Assert.AreEqual(JobbrRepositoryMock.JobId, result.Id);
        }
    }
}
