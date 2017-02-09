using System;
using System.Collections.Generic;
using System.Linq;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Scheduling.Planer;
using Jobbr.Server.Storage;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using JobRunStates = Jobbr.ComponentModel.JobStorage.Model.JobRunStates;

namespace Jobbr.Tests.Components
{
    [TestClass]
    public class SchedulerTests
    {
        private long DemoJob1Id = 1;
        private long DemoJob2Id = 2;

        private readonly JobbrRepository repository;
        private readonly NewScheduler scheduler;

        private List<PlannedJobRun> lastIssuedPlan;

        public SchedulerTests()
        {
            this.repository = new JobbrRepository(new InMemoryJobStorageProvider());

            var executorMock = new Mock<IJobExecutor>();

            executorMock.Setup(e => e.OnPlanChanged(It.IsNotNull<List<PlannedJobRun>>())).Callback<List<PlannedJobRun>>(p => this.lastIssuedPlan = p);

            this.DemoJob1Id = this.repository.AddJob(new Job());
            this.DemoJob2Id = this.repository.AddJob(new Job());

            this.scheduler = new NewScheduler(this.repository, executorMock.Object, new InstantJobRunPlaner(), new ScheduledJobRunPlaner(), new RecurringJobRunPlaner(this.repository), new DefaultSchedulerConfiguration());

            this.scheduler.Start();
        }

        private void AddAndSignalNewTrigger(InstantTrigger trigger)
        {
            this.repository.SaveAddTrigger(trigger);
            this.scheduler.OnTriggerAdded(trigger.Id);
        }

        private void AddAndSignalNewTrigger(ScheduledTrigger trigger)
        {
            this.repository.SaveAddTrigger(trigger);
            this.scheduler.OnTriggerAdded(trigger.Id);
        }

        private void AddAndSignalNewTrigger(RecurringTrigger trigger)
        {
            this.repository.SaveAddTrigger(trigger);
            this.scheduler.OnTriggerAdded(trigger.Id);
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            this.AddAndSignalNewTrigger(scheduledTrigger);

            Assert.AreEqual(1, this.lastIssuedPlan.Count, "A scheduled trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewInstantTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new InstantTrigger() { JobId = this.demoJob1Id, IsActive = true };
            this.AddAndSignalNewTrigger(scheduledTrigger);

            Assert.AreEqual(1, this.lastIssuedPlan.Count, "A instant trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_CreatesANewJobRun()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            this.AddAndSignalNewTrigger(scheduledTrigger);

            var jobRuns = this.repository.GetAllJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "A scheduled trigger should create exact one jobrun when added");
            Assert.AreEqual(scheduledTrigger.Id, jobRuns.Single().TriggerId, "The jobrun should reference the trigger that cause the job to run");
        }
        public void ScheduledTrigger_HasCompletedJobRun_DoesNotTriggerNewOne()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = DemoJob2Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(10), IsActive = true };
            this.repository.SaveAddTrigger(scheduledTrigger);
            this.scheduler.OnTriggerAdded(scheduledTrigger.Id);

            var jobRunByScheduledTrigger = this.repository.GetNextJobRunByTriggerId(scheduledTrigger.Id);
            jobRunByScheduledTrigger.State = JobRunStates.Completed;
            this.repository.Update(jobRunByScheduledTrigger);

            this.scheduler.OnJobRunEnded(jobRunByScheduledTrigger.UniqueId);

            Assert.AreEqual(0, this.lastIssuedPlan.Count, "A scheduled trigger should not cause any additional jobruns after completion");
        }

        [TestMethod]
        public void NoParallelExecutionDisabled_ForceNewPlanWhileJobIsStillRunning_NextJobRunIsCreated()
        {
            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true, NoParallelExecution = false, StartDateTimeUtc = DateTime.UtcNow.AddDays(-1) };
            this.repository.SaveAddTrigger(recurringTrigger);
            this.scheduler.OnTriggerAdded(recurringTrigger.Id);

            var scheduledTrigger = new ScheduledTrigger { JobId = DemoJob2Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(10), IsActive = true };
            this.repository.SaveAddTrigger(scheduledTrigger);
            this.scheduler.OnTriggerAdded(scheduledTrigger.Id);

            // Let's simulate that the JobRun by the Recurring Trigger has now started
            var firstJobRunByTheRecurringTrigger = this.repository.GetLastJobRunByTriggerId(recurringTrigger.Id);
            firstJobRunByTheRecurringTrigger.State = JobRunStates.Processing;
            this.repository.Update(firstJobRunByTheRecurringTrigger);

            // Let's manually force the scheduler to do a re-evaluation of the whole plan after a job has ended
            var firsJobRunByScheduledtrigger = this.repository.GetLastJobRunByTriggerId(scheduledTrigger.Id);
            firsJobRunByScheduledtrigger.State = JobRunStates.Completed;
            this.repository.Update(firsJobRunByScheduledtrigger);
            this.scheduler.OnJobRunEnded(firsJobRunByScheduledtrigger.UniqueId);

            var jobRunsTriggeredByRecurringTrigger = this.repository.GetAllJobRuns().Where(jr => jr.TriggerId == recurringTrigger.Id).OrderBy(jr => jr.Id).ToList();

            Assert.AreEqual(2, jobRunsTriggeredByRecurringTrigger.Count);
            Assert.AreEqual(2, this.lastIssuedPlan.Count, "Since one JobRun has completed, there should be only 2 jobs in the upcoming plan, even if the recurring trigger already issued a new run");
            Assert.AreEqual(3, this.repository.GetAllJobRuns().Count, "The recurring trigger should should have triggered 2 JobRuns, while the scheduled trigger should also have contributed 1 run");

            Assert.AreEqual(jobRunsTriggeredByRecurringTrigger[0].UniqueId, this.lastIssuedPlan[0].UniqueId);
            Assert.AreEqual(jobRunsTriggeredByRecurringTrigger[1].UniqueId, this.lastIssuedPlan[1].UniqueId);
        }

        [TestMethod]
        public void NoParallelExecutionEnabled_TriggerWhileJobIsStillRunning_NextJobRunIsPrevented()
        {
        }

    }
}
