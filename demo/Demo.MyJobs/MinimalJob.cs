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
            File.WriteAllText("firstfile.txt", "This is the first content");

            File.WriteAllText("secondfile.txt", "This is the second content");
        }
    }
}
