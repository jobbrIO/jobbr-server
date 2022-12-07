using Jobbr.Server.Scheduling;
using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Sets up message subscriptions.
    /// </summary>
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly ITinyMessengerHub _messengerHub;
        private readonly IJobScheduler _scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageDispatcher"/> class.
        /// </summary>
        /// <param name="messengerHub">The TinyMessenger message hub.</param>
        /// <param name="scheduler">The job scheduler.</param>
        public MessageDispatcher(ITinyMessengerHub messengerHub, IJobScheduler scheduler)
        {
            _messengerHub = messengerHub;
            _scheduler = scheduler;
        }

        /// <inheritdoc/>
        public void WireUp()
        {
            _messengerHub.Subscribe<TriggerAddedMessage>(m => _scheduler.OnTriggerAdded(m.JobId, m.TriggerId));
            _messengerHub.Subscribe<TriggerUpdatedMessage>(m => _scheduler.OnTriggerDefinitionUpdated(m.JobId, m.TriggerId));
            _messengerHub.Subscribe<TriggerStateChangedMessage>(m => _scheduler.OnTriggerStateUpdated(m.JobId, m.TriggerId));
            _messengerHub.Subscribe<JobRunCompletedMessage>(m => _scheduler.OnJobRunEnded(m.Id));
        }
    }
}