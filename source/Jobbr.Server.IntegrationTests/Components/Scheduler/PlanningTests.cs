using System;
using System.Linq;
using Jobbr.ComponentModel.JobStorage.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JobRunStates = Jobbr.ComponentModel.JobStorage.Model.JobRunStates;

namespace Jobbr.Server.IntegrationTests.Components.Scheduler
{
    [TestClass]
    public class PlanningTests : TestBase
    {
        public PlanningTests()
        {
            Scheduler.Start();
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = DemoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            Assert.AreEqual(1, LastIssuedPlan.Count, "A scheduled trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewInstantTrigger_IsAdded_WillBePlanned()
        {
            var scheduledTrigger = new InstantTrigger() { JobId = DemoJob1Id, IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            Assert.AreEqual(1, LastIssuedPlan.Count, "A instant trigger should cause one item in the plan");
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_CreatesANewJobRun()
        {
            var scheduledTrigger = new ScheduledTrigger { JobId = DemoJob1Id, StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1), IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "A scheduled trigger should create exact one jobrun when added");
            Assert.AreEqual(scheduledTrigger.Id, jobRuns.Items.Single().Trigger.Id, "The jobrun should reference the trigger that cause the job to run");
        }

        [TestMethod]
        public void NewScheduledTrigger_IsAdded_IsPlannedOnTime()
        {
            var dateTimeUtc = DateTime.UtcNow.AddHours(10);

            var scheduledTrigger = new ScheduledTrigger { JobId = DemoJob1Id, StartDateTimeUtc = dateTimeUtc, IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            var jobRun = Repository.GetJobRuns().Items.Single();

            Assert.AreEqual(dateTimeUtc, LastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should be considered in the plan");
            Assert.AreEqual(jobRun.Id, LastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void NewRecurringTriggerWithoutSeconds_IsAdded_IsPlannedOnTime()
        {
            var dateTimeUtc = new DateTime(DateTime.UtcNow.Year + 1, 09, 02, 17, 00, 00);

            var scheduledTrigger = new RecurringTrigger() { Definition = "* * * * *", JobId = DemoJob1Id, StartDateTimeUtc = dateTimeUtc, IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            var jobRun = Repository.GetJobRuns().Items.Single();

            var expectedTime = dateTimeUtc.AddMinutes(1);
            Assert.AreEqual(expectedTime, LastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(jobRun.Id, LastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void NewRecurringTriggerWithSeconds_IsAdded_DoesNotContainsSecondInTrigger()
        {
            var dateTimeUtc = new DateTime(DateTime.UtcNow.Year + 1, 02, 09, 17, 00, 15);

            var scheduledTrigger = new RecurringTrigger { Definition = "* * * * *", StartDateTimeUtc = dateTimeUtc, JobId = DemoJob1Id, IsActive = true };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            var jobRun = Repository.GetJobRuns().Items.Single();

            var expectedTime = dateTimeUtc.AddMinutes(1).AddSeconds(-15);
            Assert.AreEqual(expectedTime, LastIssuedPlan.Single().PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(jobRun.Id, LastIssuedPlan.Single().Id, "The startdate should be considered in the plan");
        }

        [TestMethod]
        public void RecurringTrigger_AfterTwoMinutes_IsPlannedMultipleTimes()
        {
            var dateTimeUtc = new DateTime(2017, 02, 09, 14, 00, 00);

            CurrentTimeProvider.Set(dateTimeUtc);

            var scheduledTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                StartDateTimeUtc = dateTimeUtc,
                IsActive = true
            };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            CurrentTimeProvider.AddMinute();
            PeriodicTimer.CallbackOnce();

            CurrentTimeProvider.AddMinute();
            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            var expectedTime1 = dateTimeUtc.AddMinutes(1);
            var expectedTime2 = dateTimeUtc.AddMinutes(2);
            var expectedTime3 = dateTimeUtc.AddMinutes(3);

            Assert.AreEqual(3, jobRuns.Items.Count);

            Assert.AreEqual(expectedTime1, LastIssuedPlan[0].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(expectedTime2, LastIssuedPlan[1].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
            Assert.AreEqual(expectedTime3, LastIssuedPlan[2].PlannedStartDateTimeUtc, "The startdate should match the StartDateTimeUtc + 1 Minute");
        }

        [TestMethod]
        public void ScheduledTrigger_HasCompletedJobRun_DoesNotTriggerNewOne()
        {
            // Note: The Scheduled Trigger needs to be in the past in order to invalidate the job reliable in this testing scenario (Issues with NCrunch, no issues with R# and VS-Runners)
            var scheduledTrigger = new ScheduledTrigger
            {
                JobId = DemoJob1Id,
                StartDateTimeUtc = DateTime.UtcNow.AddSeconds(-1),
                IsActive = true
            };
            AddAndSignalNewTrigger(DemoJob1Id, scheduledTrigger);

            // Simulate Job Completeness
            var jobRunByScheduledTrigger = Repository.GetJobRuns().Items.Single(jr => jr.Trigger.Id == scheduledTrigger.Id);
            jobRunByScheduledTrigger.State = JobRunStates.Completed;
            Repository.Update(jobRunByScheduledTrigger);

            Scheduler.OnJobRunEnded(jobRunByScheduledTrigger.Id);

            Assert.AreEqual(0, LastIssuedPlan.Count, "A scheduled trigger should not cause any additional jobruns after completion");
        }

        [TestMethod]
        public void RecurringTrigger_HasCompletedJobRun_TriggerNewOne()
        {
            var recurringTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                IsActive = true,
                NoParallelExecution = false,
                StartDateTimeUtc = DateTime.UtcNow
            };
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            // Simulate Job Completeness
            var jobRunByScheduledTrigger = Repository.GetJobRuns().Items.Single(jr => jr.Trigger.Id == recurringTrigger.Id);
            jobRunByScheduledTrigger.State = JobRunStates.Completed;
            Repository.Update(jobRunByScheduledTrigger);

            Scheduler.OnJobRunEnded(jobRunByScheduledTrigger.Id);

            var jobRun = Repository.GetJobRuns();

            Assert.AreEqual(2, jobRun.Items.Count, "Trigger should have triggered an additional job after completion of the first");
            Assert.AreEqual(1, LastIssuedPlan.Count, "A scheduled trigger should not cause any additional jobruns after completion");
        }

        [TestMethod]
        public void RecurringTrigger_WithNoTriggerOrJobChanges_DoesTriggerNewOnes()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            CurrentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                IsActive = true,
                NoParallelExecution = false
            };

            // This triggers the first job run
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            // wait for additional job run
            CurrentTimeProvider.Set(initialDate.AddHours(2));
            PeriodicTimer.CallbackOnce();

            var jobRun = Repository.GetJobRuns();

            Assert.AreEqual(2, jobRun.Items.Count, "Trigger should continue trigger additional jobruns");
            Assert.AreEqual(2, LastIssuedPlan.Count, "The plan should contain items for all 3 triggers");
        }

        [TestMethod]
        public void NoParallelExecutionDisabled_ForceNewPlanWhileJobIsStillRunning_NextJobRunIsCreated()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            CurrentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true, NoParallelExecution = false };
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            // Simulate that the jobRun has started
            var addedJobRun = Repository.GetJobRunsByTriggerId(recurringTrigger.JobId, recurringTrigger.Id).Items.Single();
            addedJobRun.State = JobRunStates.Processing;
            Repository.Update(addedJobRun);

            // Make sure the cron in the recurring trigger will base on an updated "now"
            CurrentTimeProvider.Set(initialDate.AddHours(2));
            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(2, jobRuns.Items.Count);
            Assert.AreEqual(2, LastIssuedPlan.Count, "Since one JobRun has completed, there should be now 2 jobruns");
            Assert.AreEqual(2, Repository.GetJobRuns().Items.Count, "The recurring trigger should should have triggered 2");
        }

        [TestMethod]
        public void NoParallelExecutionEnabled_TriggerWhileJobIsStillRunning_NextJobRunIsPrevented()
        {
            var initialDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            CurrentTimeProvider.Set(initialDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true, NoParallelExecution = true };

            // This triggers the first jobrun
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            // Simulate that the jobRun has started
            var addedJobRun = Repository.GetJobRunsByTriggerId(recurringTrigger.JobId, recurringTrigger.Id).Items.Single();
            addedJobRun.State = JobRunStates.Processing;
            Repository.Update(addedJobRun);

            // Make sure the cron in the recurring trigger will base on an updated "now"
            CurrentTimeProvider.Set(initialDate.AddHours(2));
            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "Creating new jobruns should be prevented if a JobRun is not yet completed for the trigger");
            Assert.AreEqual(1, LastIssuedPlan.Count, "It doesn't mather how often the Callback for recurring trigger scheduling is called, as long as there is a job running, there shoulnd be any additional jobs");
        }

        [TestMethod]
        public void RecurringTrigger_WhenAddedBeforeStart_ShouldTriggerOneRun()
        {
            Scheduler.Stop();

            var futureDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            CurrentTimeProvider.Set(futureDate);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true };

            // This triggers the first job run
            Repository.AddJob(new Job { Id = DemoJob1Id });
            Repository.SaveAddTrigger(DemoJob1Id, recurringTrigger);

            Scheduler.Start();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "There should only be one jobrun");
            Assert.AreEqual(futureDate.Date, jobRuns.Items[0].PlannedStartDateTimeUtc.Date);
            Assert.AreEqual(futureDate.Hour, jobRuns.Items[0].PlannedStartDateTimeUtc.Hour);
            Assert.AreEqual(futureDate.Minute + 1, jobRuns.Items[0].PlannedStartDateTimeUtc.Minute);
            Assert.AreEqual(0, jobRuns.Items[0].PlannedStartDateTimeUtc.Second);
        }

        [TestMethod]
        public void RecurringTrigger_WhileRunIsIncomplete_ShouldNotRaiseNewRunsAtTheSameTime()
        {
            var futureDate = new DateTime(2017, 02, 01, 15, 42, 12, DateTimeKind.Utc);
            CurrentTimeProvider.Set(futureDate);

            var recurringTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                IsActive = true
            };

            // This triggers the first job run
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            CurrentTimeProvider.AddSecond();
            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "The periodic callback should not create new jobruns if they would start at the same time (== planned starttime has not changed)");
        }

        [TestMethod]
        public void RecurringTrigger_EndDateInPast_DoesNotTriggerRun()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            CurrentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                IsActive = true,
                EndDateTimeUtc = currentNow.AddDays(-1)
            };

            // This triggers the first job run
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(0, jobRuns.Items.Count, "The trigger is not valid anymore and should not trigger a run");
        }

        [TestMethod]
        public void RecurringTrigger_StartDateInFuture_FirstRunIsAtStartDate()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            CurrentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true, StartDateTimeUtc = currentNow.AddDays(1) };

            // This triggers the first jobrun
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "A startdate in the future should trigger the run");
            Assert.AreEqual(currentNow.AddDays(1).Date, jobRuns.Items[0].PlannedStartDateTimeUtc.Date);
        }

        [TestMethod]
        public void RecurringTrigger_StartDateInPast_FirstRunIsAtCurrentNow()
        {
            var currentNow = new DateTime(2017, 02, 01, 15, 42, 00, DateTimeKind.Utc);
            CurrentTimeProvider.Set(currentNow);

            var recurringTrigger = new RecurringTrigger { Definition = "* * * * *", JobId = DemoJob1Id, IsActive = true, StartDateTimeUtc = currentNow.AddDays(-1) };

            // This triggers the first jobrun
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "A startdate in the future should trigger the run");
            Assert.AreEqual(currentNow.Date, jobRuns.Items[0].PlannedStartDateTimeUtc.Date);
        }

        [TestMethod]
        public void RecurringTrigger_StartAndEndDateCoversNow_DoesNotTriggerRun()
        {
            var recurringTrigger = new RecurringTrigger
            {
                Definition = "* * * * *",
                JobId = DemoJob1Id,
                IsActive = true,
                StartDateTimeUtc = DateTime.UtcNow.AddDays(-1),
                EndDateTimeUtc = DateTime.UtcNow.AddDays(1)
            };

            // This triggers the first job run
            AddAndSignalNewTrigger(DemoJob1Id, recurringTrigger);

            PeriodicTimer.CallbackOnce();

            var jobRuns = Repository.GetJobRuns();

            Assert.AreEqual(1, jobRuns.Items.Count, "The trigger should cause a job run because its valid right now");
        }

        private void AddAndSignalNewTrigger(long jobId, InstantTrigger trigger)
        {
            Repository.SaveAddTrigger(jobId, trigger);
            Scheduler.OnTriggerAdded(jobId, trigger.Id);
        }

        private void AddAndSignalNewTrigger(long jobId, ScheduledTrigger trigger)
        {
            Repository.SaveAddTrigger(jobId, trigger);
            Scheduler.OnTriggerAdded(jobId, trigger.Id);
        }

        private void AddAndSignalNewTrigger(long jobId, RecurringTrigger trigger)
        {
            Repository.SaveAddTrigger(jobId, trigger);
            Scheduler.OnTriggerAdded(jobId, trigger.Id);
        }
    }
}
