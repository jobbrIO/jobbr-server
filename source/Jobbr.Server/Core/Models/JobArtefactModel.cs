namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Model class for job artifacts.
    /// </summary>
    public class JobArtefactModel
    {
        /// <summary>
        /// Artifact file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// MIME type. MIME is a two-part identifier for file formats.
        /// </summary>
        public string MimeType { get; set; }

        /// <summary>
        /// Artifact size.
        /// </summary>
        public long Size { get; set; }
    }
}
