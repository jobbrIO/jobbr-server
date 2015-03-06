using System.IO;

namespace Jobbr.Server.Common
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

        public void Save(string key, string fileName, Stream content)
        {
            var dir = Directory.CreateDirectory(Path.Combine(this.dataDirectory, key));
        }

        public Stream Load(string key, string fileName)
        {
            var dir = Directory.CreateDirectory(Path.Combine(this.dataDirectory, key));
            return null;
        }
    }
}