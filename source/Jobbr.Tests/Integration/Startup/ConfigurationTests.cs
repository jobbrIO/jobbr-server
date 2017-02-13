using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
        //static ConfigurationTests()
        //{
        //    var lp = new CaptureLogProvider();
        //    LogProvider.SetCurrentLogProvider(lp);
        //}

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            var lp = new CaptureLogProvider();
            LogProvider.SetCurrentLogProvider(lp);

            // Seems to be an issue if this is set via static constructor
            // LogProvider.SetCurrentLogProvider(new CaptureLogProvider());
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            LogProvider.SetCurrentLogProvider(null);
        }



        [TestMethod]
        public void NewJobber_OwnLogger_GetsMessages1()
        {
            using (LogProvider.OpenNestedConext(nameof(NewJobber_OwnLogger_GetsMessages1)))
            {
                var builder = new JobbrBuilder();

                builder.Create();

                var allLogEntries = CaptureLogProvider.ContextLogs;
                Assert.IsTrue(allLogEntries.Any());
            }
        }

        [TestMethod]
        public void NewJobber_OwnLogger_GetsMessages2()
        {
            using (LogProvider.OpenNestedConext(nameof(NewJobber_OwnLogger_GetsMessages2)))
            {
                var builder = new JobbrBuilder();

                builder.Create();

                var allLogEntries = CaptureLogProvider.ContextLogs;
                Assert.IsTrue(allLogEntries.Any());
            }
        }

        [TestMethod]
        public void NewJobbr_WithNoStorageProvider_IssuesErrorInLog()
        {
            using (LogProvider.OpenNestedConext(nameof(NewJobbr_WithNoStorageProvider_IssuesErrorInLog)))
            {
                var builder = new JobbrBuilder();

                // Register only Artefacts and Executor
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                builder.Create();

                var allLogs = CaptureLogProvider.ContextLogs;
                var storageWarnLogs = allLogs.Where(l => l.LogLevel == LogLevel.Error && l.Message.Contains("JobStorageProvider")).ToList();

                Assert.IsTrue(allLogs.Any());

                Assert.AreEqual(1, storageWarnLogs.Count);
            }
        }

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


    public class CaptureLogProvider : ILogProvider
    {
        public class MemoryLoggerEntry
        {
            public DateTime DateTimeUtc { get; set; }

            public LogLevel LogLevel { get; set; }

            public string Message { get; set; }
        }

        public class MemoryLogger : ILog
        {
            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
            {
                lock (this)
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

                    Console.WriteLine($"[PID{Process.GetCurrentProcess().Id}] [T{Thread.CurrentThread.ManagedThreadId}] [C '{contextName}'] Message: '{text}'");

                    var item = new MemoryLoggerEntry() { DateTimeUtc = dateTimeUtc, LogLevel = logLevel, Message = text };

                    logs.AddOrUpdate(contextName, d => new List<MemoryLoggerEntry>(new[] { item }), (key, oldValues) => oldValues.Union(new List<MemoryLoggerEntry>(new[] { item })).ToList());

                    Console.WriteLine($"{dateTimeUtc:G} [{logLevel}] {text}");

                    return true;
                }
            }
        }


        private static ConcurrentDictionary<string, List<MemoryLoggerEntry>> logs = new ConcurrentDictionary<string, List<MemoryLoggerEntry>>();

        private static string contextName = String.Empty;

        private ILog logger = new MemoryLogger();

        public static List<MemoryLoggerEntry> ContextLogs
        {
            get
            {
                if (logs.ContainsKey(contextName))
                {
                    return logs[contextName];
                }

                return new List<MemoryLoggerEntry>();
            }
        }

        public ILog GetLogger(string name) => this.logger;

        public IDisposable OpenNestedContext(string message)
        {
            lock (this.logger)
            {
                contextName = message;

                Console.WriteLine($"[PID{Process.GetCurrentProcess().Id}] [T{Thread.CurrentThread.ManagedThreadId}] OpenNestedContext({message})");

                return new DestroyNextedContextOnDispose();
            }
        }

        public class DestroyNextedContextOnDispose : IDisposable
        {
            public void Dispose() => contextName = string.Empty;
        }

        public IDisposable OpenMappedContext(string key, string value) => null;
    }
}