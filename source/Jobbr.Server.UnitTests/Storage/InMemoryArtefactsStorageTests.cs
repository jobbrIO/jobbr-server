using System;
using System.IO;
using System.Linq;
using System.Text;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;

namespace Jobbr.Server.UnitTests.Storage
{
    [TestClass]
    public class InMemoryArtefactsStorageTests
    {
        private readonly IArtefactsStorageProvider _provider;

        public InMemoryArtefactsStorageTests()
        {
            _provider = new InMemoryArtefactsStorage();
        }

        [TestMethod]
        public void Save_EmptyStream_ShouldSaveAllByteArray()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            using var stream = new MemoryStream();

            // Act
            _provider.Save(container, fileName, stream);

            // Assert
            var artefacts = _provider.GetArtefacts(container);
            artefacts.Count.ShouldBe(1);
            artefacts.First().FileName.ShouldBe(fileName);
        }

        [TestMethod]
        public void Save_Stream_ShouldSaveAllByteArray()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            var content = "Some content placeholder";
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content));

            // Act
            _provider.Save(container, fileName, inputStream);

            // Assert
            using var outputStream = _provider.Load(container, fileName) as MemoryStream;
            var bytes = outputStream.ToArray();
            bytes.Length.ShouldBe(content.Length);

            var outputString = Encoding.UTF8.GetString(bytes);
            outputString.ShouldBe(content);
        }

        [Ignore("Save() initiates that the complete stream is saved.\n" +
            "However, the passed stream may have already been read up to a certain point and was not reset which may lead to unwanted behavior.")]
        [TestMethod]
        public void Save_StreamWithOffset_ShouldSaveAllByteArray()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            var content = "Some content placeholder";
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            inputStream.Position += 5;

            // Act
            _provider.Save(container, fileName, inputStream);

            // Assert
            using var outputStream = _provider.Load(container, fileName) as MemoryStream;
            var bytes = outputStream.ToArray();
            bytes.Length.ShouldBe(content.Length);

            var outputString = Encoding.UTF8.GetString(bytes);
            outputString.ShouldBe(content);
        }

        [TestMethod]
        public void Save_ClosedStream_ShouldSaveAllByteArray()
        {
            // Arrange
            using var inputStream = new MemoryStream();
            inputStream.Close();

            // Act & Assert
            Should.Throw<ObjectDisposedException>(() => _provider.Save("container", "filename.zip", inputStream));
        }

        [TestMethod]
        public void GetArtefacts_WithMultipleContainers_ShouldReturnListOfContainerArtefacts()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            using var stream = new MemoryStream();
            _provider.Save(container, fileName, stream);
            _provider.Save("another-container", "another-filename.zip", stream);

            // Act
            var result = _provider.GetArtefacts(container);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(1);
            result.ShouldContain(file => file.FileName == fileName);
        }

        [TestMethod]
        public void GetArtefacts_MissingContainer_ShouldThrow()
        {
            // Act & Assert
            Should.Throw<ArgumentException>(() => _provider.GetArtefacts("container"), "Container not found");
        }

        [TestMethod]
        public void Load_ExistingFile_ShouldReturnStream()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            var content = "Some content placeholder";
            using var inputStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            _provider.Save(container, fileName, inputStream);

            // Act
            using var result = _provider.Load(container, fileName);

            // Assert
            result.ShouldBeOfType<MemoryStream>();
            var outputContent = Encoding.UTF8.GetString(((MemoryStream)result).ToArray());
            outputContent.ShouldBe(content);
        }

        [TestMethod]
        public void Load_MissingFile_ShouldThrow()
        {
            // Arrange
            var container = "container";
            var fileName = "filename.zip";
            using var stream = new MemoryStream();
            _provider.Save(container, "another-filename.zip", stream);

            // Act & Assert
            Should.Throw<FileNotFoundException>(() => _provider.Load(container, fileName), $"File {fileName} not found");
        }
    }
}
