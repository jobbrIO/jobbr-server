using System;
using System.Threading;

namespace Jobbr.Server.Scheduling
{
    internal class FixedMinuteTimer : IPeriodicTimer, IDisposable
    {
        private Timer timer;

        private Action callback;

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

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }
    }
}