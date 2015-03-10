using System;

namespace Demo.MyJobs
{
    /// <summary>
    /// The minimal job.
    /// </summary>
    public class ParameterizedlJob
    {
        /// <summary>
        /// The run.
        /// </summary>
        public void Run(object jobParameters, RunParameter runParameters)
        {
            Console.WriteLine("Got the params {0} and {1}", jobParameters, runParameters);
        }
    }

    public class RunParameter
    {
        public string Param1 { get; set; }

        public int Param2 { get; set; }
    }
}
