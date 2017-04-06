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

namespace Jobbr.Tests.Components.Scheduler
{
    [TestClass]
    public class PlanningTests
    {
        private readonly long demoJob1Id = 1;

        private readonly JobbrRepository repository;
        private readonly NewScheduler scheduler;

        private List<PlannedJobRun> lastIssuedPlan;
        private readonly PeriodicTimerMock periodicTimer;
        private readonly ManualTimeProvider currentTimeProvider;

        public PlanningTests()
        {
            this.repository = new JobbrRepository(new InMemoryJobStorageProvider());

            var executorMock = new Mock<IJobExecutor>();
            executorMock.Setup(e => e.OnPlanChanged(It.IsNotNull<List<PlannedJobRun>>())).Callback<List<PlannedJobRun>>(p => this.lastIssuedPlan = p);

            this.periodicTimer = new PeriodicTimerMock();

            this.currentTimeProvider = new ManualTimeProvider();

            var job = new Job();
            this.repository.AddJob(job);
            this.demoJob1Id = job.Id;

            this.scheduler = new NewScheduler(this.repository, executorMock.Object, new InstantJobRunPlaner(this.currentTimeProvider), new ScheduledJobRunPlaner(this.currentTimeProvider), new RecurringJobRunPlaner(this.repository, this.currentTimeProvider), new DefaultSchedulerConfiguration(), this.periodicTimer, this.currentTimeProvider);

            this.scheduler.Start();
        }

        private void AddAndSignalNewTrigger(long jobId, InstantTrigger trigger)
        {
            this.repository.SaveAddTrigger(jobId, trigger);
            this.scheduler.OnTriggerAdded(jobId, trigger.Id);
        }

        private void AddAndSignalNewTrigger(long jobId, ScheduledTrigger trigger)
        {
            this.repository.SaveAddTrigger(jobId, trigger);
            this.scheduler.OnTriggerAdded(jobId, trigger.Id);
        }

        private void AddAndSignalNewTrigger(long jobId, RecurringTrigger trigger)
        {
            this.repository.SaveAddTrigger(jobId, trigger);
            this.scheduler.OnTriggerAdded(jobId, trigger.Id);
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            Assert.AreEqual(1, this.lastIssuedPlan.Count, "A scheduled trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewInstantTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new InstantTrigger() { JobId = this.demoJob1Id, IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            Assert.AreEqual(1, this.lastIssuedPlan.Count, "A instant trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_CreatesANewJobRun()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "A scheduled trigger should create exact one jobrun when added");
            Assert.AreEqual(scheduledTrigger.Id, jobRuns.Single().TriggerId, "The jobrun should reference the trigger that cause the job to run");
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_IsPlannedOnTime()
        {
            var dateTimeUtc = DateTime.UtcNow.AddHours(10);

            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = dateTimeUtc, IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            var jobRun = this.repository.GetJobRuns().Single();

            Assert.AreEqual(dateTimeUtc, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should be considered in the plan");
            Assert.AreEqual(jobRun.Id, this.lastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void NewRecurringTriggerWithoutSeconds_IsAdded_IsPlannedOnTime()
        {
            var dateTimeUtc = new DateTime(DateTime.UtcNow.Year + 1, 09, 02, 17, 00, 00);

            var scheduledTrigger = new RecurringTrigger() { Definition = "* * * * *", JobId = this.demoJob1Id, StartDateTimeUtc = dateTimeUtc, IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            var jobRun = this.repository.GetJobRuns().Single();

            var expectedTime = dateTimeUtc.AddMinutes(1);
            Assert.AreEqual(expectedTime, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(jobRun.Id, this.lastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void NewRecurringTriggerWithSeconds_IsAdded_DoesNotContainsSecondInTrigger()
        {
            var dateTimeUtc = new DateTime(DateTime.UtcNow.Year + 1, 02, 09, 17, 00, 15);

            var scheduledTrigger = new RecurringTrigger() { Definition = "* * * * *", StartDateTimeUtc = dateTimeUtc, JobId = this.demoJob1Id, IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            var jobRun = this.repository.GetJobRuns().Single();

            var expectedTime = dateTimeUtc.AddMinutes(1).AddSeconds(-15);
            Assert.AreEqual(expectedTime, this.lastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(jobRun.Id, this.lastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void RecurringTrigger_AfterTwoMinutes_IsPlannedMultipleTimes()
        {
            var dateTimeUtc = new DateTime(2017, 02, 09, 14, 00, 00);

            this.currentTimeProvider.Set(dateTimeUtc);

            var scheduledTrigger = new RecurringTrigger() { Definition = "* * * * *", JobId = this.demoJob1Id, StartDateTimeUtc = dateTimeUtc, IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            this.currentTimeProvider.AddMinute();
            this.periodicTimer.CallbackOnce();

            this.currentTimeProvider.AddMinute();
            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            var expectedTime1 = dateTimeUtc.AddMinutes(1);
            var expectedTime2 = dateTimeUtc.AddMinutes(2);
            var expectedTime3 = dateTimeUtc.AddMinutes(3);

            Assert.AreEqual(3, jobRuns.Count);

            Assert.AreEqual(expectedTime1, this.lastIssuedPlan[0].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(expectedTime2, this.lastIssuedPlan[1].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(expectedTime3, this.lastIssuedPlan[2].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
        }

        [TestMethod]
        public void ScheduledTrigger_HasCompletedJobRun_DoesNotTriggerNewOne()
        {
            // Note: The Scheduled Trigger needs to be in the past in order to invalidate the job reliable in this testing scenario (Issues with NCrunch, no issues with R# and VS-Runners)
            var scheduledTrigger = new ScheduledTrigger { JobId = this.demoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            this.AddAndSignalNewTrigger(this.demoJob1Id, scheduledTrigger);

            // Simulate Job Completeness
            var jobRunByScheduledTrigger = this.repository.GetJobRuns().Single(jr => jr.TriggerId == scheduledTrigger.Id);
            jobRunByScheduledTrigger.State = JobRunStates.Completed;
            this.repository.Update(jobRunByScheduledTrigger);

            this.scheduler.OnJobRunEnded(jobRunByScheduledTrigger.Id);
            
            Assert.AreEqual(0, this.lastIssuedPlan.Count, "A scheduled trigger should not cause any additional jobruns after completion");
        }

        [TestMethod]
        public void RecurringTrigger_HasCompletedJobRun_TriggerNewOne()
        {
            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, NoParallelExecution = false, StartDateTimeUtc = DateTime.UtcNow };
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            // Simulate Job Completeness
            var jobRunByScheduledTrigger = this.repository.GetJobRuns().Single(jr => jr.TriggerId == recurringTrigger.Id);
            jobRunByScheduledTrigger.State = JobRunStates.Completed;
            this.repository.Update(jobRunByScheduledTrigger);

            this.scheduler.OnJobRunEnded(jobRunByScheduledTrigger.Id);

            var jobRun = this.repository.GetJobRuns();

            Assert.AreEqual(2, jobRun.Count, "Trigger should have triggered an additional job after completion of the first");
            Assert.AreEqual(1, this.lastIssuedPlan.Count, "A scheduled trigger should not cause any additional jobruns after completion");
        }

        [TestMethod]
        public void RecurringTrigger_WithNoTriggerOrJobChanges_DoesTriggerNewOnes()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            this.currentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, NoParallelExecution = false};
            
            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            // wait for additional jobrun
            this.currentTimeProvider.Set(initialDate.AddHours(2));
            this.periodicTimer.CallbackOnce();

            var jobRun = this.repository.GetJobRuns();

            Assert.AreEqual(2, jobRun.Count, "Trigger should continue trigger additional jobruns");
            Assert.AreEqual(2, this.lastIssuedPlan.Count, "The plan should contain items for all 3 triggers");
        }

        [TestMethod]
        public void NoParallelExecutionDisabled_ForceNewPlanWhileJobIsStillRunning_NextJobRunIsCreated()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            this.currentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, NoParallelExecution = false };
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            // Simulate that the jobRun has started
            var addedJobRun = this.repository.GetJobRunsByTriggerId(recurringTrigger.JobId, recurringTrigger.Id).Single();
            addedJobRun.State = JobRunStates.Processing;
            this.repository.Update(addedJobRun);

            // Make sure the cron in the recurring trigger will base on an updated "now"
            this.currentTimeProvider.Set(initialDate.AddHours(2));
            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(2, jobRuns.Count);
            Assert.AreEqual(2, this.lastIssuedPlan.Count, "Since one JobRun has completed, there should be now 2 jobruns");
            Assert.AreEqual(2, this.repository.GetJobRuns().Count, "The recurring trigger should should have triggered 2");

            Assert.AreEqual(jobRuns[0].Id, this.lastIssuedPlan[0].Id);
            Assert.AreEqual(jobRuns[1].Id, this.lastIssuedPlan[1].Id);
        }

        [TestMethod]
        public void NoParallelExecutionEnabled_TriggerWhileJobIsStillRunning_NextJobRunIsPrevented()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            this.currentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, NoParallelExecution = true };

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            // Simulate that the jobRun has started
            var addedJobRun = this.repository.GetJobRunsByTriggerId(recurringTrigger.JobId, recurringTrigger.Id).Single();
            addedJobRun.State = JobRunStates.Processing;
            this.repository.Update(addedJobRun);

            // Make sure the cron in the recurring trigger will base on an updated "now"
            this.currentTimeProvider.Set(initialDate.AddHours(2));
            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "Creating new jobruns should be prevented if a JobRun is not yet completed for the trigger");
            Assert.AreEqual(1, this.lastIssuedPlan.Count, "It doesn't mather how often the Callback for recurring trigger scheduling is called, as long as there is a job running, there shoulnd be any additional jobs");
        }

        [TestMethod]
        public void RecurringTrigger_WhenAddedBeforeStart_ShouldTriggerOneRun()
        {
            this.scheduler.Stop();

            var futureDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            this.currentTimeProvider.Set(futureDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true };

            // This triggers the first jobrun
            this.repository.SaveAddTrigger(this.demoJob1Id, recurringTrigger);

            this.scheduler.Start();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "There should only be one jobrun");
            Assert.AreEqual(futureDate.Date, jobRuns[0].PlannedStartDateTimeUtc.Date);
            Assert.AreEqual(futureDate.Hour, jobRuns[0].PlannedStartDateTimeUtc.Hour);
            Assert.AreEqual(futureDate.Minute + 1, jobRuns[0].PlannedStartDateTimeUtc.Minute);
            Assert.AreEqual(0, jobRuns[0].PlannedStartDateTimeUtc.Second);
        }

        [TestMethod]
        public void RecurringTrigger_WhileRunIsIncomplete_ShouldNotRaiseNewRunsAtTheSameTime()
        {
            var futureDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            this.currentTimeProvider.Set(futureDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true};

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            this.currentTimeProvider.AddSecond();
            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "The periodic callback should not create new jobruns if they would start at the same time (== planned starttime has not changed)");
        }

        [TestMethod]
        public void RecurringTrigger_EndDateInPast_DoesNotTriggerRun()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            this.currentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, EndDateTimeUtc = currentNow.AddDays(-1)};

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(0, jobRuns.Count, "The trigger is not valid anymore and should not trigger a run");
        }

        [TestMethod]
        public void RecurringTrigger_StartDateInFuture_FirstRunIsAtStartDate()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            this.currentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, StartDateTimeUtc = currentNow.AddDays(1) };

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "A startdate in the future should trigger the run");
            Assert.AreEqual(currentNow.AddDays(1).Date, jobRuns[0].PlannedStartDateTimeUtc.Date);
        }

        [TestMethod]
        public void RecurringTrigger_StartDateInPast_FirstRunIsAtCurrentNow()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            this.currentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, StartDateTimeUtc = currentNow.AddDays(-1) };

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "A startdate in the future should trigger the run");
            Assert.AreEqual(currentNow.Date, jobRuns[0].PlannedStartDateTimeUtc.Date);
        }

        [TestMethod]
        public void RecurringTrigger_StartAndEndDateCoversNow_DoesNotTriggerRun()
        {
            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = this.demoJob1Id, IsActive = true, StartDateTimeUtc = DateTime.UtcNow.AddDays(-1), EndDateTimeUtc = DateTime.UtcNow.AddDays(1)};

            // This triggers the first jobrun
            this.AddAndSignalNewTrigger(this.demoJob1Id, recurringTrigger);

            this.periodicTimer.CallbackOnce();

            var jobRuns = this.repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Count, "The trigger should cause a job run because its valid right now");
        }
    }
}
