using System;
using System.Threading;

namespace Jobbr.Sample.Jobs
{
    public class TimeoutClass
    {
        public void Run()
        {
            Console.WriteLine("##jobbr[progress percent='0.10']");
            Thread.Sleep(TimeSpan.FromMinutes(2));
            Console.WriteLine("##jobbr[progress percent='100.00']");
        }
    }
}