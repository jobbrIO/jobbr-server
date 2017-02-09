using System;
using Jobbr.Server.Scheduling;

namespace Jobbr.Tests.Components
{
    public class ManualTimeProvider : IDateTimeProvider
    {
        private DateTime currentTime;

        public ManualTimeProvider()
        {
            this.currentTime = DateTime.UtcNow;
        }

        public DateTime GetUtcNow()
        {
            return this.currentTime;
        }

        public void AddMinute()
        {
            this.currentTime = this.currentTime.AddMinutes(1);
        }

        public void Set(DateTime dateTimeUtc)
        {
            this.currentTime = dateTimeUtc;
        }
    }
}