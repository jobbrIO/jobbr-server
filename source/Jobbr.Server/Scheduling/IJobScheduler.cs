using System;

namespace Jobbr.Server.Scheduling
{
    public interface IJobScheduler : IDisposable
    {
        void Start();

        void Stop();
    }
}