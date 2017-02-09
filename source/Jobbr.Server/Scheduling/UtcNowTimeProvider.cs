using System;

namespace Jobbr.Server.Scheduling
{
    internal class UtcNowTimeProvider : IDateTimeProvider
    {
        DateTime IDateTimeProvider.GetUtcNow() => DateTime.UtcNow;
    }
}