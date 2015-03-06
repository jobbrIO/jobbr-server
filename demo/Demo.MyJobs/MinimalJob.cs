using System.IO;

namespace Demo.MyJobs
{
    /// <summary>
    /// The minimal job.
    /// </summary>
    public class MinimalJob
    {
        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            File.WriteAllText("content.txt", "This is the content");
        }
    }
}
