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

            Directory.CreateDirectory(this.dataDirectory);
        }

        public void Save(string key, string fileName, Stream content)
        {
            
        }

        public Stream Load(string key, string fileName)
        {
            return null;
        }
    }
}