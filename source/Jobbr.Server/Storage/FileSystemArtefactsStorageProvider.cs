using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// The file system artefacts storage provider.
    /// </summary>
    public class FileSystemArtefactsStorageProvider : IArtefactsStorageProvider
    {
        private readonly string dataDirectory;

        public FileSystemArtefactsStorageProvider(string dataDirectory)
        {
            this.dataDirectory = dataDirectory;
        }

        public override string ToString()
        {
            return string.Format("[{0}, DataDirectory: '{1}']", this.GetType().Name, Path.GetFullPath(this.dataDirectory));
        }

        public void Save(string container, string fileName, Stream content)
        {
            var dir = Directory.CreateDirectory(Path.Combine(this.dataDirectory, container));
            var fileFullPath = Path.Combine(dir.FullName, fileName);

            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
            }

            using (var fileStream = File.Create(fileFullPath))
            {
                content.CopyTo(fileStream);
            }
        }

        public Stream Load(string container, string fileName)
        {
            var dir = Directory.CreateDirectory(Path.Combine(this.dataDirectory, container));
            var fileFullPath = Path.Combine(dir.FullName, fileName);

            if (File.Exists(fileFullPath))
            {
                return File.OpenRead(fileFullPath);
            }

            return null;
        }

        public List<FileInfo> GetFiles(string container)
        {
            var dir = Directory.CreateDirectory(Path.Combine(this.dataDirectory, container));

            return dir.GetFiles().ToList();
        }
    }
}