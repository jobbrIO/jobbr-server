using System;
using System.Threading;

namespace Jobbr.Server.Scheduling
{
    internal class FixedMinuteTimer : IPeriodicTimer
    {
        private Action callback;

        private readonly Timer timer;

        public FixedMinuteTimer()
        {
            this.timer = new Timer(state => this.callback());
        }

        public void Setup(Action value)
        {
            this.callback = value;
        }

        public void Start()
        {
            this.timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            this.timer.Change(int.MaxValue, int.MaxValue);
        }
    }
}