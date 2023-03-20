using System;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// DateTime provider.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Get's current time in UTC.
        /// </summary>
        /// <returns>Current time in UTC.</returns>
        DateTime GetUtcNow();
    }
}