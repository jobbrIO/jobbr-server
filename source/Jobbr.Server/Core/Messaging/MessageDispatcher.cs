using Jobbr.Server.Scheduling;
using TinyMessenger;

namespace Jobbr.Server.Core.Messaging
{
    public class MessageDispatcher
    {
        private readonly ITinyMessengerHub messengerHub;
        private readonly IJobScheduler scheduler;

        public MessageDispatcher(ITinyMessengerHub messengerHub, IJobScheduler scheduler)
        {
            this.messengerHub = messengerHub;
            this.scheduler = scheduler;
        }

        public void WireuUp()
        {
            this.messengerHub.Subscribe<TriggerAddedMessage>(m => this.scheduler.OnTriggerAdded(m.JobId, m.TriggerId));
            this.messengerHub.Subscribe<TriggerUpdatedMessage>(m => this.scheduler.OnTriggerDefinitionUpdated(m.JobId, m.TriggerId));
            this.messengerHub.Subscribe<TriggerStateChangedMessage>(m => this.scheduler.OnTriggerStateUpdated(m.JobId, m.TriggerId));

            this.messengerHub.Subscribe<JobRunCompletedMessage>(m => this.scheduler.OnJobRunEnded(m.Id));
        }
    }
}