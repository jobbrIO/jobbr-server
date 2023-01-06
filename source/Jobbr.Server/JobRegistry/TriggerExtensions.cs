using System;
using Jobbr.ComponentModel.JobStorage.Model;

namespace Jobbr.Server.JobRegistry
{
    /// <summary>
    /// Extension methods for job triggers.
    /// </summary>
    public static class TriggerExtensions
    {
        /// <summary>
        /// Checks if two trigger instances are actually the same instance.
        /// </summary>
        /// <param name="trigger">Target trigger.</param>
        /// <param name="other">Comparison trigger.</param>
        /// <returns>Returns true if the instances are same and false if not.</returns>
        public static bool IsTriggerEqual(this JobTriggerBase trigger, JobTriggerBase other)
        {
            if (ReferenceEquals(trigger, other))
            {
                return true;
            }

            if (trigger == null && other != null)
            {
                return false;
            }

            if (trigger != null && other == null)
            {
                return false;
            }

            if (trigger.GetType() != other.GetType())
            {
                return false;
            }

            return IsSame(trigger, other);
        }

        private static bool IsSame(JobTriggerBase left, JobTriggerBase right)
        {
            var parametersEqual = string.Equals(left.Parameters, right.Parameters, StringComparison.OrdinalIgnoreCase);
            var userDisplayNameEqual = string.Equals(left.UserDisplayName, right.UserDisplayName, StringComparison.OrdinalIgnoreCase);
            var userIdEqual = string.Equals(left.UserId, right.UserId, StringComparison.OrdinalIgnoreCase);

            var baseEqual = parametersEqual && userDisplayNameEqual && userIdEqual;
            if (!baseEqual)
            {
                return false;
            }

            // Types (left and right) are equal here
            switch (left)
            {
                case RecurringTrigger leftRecurringTrigger:
                    return IsSame(leftRecurringTrigger, (RecurringTrigger)right);
                case ScheduledTrigger leftSchedulingTrigger:
                    return IsSame(leftSchedulingTrigger, (ScheduledTrigger)right);
                case InstantTrigger leftInstantTrigger:
                    return IsSame(leftInstantTrigger, (InstantTrigger)right);
                default:
                    return false;
            }
        }

        private static bool IsSame(ScheduledTrigger left, ScheduledTrigger right)
        {
            return left.StartDateTimeUtc == right.StartDateTimeUtc;
        }

        private static bool IsSame(InstantTrigger left, InstantTrigger right)
        {
            return left.DelayedMinutes == right.DelayedMinutes;
        }

        private static bool IsSame(RecurringTrigger left, RecurringTrigger right)
        {
            var definitionEqual = string.Equals(left.Definition, right.Definition, StringComparison.OrdinalIgnoreCase);
            var startDateEqual = left.StartDateTimeUtc == right.StartDateTimeUtc;
            var endDateEqual = left.EndDateTimeUtc == right.EndDateTimeUtc;
            var parallelExecutionEqual = left.NoParallelExecution == right.NoParallelExecution;

            return definitionEqual && startDateEqual && endDateEqual && parallelExecutionEqual;
        }
    }
}