using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Execution
{
    [TestClass]
    public class GracefulStopTests : JobRunExecutionTestBase
    {
        ////private JobRun currentRun;

        [TestInitialize]
        public void Setup()
        {
            ////var job = this.CreateTestJob();
            ////job.Type = typeof(MyFancyTask).ToString();
            ////var trigger = CreateInstantTrigger(job);

            ////this.currentRun = this.TriggerNewJobRun(trigger);
        }
    }

    public class MyFancyTask
    {
        public int TimesIWasCalled { get; private set; }

        public bool TaskWasFinished { get; private set; }

        public void Run()
        {
            this.TaskWasFinished = false;
            this.TimesIWasCalled++;

            new Task(() =>
            {
                Console.WriteLine("Task started");
                Thread.Sleep(2000);
                Console.WriteLine("Task finished");
            }).RunSynchronously();

            this.TaskWasFinished = true;
        }
    }
}