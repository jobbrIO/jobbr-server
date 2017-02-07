using System;

namespace Jobbr.Server.Scheduling
{
    public interface IJobScheduler : IDisposable
    {
        void Start();

        void Stop();

        void OnTriggerDefinitionUpdated(long triggerId);

        void OnTriggerStateUpdated(long triggerId);

        void OnTriggerAdded(long triggerId);

        void OnJobRunEnded(Guid uniqueId);
    }
}