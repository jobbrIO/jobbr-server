using System;
using Jobbr.ComponentModel.JobStorage.Model;
using Jobbr.Server.JobRegistry;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Registration
{
    [TestClass]
    public class TriggerExtensionsTests
    {
        [TestMethod]
        public void TriggerShouldBeTheSame_WhenReferenceIsEqual()
        {
            var trigger = new InstantTrigger();

            var isTriggerEqual = trigger.IsTriggerEqual(trigger);

            Assert.IsTrue(isTriggerEqual);
        }

        [TestMethod]
        public void TriggerShouldNotBeTheSame_WhenOnOfTheTriggersIsNull()
        {
            var trigger = new InstantTrigger();

            var isTriggerEqual = trigger.IsTriggerEqual(null);

            Assert.IsFalse(isTriggerEqual);

            isTriggerEqual = ((InstantTrigger) null).IsTriggerEqual(trigger);

            Assert.IsFalse(isTriggerEqual);
        }

        [TestMethod]
        public void TriggersShouldNotBeEqual_WhenDifferentTypes()
        {
            var firstTrigger = new InstantTrigger();
            var secondTrigger = new RecurringTrigger();

            var isTriggerEqual = firstTrigger.IsTriggerEqual(secondTrigger);

            Assert.IsFalse(isTriggerEqual);
        }

        [TestMethod]
        public void TriggersAreEqual_WhenBaseInformationAreEqualEvenIdIsDifferent()
        {
            const string userDisplayName = "Jobbr";
            const string parameters = "json";
            const string userId = "1";
            var firstTrigger = new InstantTrigger
            {
                Id = 1,
                JobId = 2,
                Comment = "Different",
                UserDisplayName = userDisplayName,
                Parameters = parameters,
                UserId = userId,
            };
            var secondTrigger = new InstantTrigger
            {
                Id = 2,
                JobId = 4,
                Comment = "Same same but different",
                Parameters = parameters,
                UserId = userId,
                UserDisplayName = userDisplayName
            };

            var isTriggerEqual = firstTrigger.IsTriggerEqual(secondTrigger);

            Assert.IsTrue(isTriggerEqual);
        }

        [TestMethod]
        public void TriggersAreNotEqual_WhenParameterIsDifferent()
        {
            var firstTrigger = new InstantTrigger { Parameters = "CAS9" };
            var secondTrigger = new InstantTrigger { Parameters = "CRISPR" };

            var isTriggerEqual = firstTrigger.IsTriggerEqual(secondTrigger);

            Assert.IsFalse(isTriggerEqual);
        }

        [TestMethod]
        public void InstantTriggerShouldBeEqual_WhenSameDelay()
        {
            const int delay = 100;
            const int otherDelay = 120;
            var firstTrigger = new InstantTrigger { DelayedMinutes = delay };
            var secondTrigger = new InstantTrigger {DelayedMinutes = delay};
            var thirdTrigger = new InstantTrigger {DelayedMinutes = otherDelay};

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }

        [TestMethod]
        public void ScheduledTriggerShouldBeEqual_WhenStartDateIsSame()
        {
            var startTime = new DateTime(2000, 1, 1);
            var anotherStarTime = new DateTime(1999, 12, 12);
            var firstTrigger = new ScheduledTrigger {StartDateTimeUtc = startTime};
            var secondTrigger = new ScheduledTrigger {StartDateTimeUtc = startTime};
            var thirdTrigger = new ScheduledTrigger {StartDateTimeUtc = anotherStarTime};

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }

        [TestMethod]
        public void RecurringTriggerShouldBeEqual_WhenDefinitionIsEqual()
        {
            const string definition = "0 * * * *";
            var firstTrigger = new RecurringTrigger {Definition = definition };
            var secondTrigger = new RecurringTrigger {Definition = definition };
            var thirdTrigger = new RecurringTrigger();

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }

        [TestMethod]
        public void RecurringTriggerShouldBeEqual_WhenStartDateIsEqual()
        {
            var date = new DateTime(1991, 10, 5);
            var firstTrigger = new RecurringTrigger { StartDateTimeUtc = date };
            var secondTrigger = new RecurringTrigger { StartDateTimeUtc = date };
            var thirdTrigger = new RecurringTrigger();

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }

        [TestMethod]
        public void RecurringTriggerShouldBeEqual_WhenEndDateIsEqual()
        {
            var date = new DateTime(1991, 10, 5);
            var firstTrigger = new RecurringTrigger { EndDateTimeUtc = date };
            var secondTrigger = new RecurringTrigger { EndDateTimeUtc = date };
            var thirdTrigger = new RecurringTrigger();

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }

        [TestMethod]
        public void RecurringTriggerShouldBeEqual_WhenNoParallelExecutionIsEqual()
        {
            const bool noParallel = false;
            var firstTrigger = new RecurringTrigger { NoParallelExecution = noParallel };
            var secondTrigger = new RecurringTrigger { NoParallelExecution = noParallel };
            var thirdTrigger = new RecurringTrigger { NoParallelExecution = !noParallel };

            var firstAndSecondEqual = firstTrigger.IsTriggerEqual(secondTrigger);
            var secondAndThirdEqual = secondTrigger.IsTriggerEqual(thirdTrigger);

            Assert.IsTrue(firstAndSecondEqual);
            Assert.IsFalse(secondAndThirdEqual);
        }
    }
}