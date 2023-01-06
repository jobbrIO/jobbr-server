using System;

namespace Jobbr.Server.Scheduling
{
    /// <summary>
    /// UTC time provider.
    /// </summary>
    internal class UtcNowTimeProvider : IDateTimeProvider
    {
        /// <inheritdoc/>
        DateTime IDateTimeProvider.GetUtcNow() => DateTime.UtcNow;
    }
}