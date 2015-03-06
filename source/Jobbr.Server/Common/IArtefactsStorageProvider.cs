using System.Collections.Generic;
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
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="content">
        /// The content.
        /// </param>
        void Save(string container, string fileName, Stream content);

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="container">
        /// The container.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        Stream Load(string container, string fileName);

        List<FileInfo> GetFiles(string container);
    }
}
