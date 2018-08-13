using System;
using System.Threading;

namespace Jobbr.Sample.Jobs
{
    public class InTimeClass
    {
        public void Run()
        {
            Console.WriteLine("##jobbr[progress percent='1.00']");
            Thread.Sleep(1000);
            Console.WriteLine("##jobbr[progress percent='99.00']");
            Console.WriteLine("##jobbr[progress percent='100.00']");
        }
    }
}