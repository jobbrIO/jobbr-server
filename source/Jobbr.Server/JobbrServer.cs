using System;
using System.ComponentModel.DataAnnotations;

namespace Jobbr.Server
{
    /// <summary>
    /// The jobber server.
    /// </summary>
    public class JobbrServer
    {
        private readonly JobbrConfiguration configuration;

        private DefaultScheduler scheduler;

        public JobbrServer(JobbrConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void Start()
        {
            this.scheduler = new DefaultScheduler(this.configuration.JobRepositoryProvider, this.configuration.JobQueueProvider);
        }

        public void Stop()
        {
            
        }
    }

    /// <summary>
    /// The scheduler watches the repository and its triggers and created new job runs
    /// </summary>
    public class DefaultScheduler : IDisposable
    {
        private readonly IJobRepositoryProvider jobRepositoryProvider;

        private readonly IJobQueueProvider jobQueueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultScheduler" /> class.
        /// </summary>
        /// <param name="jobRepositoryProvider">The Repository</param>
        /// <param name="jobQueueProvider">The QueueProvider</param>
        public DefaultScheduler(IJobRepositoryProvider jobRepositoryProvider, IJobQueueProvider jobQueueProvider)
        {
            this.jobRepositoryProvider = jobRepositoryProvider;
            this.jobQueueProvider = jobQueueProvider;
        }

        public void Dispose()
        {

        }
    }

    public interface IJobQueueProvider
    {

    }

    public interface IJobRepositoryProvider
    {

    }

    public class Job
    {
        [Key]
        public long Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public DateTime CreatedDateTimeUtc { get; set; }

        public DateTime? UpdatedDateTimeUtc { get; set; }
    }

    public class JobInstance
    {
        [Key]
        public long Id { get; set; }

        public long JobId { get; set; }

        public Guid Guid { get; set; }


    }
}
