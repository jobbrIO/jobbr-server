using System;
using Shouldly;

namespace Jobbr.Server.UnitTests
{
    public static class ShouldDateTimeExtensions
    {
        private static readonly TimeSpan DefaultOffsetTolerance = TimeSpan.FromSeconds(1);

        public static DateTime ShouldBeUtcNowWithTolerance(this DateTime actual, string customMessage = null)
        {
            return actual.ShouldBeUtcNowWithTolerance(DefaultOffsetTolerance, customMessage);
        }

        public static DateTime ShouldBeUtcNowWithTolerance(this DateTime actual, TimeSpan tolerance, string customMessage = null)
        {
            var offset = DateTime.UtcNow - actual;
            offset.ShouldBeLessThanOrEqualTo(tolerance, customMessage);
            return actual;
        }
    }
}
