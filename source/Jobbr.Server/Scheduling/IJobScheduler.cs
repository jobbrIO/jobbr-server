using System;

namespace Jobbr.Server.Scheduling
{
    public interface IJobScheduler : IDisposable
    {
        void Start();

        void Stop();

        void OnTriggerDefinitionUpdated(long jobId, long triggerId);

        void OnTriggerStateUpdated(long jobId, long triggerId);

        void OnTriggerAdded(long jobId, long triggerId);

        void OnJobRunEnded(long id);
    }
}