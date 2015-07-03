using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.ClientRuntime
{
    [TestClass]
    public class JobbrRuntimeTests
    {
        [TestMethod]
        public void StartJob_WithParameters()
        {

        }

        [TestMethod]
        public void StartJob_With_OnlyInstanceParameters()
        {
        }
        
        [TestMethod]
        public void StartJob_With_OnlyJobParameter()
        {
        }
    }

    public class DummyJob
    {
        public void Run()
        {
        }

        public void Run(int one, int two)
        {

        }

        public void Run(object jobParameters, CustomRunParameter customRunParameters)
        {
            Console.WriteLine("Got the params {0} and {1}", jobParameters, customRunParameters);
        }
    }


    public class CustomRunParameter
    {
        public string Param1 { get; set; }

        public int Param2 { get; set; }
    }
}