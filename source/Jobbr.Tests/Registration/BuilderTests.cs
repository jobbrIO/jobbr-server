using System;
using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.ArtefactStorage;
using Jobbr.ComponentModel.ArtefactStorage.Model;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Jobbr.Tests.Integration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Registration
{
    [TestClass]
    public class BuilderTests
    {
        [TestMethod]
        public void RegisterCustomArtefactStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            var builder = new JobbrBuilder();

            builder.Register<IArtefactsStorageProvider>(typeof(CustomArtefactStorageAdapter));
            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            builder.Create();

            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomArtefactStorageAdapter),
                ExposeAllServicesComponent.Instance.ArtefactsStorageProvider.GetType());
        }

        [TestMethod]
        public void RegisterCustomJobStorageProvider_AfterCreation_CorrectTypeIsActivated()
        {
            var builder = new JobbrBuilder();

            builder.Register<IJobStorageProvider>(typeof(CustomJobStorageProvider));
            builder.Register<IJobbrComponent>(typeof(ExposeAllServicesComponent));

            builder.Create();

            Assert.IsNotNull(ExposeAllServicesComponent.Instance.ArtefactsStorageProvider);
            Assert.AreEqual(typeof(CustomJobStorageProvider),
                ExposeAllServicesComponent.Instance.JobStorageProvider.GetType());
        }
    }

    #region Fake Implementations

    public class CustomArtefactStorageAdapter : IArtefactsStorageProvider
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

    public class CustomJobStorageProvider : IJobStorageProvider
    {
        public long AddJob(Job job)
        {
            return 0;
        }

        public List<Job> GetJobs()
        {
            return null;
        }

        public Job GetJobById(long id)
        {
            return null;
        }

        public Job GetJobByUniqueName(string identifier)
        {
            return null;
        }

        public bool Update(Job job)
        {
            return false;
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

        public JobTriggerBase GetTriggerById(long triggerId)
        {
            return null;
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            return null;
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            return null;
        }

        public bool DisableTrigger(long triggerId)
        {
            return false;
        }

        public bool EnableTrigger(long triggerId)
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

        public int AddJobRun(JobRun jobRun)
        {
            return 0;
        }

        public List<JobRun> GetJobRuns()
        {
            return null;
        }

        public JobRun GetJobRunById(long id)
        {
            return null;
        }

        public JobRun GetLastJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            return null;
        }

        public JobRun GetNextJobRunByTriggerId(long triggerId, DateTime utcNow)
        {
            return null;
        }

        public List<JobRun> GetJobRunsByTriggerId(long triggerId)
        {
            return null;
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state)
        {
            return null;
        }

        public List<JobRun> GetJobRunsByUserId(long userId)
        {
            return null;
        }

        public List<JobRun> GetJobRunsByUserName(string userName)
        {
            return null;
        }

        public bool Update(JobRun jobRun)
        {
            return false;
        }

        public bool UpdateProgress(long jobRunId, double? progress)
        {
            return false;
        }

        public bool CheckParallelExecution(long triggerId)
        {
            return false;
        }
    }

}

#endregion
