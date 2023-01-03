using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;

namespace Jobbr.Server.Storage
{
    public class InMemoryArtefactsStorage : IArtefactsStorageProvider
    {
        private readonly IDictionary<string, IList<InMemoryFile>> files = new Dictionary<string, IList<InMemoryFile>>();

        public void Save(string container, string fileName, Stream content)
        {
            if (files.ContainsKey(container) == false)
            {
                files.Add(container, new List<InMemoryFile>());
            }

            var list = files[container];

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

        public List<JobbrArtefact> GetArtefacts(string container)
        {
            var filesInContainer = GetFilesFromContainer(container);

            return filesInContainer.Select(s => new JobbrArtefact { FileName = s.Name }).ToList();
        }

        private IEnumerable<InMemoryFile> GetFilesFromContainer(string container)
        {
            if (files.ContainsKey(container) == false)
            {
                throw new ArgumentException("Container not found");
            }

            return files[container];
        }

        private class InMemoryFile
        {
            public string Name { get; set; }

            public byte[] Data { get; set; }
        }
    }
}
