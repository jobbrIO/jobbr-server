using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobRunStates = Jobbr.ComponentModel.JobStorage.Model.JobRunStates;

namespace Jobbr.Tests.Integration.Startup
{

    #region Emplty Implementatios

    public class PseudoJobStorageProvider : IJobStorageProvider
    {
        public List<Job> GetJobs()
        {
            return new List<Job>();
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
            return new List<JobTriggerBase>();
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

        public JobRun GetJobRunById(Guid uniqueId)
        {
            return null;
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

    #endregion

    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        public void ConsoleCapturer_WhenActive_ContainsConsoleOut()
        {
            using (var capture = new ConsoleCapturer())
            {
                Console.WriteLine("Hello World");

                var allLogEntries = capture.GetLines().ToList();

                Assert.IsTrue(allLogEntries.Any());
                Assert.AreEqual("Hello World", allLogEntries[0]);
                Assert.AreEqual(2, allLogEntries.Count);
            }
        }

        [TestMethod]
        public void CreateJobber_WithConsoleCapturer_CaptureMessagesFromConsole()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                builder.Create();

                var allLogEntries = capture.GetLines();
                Assert.IsTrue(allLogEntries.Any());
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoStorageProvider_IssuesError()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                // Register only Artefacts and Executor
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                builder.Create();

                var storageWarnLogs = capture.GetLines("ERROR", "JobStorageProvider").ToList();

                Assert.IsTrue(storageWarnLogs.Any());

                Assert.AreEqual(1, storageWarnLogs.Count);
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoArtefactsProvider_IssuesWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                // Register only Executor and JobStorage
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

                builder.Create();

                var artefactsWarnings = capture.GetLines("WARN", "Artefacts").ToList();

                Assert.IsTrue(artefactsWarnings.Any());
                Assert.AreEqual(1, artefactsWarnings.Count);
            }
        }

        [TestMethod]
        public void CreateJobbr_WithNoExecutor_IssuesError()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));

                builder.Create();

                var artefactsWarnings = capture.GetLines("ERROR", "Executor").ToList();

                Assert.IsTrue(artefactsWarnings.Any());
                Assert.AreEqual(1, artefactsWarnings.Count);
            }
        }

        [TestMethod]
        public void CreateJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                builder.Create();

                var errors = capture.GetLines("ERROR").ToList();
                var warnings = capture.GetLines("WARN").ToList();

                Assert.IsFalse(errors.Any());
                Assert.IsFalse(warnings.Any());
            }
        }

        [TestMethod]
        public void StartJobbr_WithAllRequiredComponents_NoErrorNoWarn()
        {
            using (var capture = new ConsoleCapturer())
            {
                var builder = new JobbrBuilder();

                // Register only Artefacts and JoStorage
                builder.Register<IArtefactsStorageProvider>(typeof(PseudoArfetacstStorageProvider));
                builder.Register<IJobStorageProvider>(typeof(PseudoJobStorageProvider));
                builder.Register<IJobExecutor>(typeof(PseudoExecutor));

                var server = builder.Create();

                server.Start();

                var errors = capture.GetLines("ERROR").ToList();
                var warnings = capture.GetLines("WARN").ToList();
                var fatals = capture.GetLines("FATAL").ToList();

                Assert.IsFalse(fatals.Any(), "Got too manny fatals: \n\n * " + string.Join("\n * ", fatals));
                Assert.IsFalse(errors.Any(), "Got too manny errors: \n\n * " + string.Join("\n * ", errors));
                Assert.IsFalse(warnings.Any(), "Got too manny warnings: \n\n * " + string.Join("\n * ", warnings));
            }
        }
    }
}