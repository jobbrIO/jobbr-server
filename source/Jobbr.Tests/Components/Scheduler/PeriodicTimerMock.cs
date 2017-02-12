using System;
using Jobbr.Server.Scheduling;

namespace Jobbr.Tests.Components.Scheduler
{
    public class PeriodicTimerMock : IPeriodicTimer
    {
        private Action callback;

        public void Setup(Action value)
        {
            this.callback = value;
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void CallbackOnce()
        {
            this.callback();
        }
    }
}