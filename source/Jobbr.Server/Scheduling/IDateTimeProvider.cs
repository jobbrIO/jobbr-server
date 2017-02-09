using System;

namespace Jobbr.Server.Scheduling
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }
}