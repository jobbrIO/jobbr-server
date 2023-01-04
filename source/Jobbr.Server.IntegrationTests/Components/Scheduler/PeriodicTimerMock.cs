using System;
using Jobbr.Server.Scheduling;

namespace Jobbr.Server.IntegrationTests.Components.Scheduler
{
    public class PeriodicTimerMock : IPeriodicTimer
    {
        private Action _callback;

        public void Setup(Action value)
        {
            _callback = value;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void CallbackOnce()
        {
            _callback();
        }
    }
}