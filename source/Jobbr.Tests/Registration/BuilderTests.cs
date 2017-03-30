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
        public void AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public List<Job> GetJobs(int page = 0, int pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public Job GetJobById(long id)
        {
            throw new NotImplementedException();
        }

        public Job GetJobByUniqueName(string identifier)
        {
            throw new NotImplementedException();
        }

        public void Update(Job job)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddTrigger(long jobId, ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public JobTriggerBase GetTriggerById(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        {
            throw new NotImplementedException();
        }

        public List<JobTriggerBase> GetActiveTriggers()
        {
            throw new NotImplementedException();
        }

        public void DisableTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void EnableTrigger(long jobId, long triggerId)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void Update(long jobId, RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public void AddJobRun(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRuns(long page = 0, long pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public JobRun GetJobRunById(long id)
        {
            throw new NotImplementedException();
        }

        public JobRun GetLastJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public JobRun GetNextJobRunByTriggerId(long jobId, long triggerId, DateTime utcNow)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByTriggerId(long jobId, long triggerId, long page = 0, long pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByState(JobRunStates state, long page = 0, long pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByUserId(string userId, long page = 0, long pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public List<JobRun> GetJobRunsByUserDisplayName(string userDisplayName, long page = 0, long pageSize = 50)
        {
            throw new NotImplementedException();
        }

        public void Update(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public void UpdateProgress(long jobRunId, double? progress)
        {
            throw new NotImplementedException();
        }

        public bool IsAvailable()
        {
            throw new NotImplementedException();
        }
    }

}

#endregion
