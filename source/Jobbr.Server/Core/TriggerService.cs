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
        private static readonly ILog Logger = LogProvider.For<TriggerService>();

        private readonly IJobbrRepository jobbrRepository;
        private readonly ITinyMessengerHub messengerHub;
        private readonly IMapper mapper;

        public TriggerService(IJobbrRepository jobbrRepository, ITinyMessengerHub messengerHub, IMapper mapper)
        {
            this.jobbrRepository = jobbrRepository;
            this.messengerHub = messengerHub;
            this.mapper = mapper;
        }

        internal void Add(long jobId, RecurringTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<RecurringTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Add(long jobId, ScheduledTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<ScheduledTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Add(long jobId, InstantTriggerModel trigger)
        {
            var triggerEntity = this.mapper.Map<InstantTrigger>(trigger);

            this.jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            this.messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Disable(long jobId, long triggerId)
        {
            this.jobbrRepository.DisableTrigger(jobId, triggerId);
            this.messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        internal void Delete(long jobId, long triggerId)
        {
            this.jobbrRepository.DeleteTrigger(jobId, triggerId);
            this.messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        internal void Enable(long jobId, long triggerId)
        {
            this.jobbrRepository.EnableTrigger(jobId, triggerId);

            this.messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        internal void Update(long jobId, long triggerId, string definition)
        {
            var trigger = this.jobbrRepository.GetTriggerById(jobId, triggerId);

            var recurringTrigger = trigger as RecurringTrigger;

            if (recurringTrigger == null)
            {
                Logger.Warn($"Unable to update RecurringTrigger with id '{triggerId}': Trigger not found!");
                return;
            }

            recurringTrigger.Definition = definition;

            bool hadChanges;
            this.jobbrRepository.SaveUpdateTrigger(jobId, trigger, out hadChanges);

            if (hadChanges)
            {
                this.messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
            }
        }

        internal void Update(long jobId, long triggerId, DateTime startDateTimeUtc)
        {
            var trigger = this.jobbrRepository.GetTriggerById(jobId, triggerId);

            var recurringTrigger = trigger as ScheduledTrigger;

            if (recurringTrigger == null)
            {
                Logger.Warn($"Unable to update ScheduledTrigger with id '{triggerId}': Trigger not found!");
                return;
            }

            recurringTrigger.StartDateTimeUtc = startDateTimeUtc;

            bool hadChanges;
            this.jobbrRepository.SaveUpdateTrigger(jobId, trigger, out hadChanges);

            if (hadChanges)
            {
                this.messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
            }
        }
    }
}
