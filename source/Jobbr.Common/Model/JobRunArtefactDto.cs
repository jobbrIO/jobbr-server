namespace Jobbr.Common.Model
{
    /// <summary>
    /// The job run artefact dto.
    /// </summary>
    public class JobRunArtefactDto
    {
        public string Filename { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; }
    }
}