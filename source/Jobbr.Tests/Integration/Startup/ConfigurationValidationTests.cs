using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Startup
{
    [TestClass]
    public class ConfigurationValidationTests
    {
        public class DemoSettings
        {
        }

        public class DemoComponentValidator : IConfigurationValidator
        {
            private readonly Action func;
            private readonly bool validationShouldFail;
            private readonly bool throwExecption;

            public Type ConfigurationType { get; set; } = typeof(DemoSettings);

            public DemoComponentValidator(bool validationShouldFail, bool throwExecption)
            {
                this.validationShouldFail = validationShouldFail;
                this.throwExecption = throwExecption;
            }

            public DemoComponentValidator(Action func)
            {
                this.func = func;
            }

            public bool Validate(object configuration)
            {
                if (this.throwExecption)
                {
                    throw new Exception("Exception from here");
                }

                this.func?.Invoke();

                return !this.validationShouldFail;
            }
        }

        [TestMethod]
        public void ValidatorForSettings_WhenRegistered_IsCalled()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());
            var isCalled = false;

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(() => isCalled = true));

            var jobbr = builder.Create();

            jobbr.Start();

            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ValidationFails_PreventsStart()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: true, throwExecption: false));

            var jobbr = builder.Create();

            jobbr.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ThrowsException_PreventsStart()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: false, throwExecption: true));

            var jobbr = builder.Create();

            jobbr.Start();
        }
    }
}
