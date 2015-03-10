using System;
using System.Threading;

namespace Demo.MyJobs
{
    public class ProgressJob
    {
        public void Run()
        {
            var iterations = 15;

            for (var i = 0; i < iterations; i++)
            {
                Thread.Sleep(500);

                Console.WriteLine("##jobbr[progress percent='{0:0.00}']", (double)(i + 1) / iterations * 100);
            }
        }
    }
}
