using System;
using AutoMapper;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.Core.Messaging;
using Jobbr.Server.Core.Models;
using Jobbr.Server.Storage;
using Microsoft.Extensions.Logging;
using TinyMessenger;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// Service for managing triggers.
    /// </summary>
    internal class TriggerService
    {
        private readonly ILogger<TriggerService> _logger;
        private readonly IJobbrRepository _jobbrRepository;
        private readonly ITinyMessengerHub _messengerHub;
        private readonly IMapper _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="TriggerService"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="jobbrRepository">Repository for accessing job data,</param>
        /// <param name="messengerHub">SubPub messenger hub.</param>
        /// <param name="mapper">The mapper.</param>
        public TriggerService(ILogger<TriggerService> logger, IJobbrRepository jobbrRepository, ITinyMessengerHub messengerHub, IMapper mapper)
        {
            _logger = logger;
            _jobbrRepository = jobbrRepository;
            _messengerHub = messengerHub;
            _mapper = mapper;
        }

        internal void Add(long jobId, RecurringTriggerModel trigger)
        {
            var triggerEntity = _mapper.Map<RecurringTrigger>(trigger);

            _jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            _messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Add(long jobId, ScheduledTriggerModel trigger)
        {
            var triggerEntity = _mapper.Map<ScheduledTrigger>(trigger);

            _jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            _messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Add(long jobId, InstantTriggerModel trigger)
        {
            var triggerEntity = _mapper.Map<InstantTrigger>(trigger);

            _jobbrRepository.SaveAddTrigger(jobId, triggerEntity);
            trigger.Id = triggerEntity.Id;
            trigger.JobId = triggerEntity.JobId;

            _messengerHub.PublishAsync(new TriggerAddedMessage(this, new TriggerKey { JobId = triggerEntity.JobId, TriggerId = triggerEntity.Id }));
        }

        internal void Disable(long jobId, long triggerId)
        {
            _jobbrRepository.DisableTrigger(jobId, triggerId);
            _messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        internal void Delete(long jobId, long triggerId)
        {
            _jobbrRepository.DeleteTrigger(jobId, triggerId);
            _messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        internal void Enable(long jobId, long triggerId)
        {
            _jobbrRepository.EnableTrigger(jobId, triggerId);

            _messengerHub.PublishAsync(new TriggerStateChangedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
        }

        // TODO: combine update methods, too much copy-paste here
        internal void Update(long jobId, long triggerId, string definition)
        {
            var trigger = _jobbrRepository.GetTriggerById(jobId, triggerId);

            var recurringTrigger = trigger as RecurringTrigger;

            if (recurringTrigger == null)
            {
                _logger.LogWarning("Unable to update RecurringTrigger with id '{triggerId}': Trigger not found!", triggerId);
                return;
            }

            recurringTrigger.Definition = definition;

            _jobbrRepository.SaveUpdateTrigger(jobId, trigger, out var hadChanges);

            if (hadChanges)
            {
                _messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
            }
        }

        internal void Update(RecurringTriggerModel trigger)
        {
            var triggerEntity = _mapper.Map<RecurringTrigger>(trigger);

            // ReSharper disable once UsePatternMatching
            var fromDb = _jobbrRepository.GetTriggerById(trigger.JobId, trigger.Id) as RecurringTrigger;

            if (fromDb == null)
            {
                _logger.LogWarning("Unable to update RecurringTrigger with id '{triggerId}' (JobId '{jobId}'): Trigger not found!", trigger.Id, trigger.JobId);
                return;
            }

            _jobbrRepository.SaveUpdateTrigger(trigger.JobId, triggerEntity, out var hadChanges);

            if (hadChanges)
            {
                _messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = trigger.JobId, TriggerId = trigger.Id }));
            }
        }

        internal void Update(ScheduledTriggerModel trigger)
        {
            var triggerEntity = _mapper.Map<ScheduledTrigger>(trigger);

            // ReSharper disable once UsePatternMatching
            var fromDb = _jobbrRepository.GetTriggerById(trigger.JobId, trigger.Id) as ScheduledTrigger;

            if (fromDb == null)
            {
                _logger.LogWarning("Unable to update ScheduledTrigger with id '{triggerId}' (JobId '{jobId}'): Trigger not found!", trigger.Id, trigger.JobId);
                return;
            }

            _jobbrRepository.SaveUpdateTrigger(trigger.JobId, triggerEntity, out var hadChanges);

            if (hadChanges)
            {
                _messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = trigger.JobId, TriggerId = trigger.Id }));
            }
        }

        internal void Update(long jobId, long triggerId, DateTime startDateTimeUtc)
        {
            var trigger = _jobbrRepository.GetTriggerById(jobId, triggerId);

            // ReSharper disable once UsePatternMatching
            var recurringTrigger = trigger as ScheduledTrigger;

            if (recurringTrigger == null)
            {
                _logger.LogWarning("Unable to update ScheduledTrigger with id '{triggerId}': Trigger not found!", trigger.Id);
                return;
            }

            recurringTrigger.StartDateTimeUtc = startDateTimeUtc;

            _jobbrRepository.SaveUpdateTrigger(jobId, trigger, out var hadChanges);

            if (hadChanges)
            {
                _messengerHub.PublishAsync(new TriggerUpdatedMessage(this, new TriggerKey { JobId = jobId, TriggerId = triggerId }));
            }
        }
    }
}
