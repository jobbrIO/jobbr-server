namespace Jobbr.Server.Core
{
    //public interface IJobManagementService
    //{
        //        List<Job> GetAllJobs();

        //        Job GetJobById(long id);

        //        Job AddJob(Job job);

        //        JobRun GetJobRun(long id);

        //        List<JobTriggerBase> GetTriggers(long jobId);

        //        long AddTrigger(RecurringTrigger trigger);

        //        long AddTrigger(ScheduledTrigger trigger);

        //        long AddTrigger(InstantTrigger trigger);

        //        bool DisableTrigger(long triggerId, bool enableNotification);

        //        bool EnableTrigger(long triggerId);

        //        List<JobTriggerBase> GetActiveTriggers();

        //        List<JobRun> GetJobRuns();

        //        List<JobRun> GetJobRuns(JobRunState state);

        //        void UpdateTrigger(long id, JobTriggerBase trigger);

        //        Job GetJobByUniqueName(string identifier);

        //        JobTriggerBase GetTriggerById(long triggerId);

        //        List<JobTriggerBase> GetTriggersByJobId(long jobId);
    //}

    //public class JobManagementService : IJobManagementService
    //{
        //        private readonly IJobbrRepository jobbrRepository;

        //        private readonly IStateService stateService;

        //        public JobManagementService(IJobbrRepository jobbrRepository, IStateService stateService)
        //        {
        //            this.jobbrRepository = jobbrRepository;
        //            this.stateService = stateService;
        //        }

        //        public List<Job> GetAllJobs()
        //        {
        //            return this.jobbrRepository.GetAllJobs();
        //        }

        //        public Job GetJobById(long id)
        //        {
        //            return this.jobbrRepository.GetJob(id);
        //        }

        //        public Job AddJob(Job job)
        //        {
        //            var id = this.jobbrRepository.AddJob(job);

        //            job.Id = id;

        //            return job;
        //        }

        //        public JobRun GetJobRun(long id)
        //        {
        //            return this.jobbrRepository.GetJobRun(id);
        //        }

        //        public List<JobTriggerBase> GetTriggers(long jobId)
        //        {
        //            return this.jobbrRepository.GetTriggersByJobId(jobId);
        //        }

        //        public long AddTrigger(RecurringTrigger trigger)
        //        {
        //            this.jobbrRepository.SaveAddTrigger(trigger);

        //            this.stateService.DipathOnTriggerUpdate(trigger);

        //            return trigger.Id;
        //        }

        //        public long AddTrigger(ScheduledTrigger trigger)
        //        {
        //            this.jobbrRepository.SaveAddTrigger(trigger);

        //            this.stateService.DipathOnTriggerUpdate(trigger);

        //            return trigger.Id;
        //        }
        //        public long AddTrigger(InstantTrigger trigger)
        //        {
        //            this.jobbrRepository.SaveAddTrigger(trigger);
        //            this.stateService.DipathOnTriggerUpdate(trigger);

        //            return trigger.Id;
        //        }

        //        public bool DisableTrigger(long triggerId, bool enableNotification = true)
        //        {
        //            JobTriggerBase trigger;
        //            if (!this.SaveAddTrigger(triggerId, out trigger)) return false;

        //            if (enableNotification)
        //            {
        //                this.stateService.DipathOnTriggerUpdate(trigger);
        //            }

        //            return true;
        //        }

        //        private bool SaveAddTrigger(long triggerId, out JobTriggerBase trigger)
        //        {
        //            trigger = this.jobbrRepository.GetTriggerById(triggerId);

        //            if (!trigger.IsActive)
        //            {
        //                return false;
        //            }

        //            trigger.IsActive = false;
        //            this.jobbrRepository.DisableTrigger(triggerId);
        //            return true;
        //        }

        //        public bool EnableTrigger(long triggerId)
        //        {
        //            JobTriggerBase trigger;
        //            if (!this.jobbrRepository.SaveEnableTrigger(triggerId, out trigger)) return false;

        //            this.stateService.DipathOnTriggerUpdate(trigger);

        //            return true;
        //        }

        //        public List<JobTriggerBase> GetActiveTriggers()
        //        {
        //            return this.jobbrRepository.GetActiveTriggers();
        //        }

        //        public List<JobRun> GetJobRuns()
        //        {
        //            return this.jobbrRepository.GetAllJobRuns();
        //        }

        //        public List<JobRun> GetJobRuns(JobRunState state)
        //        {
        //            return this.jobbrRepository.GetJobRunsByState(state);
        //        }

        //        public void UpdateTrigger(long id, JobTriggerBase trigger)
        //        {
        //            if (id == 0)
        //            {
        //                throw new ArgumentException("JobId is required", "id");
        //            }

        //            bool hadChanges;
        //            var triggerFromDb = this.jobbrRepository.SaveUpdateTrigger(id, trigger, out hadChanges);

        //            if (hadChanges)
        //            {
        //                this.stateService.DipathOnTriggerUpdate(triggerFromDb);
        //            }
        //        }

        //        public Job GetJobByUniqueName(string identifier)
        //        {
        //            return this.jobbrRepository.GetJobByUniqueName(identifier);
        //        }

        //        public JobTriggerBase GetTriggerById(long triggerId)
        //        {
        //            return this.jobbrRepository.GetTriggerById(triggerId);
        //        }

        //        public List<JobTriggerBase> GetTriggersByJobId(long jobId)
        //        {
        //            return this.jobbrRepository.GetTriggersByJobId(jobId);
        //        }
    //}
}