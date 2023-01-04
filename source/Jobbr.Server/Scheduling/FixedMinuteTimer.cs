using System;
using System.Threading;

namespace Jobbr.Server.Scheduling
{
    internal class FixedMinuteTimer : IPeriodicTimer, IDisposable
    {
        private Timer _timer;

        private Action callback;

        public FixedMinuteTimer()
        {
            _timer = new Timer(state => callback());
        }

        public void Setup(Action value)
        {
            callback = value;
        }

        public void Start()
        {
            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        public void Stop()
        {
            _timer.Change(int.MaxValue, int.MaxValue);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}