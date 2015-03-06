using System.IO;

namespace Jobbr.Server.Common
{
    /// <summary>
    /// The ArtefactsStorageProvider interface.
    /// </summary>
    public interface IArtefactsStorageProvider
    {
        /// <summary>
        /// The save.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        void Save(string key, string fileName, Stream content);

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream Load(string key, string fileName);
    
    }
}
