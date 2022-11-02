using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Tests.Integration.Startup
{
    [TestClass]
    public class ConfigurationValidationTests
    {
        public class DemoComponent : IJobbrComponent
        {
            public void Dispose()
            {
            }

            public void Start()
            {
            }

            public void Stop()
            {
            }
        }

        public class DemoSettings
        {
        }

        public class DemoComponentValidator : IConfigurationValidator
        {
            private readonly Action<DemoSettings> func;
            private readonly bool validationShouldFail;
            private readonly bool throwException;

            public Type ConfigurationType { get; set; } = typeof(DemoSettings);

            public DemoComponentValidator(bool validationShouldFail, bool throwException)
            {
                this.validationShouldFail = validationShouldFail;
                this.throwException = throwException;
            }

            public DemoComponentValidator(Action<DemoSettings> func)
            {
                this.func = func;
            }

            public bool Validate(object configuration)
            {
                if (this.throwException)
                {
                    throw new Exception("Exception from here");
                }

                this.func?.Invoke((DemoSettings)configuration);

                return !this.validationShouldFail;
            }
        }

        [TestMethod]
        public void ValidatorForSettings_WhenRegistered_IsCalled()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());
            var isCalled = false;

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(s => isCalled = true));

            var jobbr = builder.Create();

            jobbr.Start();

            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ValidatorForSettings_WhenCalled_SettingsIsPassedThrough()
        {
            var builder = new JobbrBuilder();

            var demoSettings = new DemoSettings();
            object settingsToValidate = null;

            builder.Add<DemoSettings>(demoSettings);
            
            builder.Add<IConfigurationValidator>(new DemoComponentValidator(s => settingsToValidate = s));

            var jobbr = builder.Create();

            jobbr.Start();

            Assert.IsNotNull(settingsToValidate);
            Assert.AreSame(demoSettings, settingsToValidate);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ValidationFails_PreventsStart()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: true, throwException: false));

            var jobbr = builder.Create();

            jobbr.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ThrowsException_PreventsStart()
        {
            var builder = new JobbrBuilder();

            builder.Add<DemoSettings>(new DemoSettings());

            builder.Add<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: false, throwException: true));

            var jobbr = builder.Create();

            jobbr.Start();
        }
    }
}
