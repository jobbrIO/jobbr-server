using Jobbr.Server.Scheduling;
using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class MessageDispatcher
    {
        private readonly IJobScheduler scheduler;

        public MessageDispatcher(ITinyMessengerHub messengerHub, IJobScheduler scheduler)
        {
            this.scheduler = scheduler;

            messengerHub.Subscribe<TriggerAddedMessage>(m => this.scheduler.OnTriggerAdded(m.JobId, m.TriggerId));
            messengerHub.Subscribe<TriggerUpdatedMessage>(m => this.scheduler.OnTriggerDefinitionUpdated(m.JobId, m.TriggerId));
            messengerHub.Subscribe<TriggerStateChangedMessage>(m => this.scheduler.OnTriggerStateUpdated(m.JobId, m.TriggerId));

            messengerHub.Subscribe<JobRunCompletedMessage>(m => this.scheduler.OnJobRunEnded(m.Id));
        }
    }
}