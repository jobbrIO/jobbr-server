using System;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// Interface for periodic timers.
    /// </summary>
    public interface IPeriodicTimer
    {
        /// <summary>
        /// Setup timer.
        /// </summary>
        /// <param name="value">Timer event as <see cref="Action"/>.</param>
        void Setup(Action value);

        /// <summary>
        /// Start timer.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop timer.
        /// </summary>
        void Stop();
    }
}