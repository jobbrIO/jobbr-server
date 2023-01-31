using System.Collections.Generic;
using Jobbr.ComponentModel.Execution;
using Jobbr.ComponentModel.Execution.Model;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Scheduling;
using Jobbr.Server.Scheduling.Planer;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Jobbr.Server.IntegrationTests.Components.Scheduler
{
    public class TestBase
    {
        protected TestBase()
        {
            Repository = new JobbrRepository(NullLoggerFactory.Instance, new InMemoryJobStorageProvider());

            var executorMock = new Mock<IJobExecutor>();
            executorMock.Setup(e => e.OnPlanChanged(It.IsNotNull<List<PlannedJobRun>>())).Callback<List<PlannedJobRun>>(p => LastIssuedPlan = p);

            PeriodicTimer = new PeriodicTimerMock();

            CurrentTimeProvider = new ManualTimeProvider();

            var job = new Job();
            Repository.AddJob(job);
            DemoJob1Id = job.Id;

            Scheduler = new DefaultScheduler(
                NullLoggerFactory.Instance,
                Repository,
                executorMock.Object,
                new InstantJobRunPlaner(CurrentTimeProvider),
                new ScheduledJobRunPlaner(CurrentTimeProvider),
                new RecurringJobRunPlaner(NullLoggerFactory.Instance, Repository, CurrentTimeProvider),
                new DefaultSchedulerConfiguration(),
                PeriodicTimer,
                CurrentTimeProvider);
        }

        protected long DemoJob1Id { get; }

        protected JobbrRepository Repository { get; }

        protected DefaultScheduler Scheduler { get; }

        protected List<PlannedJobRun> LastIssuedPlan { get; private set; }

        protected PeriodicTimerMock PeriodicTimer { get; }

        protected ManualTimeProvider CurrentTimeProvider { get; }
    }
}