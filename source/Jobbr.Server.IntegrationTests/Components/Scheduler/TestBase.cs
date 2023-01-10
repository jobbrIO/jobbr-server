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
        protected long demoJob1Id = 1;
        protected JobbrRepository repository;
        protected DefaultScheduler scheduler;
        protected List<PlannedJobRun> lastIssuedPlan;
        protected PeriodicTimerMock periodicTimer;
        protected ManualTimeProvider currentTimeProvider;

        public TestBase()
        {
            repository = new JobbrRepository(NullLoggerFactory.Instance, new InMemoryJobStorageProvider());

            var executorMock = new Mock<IJobExecutor>();
            executorMock.Setup(e => e.OnPlanChanged(It.IsNotNull<List<PlannedJobRun>>())).Callback<List<PlannedJobRun>>(p => lastIssuedPlan = p);

            periodicTimer = new PeriodicTimerMock();

            currentTimeProvider = new ManualTimeProvider();

            var job = new Job();
            repository.AddJob(job);
            demoJob1Id = job.Id;

            scheduler = new DefaultScheduler(NullLoggerFactory.Instance, repository, executorMock.Object,
                new InstantJobRunPlaner(currentTimeProvider), new ScheduledJobRunPlaner(currentTimeProvider),
                new RecurringJobRunPlaner(NullLoggerFactory.Instance, repository, currentTimeProvider), new DefaultSchedulerConfiguration(),
                periodicTimer, currentTimeProvider);
        }
    }
}