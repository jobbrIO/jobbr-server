using System;
using Shouldly;

namespace Jobbr.Server.UnitTests
{
    public static class ShouldDateTimeExtensions
    {
        private static readonly TimeSpan DefaultOffsetTolerance = TimeSpan.FromSeconds(1);

        public static DateTime ShouldBeUtcNow(this DateTime actual, string customMessage = null)
        {
            return actual.ShouldBeUtcNow(DefaultOffsetTolerance, customMessage);
        }

        public static DateTime ShouldBeUtcNow(this DateTime actual, TimeSpan tolerance, string customMessage = null)
        {
            var offset = DateTime.UtcNow - actual;
            offset.ShouldBeLessThanOrEqualTo(tolerance, customMessage);
            return actual;
        }
    }
}
