using System;
using Jobbr.Server.Core.Models;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Interface for a trigger management service.
    /// </summary>
    public interface ITriggerService
    {
        /// <summary>
        /// Adds a new recurring trigger to a job.
        /// </summary>
        /// <param name="jobId">ID of the job.</param>
        /// <param name="trigger">The model for the trigger.</param>
        void Add(long jobId, RecurringTriggerModel trigger);

        /// <summary>
        /// Adds a new scheduled trigger to a job.
        /// </summary>
        /// <param name="jobId">ID of the job.</param>
        /// <param name="trigger">The model for the trigger.</param>
        void Add(long jobId, ScheduledTriggerModel trigger);

        /// <summary>
        /// Adds a new instant trigger to a job.
        /// </summary>
        /// <param name="jobId">ID of the job.</param>
        /// <param name="trigger">The model for the trigger.</param>
        void Add(long jobId, InstantTriggerModel trigger);

        /// <summary>
        /// Disables a trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void Disable(long jobId, long triggerId);

        /// <summary>
        /// Deletes a trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void Delete(long jobId, long triggerId);

        /// <summary>
        /// Enables a trigger.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        void Enable(long jobId, long triggerId);

        /// <summary>
        /// Updates a trigger definition.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="definition">Trigger definition.</param>
        void Update(long jobId, long triggerId, string definition);

        /// <summary>
        /// Updates a recurring trigger.
        /// </summary>
        /// <param name="trigger">Trigger model used for the update.</param>
        void Update(RecurringTriggerModel trigger);

        /// <summary>
        /// Updates a scheduled trigger.
        /// </summary>
        /// <param name="trigger">Trigger model used for the update.</param>
        void Update(ScheduledTriggerModel trigger);

        /// <summary>
        /// Updates a trigger start time.
        /// </summary>
        /// <param name="jobId">Job ID.</param>
        /// <param name="triggerId">Trigger ID.</param>
        /// <param name="startDateTimeUtc">New start time in UTC.</param>
        void Update(long jobId, long triggerId, DateTime startDateTimeUtc);
    }
}