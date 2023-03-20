using System;
using System.Threading;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Fixed minute timer.
    /// </summary>
    internal class FixedMinuteTimer : IPeriodicTimer, IDisposable
    {
        private Timer _timer;
        private Action _callback;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedMinuteTimer"/> class.
        /// </summary>
        public FixedMinuteTimer()
        {
            _timer = new Timer(_ => _callback());
        }

        /// <inheritdoc/>
        public void Setup(Action value)
        {
            _callback = value;
        }

        /// <inheritdoc/>
        public void Start()
        {
            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
        }

        /// <inheritdoc/>
        public void Stop()
        {
            _timer.Change(int.MaxValue, int.MaxValue);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Conditional dispose.
        /// </summary>
        /// <param name="isDisposing">Dispose condition.</param>
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