using System;
using Shouldly;

namespace Jobbr.Server.UnitTests
{
    public static class ShouldDateTimeExtensions
    {
        private static readonly TimeSpan DefaultOffsetTolerance = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Compare <paramref name="actual"/> to <see cref="DateTime.UtcNow"/> for equality with a tolerance of one second or throw a <see cref="ShouldAssertException"/>.
        /// </summary>
        /// <param name="actual">The value to check against.</param>
        /// <param name="customMessage">Custom error message to display when an error occurs.</param>
        public static DateTime ShouldBeUtcNowWithTolerance(this DateTime actual, string customMessage = null)
        {
            return actual.ShouldBeUtcNowWithTolerance(DefaultOffsetTolerance, customMessage);
        }

        /// <summary>
        /// Compares the actual <paramref name="actual"/> with <see cref="DateTime.UtcNow"/> for equality within the given <paramref name="tolerance"/> or throw a <see cref="ShouldAssertException"/>.
        /// </summary>
        /// <param name="actual">The value to check against.</param>
        /// <param name="tolerance">The allowed tolerance for the equality to be checked.</param>
        /// <param name="customMessage">Custom error message to display when an error occurs.</param>
        public static DateTime ShouldBeUtcNowWithTolerance(this DateTime actual, TimeSpan tolerance, string customMessage = null)
        {
            var offset = DateTime.UtcNow - actual;
            offset.ShouldBeLessThanOrEqualTo(tolerance, customMessage);
            return actual;
        }
    }
}
