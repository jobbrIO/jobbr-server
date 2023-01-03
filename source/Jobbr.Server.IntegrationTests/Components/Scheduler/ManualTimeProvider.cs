using System;
using Jobbr.Server.Scheduling;

namespace Jobbr.Server.IntegrationTests.Components.Scheduler
{
    public class ManualTimeProvider : IDateTimeProvider
    {
        private DateTime currentTime;

        public ManualTimeProvider()
        {
            currentTime = DateTime.UtcNow;
        }

        public DateTime GetUtcNow()
        {
            return currentTime;
        }

        public void AddMinute()
        {
            currentTime = currentTime.AddMinutes(1);
        }

        public void AddSecond()
        {
            currentTime = currentTime.AddSeconds(1);
        }

        public void Set(DateTime dateTimeUtc)
        {
            currentTime = dateTimeUtc;
        }
    }
}