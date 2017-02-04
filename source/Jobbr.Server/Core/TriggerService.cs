using System;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    public class TriggerService
    {
        private readonly IJobbrRepository jobbrRepository;
        private readonly ITinyMessengerHub messengerHub;

        private static readonly ILog Logger = LogProvider.For<TriggerService>();

        public TriggerService(IJobbrRepository jobbrRepository, ITinyMessengerHub messengerHub)
        {
            this.jobbrRepository = jobbrRepository;
            this.messengerHub = messengerHub;
        }

        internal long Add(RecurringTriggerModel trigger)
        {
            var triggerEntity = new RecurringTrigger()
            {
                JobId = trigger.JobId,
                Comment = trigger.Comment,
                Definition = trigger.Definition,
                NoParallelExecution = trigger.NoParallelExecution,
                Parameters = trigger.Parameters,
                IsActive = trigger.IsActive,
                StartDateTimeUtc = trigger.StartDateTimeUtc,
                EndDateTimeUtc = trigger.EndDateTimeUtc,
                UserDisplayName = trigger.UserDisplayName,
                UserId = trigger.UserId,
                UserName = trigger.UserName
            };

            this.jobbrRepository.SaveAddTrigger(triggerEntity);
            trigger.Id = triggerEntity.Id;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, triggerEntity.Id));

            return trigger.Id;
        }

        public void Update(long triggerId, string definition)
        {
            var trigger = this.jobbrRepository.GetTriggerById(triggerId);

            var recurringTrigger = trigger as RecurringTrigger;

            if (recurringTrigger == null)
            {
                Logger.Warn($"Unable to update trigger with id '{triggerId}': Trigger not found!");
                return;
            }

            recurringTrigger.Definition = definition;

            this.jobbrRepository.SaveUpdateTrigger(triggerId, trigger, out bool hadChanges);

            if (hadChanges)
            {
                this.messengerHub.PublishAsync(new TriggerUpdatedMessage(this, triggerId));
            }
        }
    }

    public class TriggerUpdatedMessage : GenericTinyMessage<long>
    {
        public TriggerUpdatedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }

    public class TriggerStateChangedMessage : GenericTinyMessage<long>
    {
        public TriggerStateChangedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }

    internal class TriggerAddedMessage : GenericTinyMessage<long>
    {
        public TriggerAddedMessage(object sender, long content)
            : base(sender, content)
        {
        }

        public long TriggerId => this.Content;
    }

    internal class RecurringTriggerModel
        {
            public DateTime? StartDateTimeUtc { get; set; }

            public DateTime? EndDateTimeUtc { get; set; }

            public string Definition { get; set; }

            public string Comment { get; set; }

            public bool NoParallelExecution { get; set; }

            public long JobId { get; set; }

            public string Parameters { get; set; }

            public bool IsActive { get; set; }

            public string UserDisplayName { get; set; }

            public long? UserId { get; set; }

            public string UserName { get; set; }

            public long Id { get; set; }
        }
    
}
