using System;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Interface for job schedulers.
    /// </summary>
    public interface IJobScheduler : IDisposable
    {
        /// <summary>
        /// Start scheduler.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop scheduler.
        /// </summary>
        void Stop();

        /// <summary>
        /// Event handler for updated trigger definitions.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void OnTriggerDefinitionUpdated(long jobId, long triggerId);

        /// <summary>
        /// Event handler for updated trigger states.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void OnTriggerStateUpdated(long jobId, long triggerId);

        /// <summary>
        /// Event handler for trigger adding events.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void OnTriggerAdded(long jobId, long triggerId);

        /// <summary>
        /// Event handler for job run end event.
        /// </summary>
        /// <param name="id">Job run ID.</param>
        void OnJobRunEnded(long id);
    }
}