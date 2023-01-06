using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// In-memory artifact storage. Intended for backup use.
    /// </summary>
    public class InMemoryArtefactsStorage : IArtefactsStorageProvider
    {
        private readonly IDictionary<string, IList<InMemoryFile>> _files = new Dictionary<string, IList<InMemoryFile>>();

        /// <summary>
        /// Save artifact.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="content">Artifact content to save.</param>
        public void Save(string container, string fileName, Stream content)
        {
            if (_files.ContainsKey(container) == false)
            {
                _files.Add(container, new List<InMemoryFile>());
            }

            var list = _files[container];

            var memoryStream = new MemoryStream();

            content.CopyTo(memoryStream);

            var data = memoryStream.ToArray();

            var item = new InMemoryFile
            {
                Name = fileName,
                Data = data
            };

            list.Add(item);
        }

        /// <summary>
        /// Load artifact.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>Artifact as a stream.</returns>
        /// <exception cref="FileNotFoundException">File not found.</exception>
        public Stream Load(string container, string fileName)
        {
            var filesInContainer = GetFilesFromContainer(container);

            var file = filesInContainer.FirstOrDefault(p => string.Equals(p.Name, fileName, StringComparison.OrdinalIgnoreCase));

            if (file == null)
            {
                throw new FileNotFoundException($"File {fileName} not found");
            }

            return new MemoryStream(file.Data);
        }

        /// <summary>
        /// Get all container artifacts.
        /// </summary>
        /// <param name="container">Container name.</param>
        /// <returns>List of artifacts.</returns>
        public List<JobbrArtefact> GetArtefacts(string container)
        {
            var filesInContainer = GetFilesFromContainer(container);

            return filesInContainer.Select(s => new JobbrArtefact { FileName = s.Name }).ToList();
        }

        private IEnumerable<InMemoryFile> GetFilesFromContainer(string container)
        {
            if (_files.ContainsKey(container) == false)
            {
                throw new ArgumentException("Container not found");
            }

            return _files[container];
        }

        private class InMemoryFile
        {
            public string Name { get; set; }

            public byte[] Data { get; set; }
        }
    }
}
