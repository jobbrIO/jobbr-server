using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Builder;
using Jobbr.Server.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobRunStates = Jobbr.ComponentModel.JobStorage.Model.JobRunStates;

namespace Jobbr.Tests.Integration.Startup
{
    public class PseudoJobStorageProvider : IJobStorageProvider
    {
        public List<Job> GetJobs()
        {
            return null;
        }

        public long AddJob(Job job)
        {
            return 0;
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return null;
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            return 0;
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            return 0;
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            return 0;
        }

        public bool DisableTrigger(long triggerId)
        {
            return false;
        }

        public bool EnableTrigger(long triggerId)
        {
            return false;
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return null;
        }

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            return null;
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId)
        {
            return null;
        }

        public JobRun GetFutureJobRunsByTriggerId(long triggerId)
        {
            return null;
        }

        public int AddJobRun(JobRun jobRun)
        {
            return 0;
        }

        public List<JobRun> GetJobRuns()
        {
            return null;
        }

        public bool UpdateProgress(long jobRunId, double? progress)
        {
            return false;
        }

        public bool Update(JobRun jobRun)
        {
            return false;
        }

        public Job GetJobById(long id)
        {
            return null;
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return null;
        }

        public JobRun GetJobRunById(long id)
        {
            return null;
        }

        public List<JobRun> GetJobRunsForUserId(long userId)
        {
            return null;
        }

        public List<JobRun> GetJobRunsForUserName(string userName)
        {
            return null;
        }

        public bool Update(Job job)
        {
            return false;
        }

        public bool Update(InstantTrigger trigger)
        {
            return false;
        }

        public bool Update(ScheduledTrigger trigger)
        {
            return false;
        }

        public bool Update(RecurringTrigger trigger)
        {
            return false;
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            return null;
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            return null;
        }

        public bool CheckParallelExecution(long triggerId)
        {
            return false;
        }
    }

    public class PseudoArfetacstStorageProvider : IArtefactsStorageProvider
    {
        public void Save(string container, string fileName, Stream content)
        {
        }

        public Stream Load(string container, string fileName)
        {
            return null;
        }

        public List<JobbrArtefact> GetArtefacts(string container)
        {
            return null;
        }
    }

    public class PseudoExecutor : IJobExecutor
    {
        public void Dispose()
        {
        }

        public void Start()
        {
        }

        public void OnPlanChanged(List<PlannedJobRun> newPlan)
        {
        }

        public bool OnJobRunCanceled(Guid uniqueId)
        {
            return false;
        }

        public void Stop()
        {
        }
    }

    [TestClass]
    public class ConfigurationTests
    {
        private static object syncRoot = new object();
        private CaptureInMemoryLogProvider captureInMemoryLogProvider;

        public ConfigurationTests()
        {
            LogProvider.LogProviderResolvers.Clear();

            this.captureInMemoryLogProvider = new CaptureInMemoryLogProvider();
            LogProvider.LogProviderResolvers.Clear();
            LogProvider.SetCurrentLogProvider(captureInMemoryLogProvider);
        }

        [TestCleanup]
        public void CleanUp()
        {
            // Set the default logger back
            LogProvider.SetCurrentLogProvider(null);
        }

        [TestMethod]
        public void NewJobber_OwnLogger_GetsMessages1()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} NewJobber_OwnLogger_GetsMessages started");
            var builder = new JobbrBuilder();

            //Thread.Sleep(new Random().Next(1, 2500));
            builder.Create();

            //Thread.Sleep(new Random().Next(1, 2500));

            var allLogEntries = captureInMemoryLogProvider.GetLogsFromThread();
            Assert.IsTrue(allLogEntries.Any());
            Console.WriteLine("NewJobber_OwnLogger_GetsMessages ended");
        }

        [TestMethod]
        public void NewJobber_OwnLogger_GetsMessages2()
        {
            Console.WriteLine($"{Thread.CurrentThread.ManagedThreadId} NewJobber_OwnLogger_GetsMessages started");
            var builder = new JobbrBuilder();

            //Thread.Sleep(new Random().Next(1, 2500));
            builder.Create();

            //Thread.Sleep(new Random().Next(1, 2500));
            var allLogEntries = captureInMemoryLogProvider.GetLogsFromThread();
            Assert.IsTrue(allLogEntries.Any());
            Console.WriteLine("NewJobber_OwnLogger_GetsMessages ended");
        }

        //[TestMethod]
        //public void NewJobbr_WithNoStorageProvider_IssuesErrorInLog()
        //{
        //    Console.WriteLine("NewJobbr_WithNoStorageProvider_IssuesErrorInLog started");

        //    // Assert.Fail("This test needs to be re-implemented!");

        //    var builder = new JobbrBuilder();

        //    // Register only Artefacts and Executor
        //    builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
        //    builder.Register<IJobExecutor>(typeof(PseudoExecutor));

        //    builder.Create();

        //    //WaitFor.HasElements(() => captureInMemoryLogProvider.GetLogs(), 1000);

        //    var allLogs = captureInMemoryLogProvider.GetLogsFromThread();
        //    var storageWarnLogs = allLogs.Where(l => l.LogLevel == LogLevel.Error && l.Message.Contains("JobStorageProvider")).ToList();

        //    Assert.IsTrue(allLogs.Any());

        //    Assert.AreEqual(1, storageWarnLogs.Count);
        //    //var config = new CompleteJobberConfiguration();
        //    //config.JobStorageProvider = null;

        //    //var jobbr = new JobbrServer(config);

        //    //jobbr.Start(1000);
        //    Console.WriteLine("NewJobbr_WithNoStorageProvider_IssuesErrorInLog ended");

        //}

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoArtefactsProvider_IssuesWarnInLog()
        {
            Assert.Fail("This test needs to be re-implemented!");

            //var config = new CompleteJobberConfiguration();
            //config.ArtefactStorageProvider = null;

            //var jobbr = new JobbrServer(config);

            //jobbr.Start(1000);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NewJobbr_WithNoExecutor_IssuesWarnInLog()
        {
            Assert.Fail("This test needs to be implemented!");

            //var config = new CompleteJobberConfiguration();
        }

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoJobRunDirectory_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.JobRunDirectory = null;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoJobRunnerExeResolver_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.JobRunnerExeResolver = null;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}

        ////[TestMethod]
        ////[ExpectedException(typeof(ArgumentException))]
        ////public void NewJobbr_WithNoBackendUrl_FailsOnStart()
        ////{
        ////    Assert.Fail("This test needs to be re-implemented!");

        ////    //var config = new CompleteJobberConfiguration();
        ////    //config.BackendAddress = string.Empty;

        ////    //var jobbr = new JobbrServer(config);

        ////    //jobbr.Start(1000);
        ////}
    }

    public class CaptureInMemoryLogProvider : ILogProvider
    {
        private readonly ThreadLocal<MemoryLogger> memLoggersForThreads = new ThreadLocal<MemoryLogger>();
        private MemoryLogger logger;

        public CaptureInMemoryLogProvider()
        {
            logger = new MemoryLogger();
            this.memLoggersForThreads.Value = new MemoryLogger();
        }

        public ILog GetLogger(string name)
        {
            //return this.memLoggersForThreads.Value;
            return logger;
   ;     }

        public List<MemoryLogger.MemoryLoggerEntry> GetLogsFromThread()
        {
            //return this.memLoggersForThreads.Value.Store.ToList();
            return logger.Store;
        }

        public IDisposable OpenNestedContext(string message)
        {
            return null;
        }

        public IDisposable OpenMappedContext(string key, string value)
        {
            return null;
        }
    }

    public class MemoryLogger : ILog
    {
        public class MemoryLoggerEntry
        {
            public DateTime DateTimeUtc { get; set; }

            public LogLevel LogLevel { get; set; }

            public string Message { get; set; }
        }

        List<MemoryLoggerEntry> store = new List<MemoryLoggerEntry>();

        public List<MemoryLoggerEntry> Store
        {
            get { return this.store; }
        }

        public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null,
            params object[] formatParameters)
        {
            if (messageFunc == null)
            {
                return true;
            }

            var dateTimeUtc = DateTime.Today;

            var text = messageFunc();

            if (formatParameters != null)
            {
                text = string.Format(text, formatParameters);
            }

            this.store.Add(new MemoryLoggerEntry() {DateTimeUtc = dateTimeUtc, LogLevel = logLevel, Message = text});

            Console.WriteLine($"{dateTimeUtc:G} [{logLevel}] {text}");

            return true;
        }
    }
}