using System;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Logging;
using Jobbr.Server.Storage;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    internal class TriggerService
    {
        private readonly IJobbrRepository jobbrRepository;
        private readonly ITinyMessengerHub messengerHub;
        private readonly IMapper mapper;

        private static readonly ILog Logger = LogProvider.For<TriggerService>();

        public TriggerService(IJobbrRepository jobbrRepository, ITinyMessengerHub messengerHub, IMapper mapper)
        {
            this.jobbrRepository = jobbrRepository;
            this.messengerHub = messengerHub;
            this.mapper = mapper;
        }

        internal long Add(RecurringTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<RecurringTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(triggerEntity);
            trigger.Id = triggerEntity.Id;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, triggerEntity.Id));

            return trigger.Id;
        }

        internal long Add(ScheduledTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<ScheduledTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(triggerEntity);
            trigger.Id = triggerEntity.Id;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, triggerEntity.Id));

            return trigger.Id;
        }

        internal long Add(InstantTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<InstantTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(triggerEntity);
            trigger.Id = triggerEntity.Id;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, triggerEntity.Id));

            return trigger.Id;
        }

        internal bool Disable(long triggerId)
        {
            if (this.jobbrRepository.DisableTrigger(triggerId))
            {
                this.messengerHub.PublishAsync(new TriggerStateChangedMessage(this, triggerId));
                return true;
            }

            return false;
        }

        internal bool Enable(long triggerId)
        {
            if (this.jobbrRepository.EnableTrigger(triggerId))
            {
                this.messengerHub.PublishAsync(new TriggerStateChangedMessage(this, triggerId));
                return true;
            }

            return false;
        }

        internal void Update(long triggerId, string definition)
        {
            var trigger = this.jobbrRepository.GetTriggerById(triggerId);

            var recurringTrigger = trigger as RecurringTrigger;

            if (recurringTrigger == null)
            {
                Logger.Warn($"Unable to update RecurringTrigger with id '{triggerId}': Trigger not found!");
                return;
            }

            recurringTrigger.Definition = definition;

            bool hadChanges = false;
            this.jobbrRepository.SaveUpdateTrigger(triggerId, trigger, out hadChanges);

            if (hadChanges)
            {
                this.messengerHub.PublishAsync(new TriggerUpdatedMessage(this, triggerId));
            }
        }

        internal void Update(long triggerId, DateTime startDateTimeUtc)
        {
            var trigger = this.jobbrRepository.GetTriggerById(triggerId);

            var recurringTrigger = trigger as ScheduledTrigger;

            if (recurringTrigger == null)
            {
                Logger.Warn($"Unable to update ScheduledTrigger with id '{triggerId}': Trigger not found!");
                return;
            }

            recurringTrigger.StartDateTimeUtc = startDateTimeUtc;

            var hadChanges = false;
            this.jobbrRepository.SaveUpdateTrigger(triggerId, trigger, out hadChanges);

            if (hadChanges)
            {
                this.messengerHub.PublishAsync(new TriggerUpdatedMessage(this, triggerId));
            }
        }
    }
}
