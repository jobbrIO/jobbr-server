using System;
using System.Collections.Generic;
using System.IO;
using Jobbr.ComponentModel.Management;
using Jobbr.ComponentModel.Management.Model;

namespace Jobbr.Server.ComponentServices.Management
{
    internal class JobManagementService : IJobManagementService
    {
        public JobManagementService()
        {
            
        }

        public Job AddJob(Job job)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(RecurringTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(ScheduledTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public long AddTrigger(InstantTrigger trigger)
        {
            throw new NotImplementedException();
        }

        public bool DisableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public bool EnableTrigger(long triggerId)
        {
            throw new NotImplementedException();
        }

        public void UpdateTriggerDefinition(long triggerId, string definition)
        {
            throw new NotImplementedException();
        }

        public void UpdatetriggerStartTime(long triggerId, DateTime startDateTimeUtc)
        {
            throw new NotImplementedException();
        }

        public List<JobArtefact> GetArtefactForJob(JobRun jobRun)
        {
            throw new NotImplementedException();
        }

        public Stream GetArtefactAsStream(JobRun jobRun, string filename)
        {
            throw new NotImplementedException();
        }
    }
}