using System;

namespace Jobbr.Server.Scheduling
{
    public interface IPeriodicTimer
    {
        void Setup(Action value);

        void Start();

        void Stop();
    }
}