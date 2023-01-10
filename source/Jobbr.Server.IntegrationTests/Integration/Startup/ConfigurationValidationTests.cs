using System;
using Jobbr.ComponentModel.Registration;
using Jobbr.Server.Builder;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jobbr.Server.IntegrationTests.Integration.Startup
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
            private readonly Action<DemoSettings> _func;
            private readonly bool _validationShouldFail;
            private readonly bool _throwException;

            public Type ConfigurationType { get; set; } = typeof(DemoSettings);

            public DemoComponentValidator(bool validationShouldFail, bool throwException)
            {
                _validationShouldFail = validationShouldFail;
                _throwException = throwException;
            }

            public DemoComponentValidator(Action<DemoSettings> func)
            {
                _func = func;
            }

            public bool Validate(object configuration)
            {
                if (_throwException)
                {
                    throw new Exception("Exception from here");
                }

                _func?.Invoke((DemoSettings)configuration);

                return !_validationShouldFail;
            }
        }

        [TestMethod]
        public void ValidatorForSettings_WhenRegistered_IsCalled()
        {
            var builder = new JobbrBuilder(NullLoggerFactory.Instance);

            builder.Add<DemoSettings>(new DemoSettings());
            var isCalled = false;

            builder.AddForCollection<IConfigurationValidator>(new DemoComponentValidator(s => isCalled = true));

            var jobbr = builder.Create();

            jobbr.Start();

            Assert.IsTrue(isCalled);
        }

        [TestMethod]
        public void ValidatorForSettings_WhenCalled_SettingsIsPassedThrough()
        {
            var builder = new JobbrBuilder(NullLoggerFactory.Instance);

            var demoSettings = new DemoSettings();
            object settingsToValidate = null;

            builder.Add<DemoSettings>(demoSettings);

            builder.AddForCollection<IConfigurationValidator>(new DemoComponentValidator(s => settingsToValidate = s));

            var jobbr = builder.Create();

            jobbr.Start();

            Assert.IsNotNull(settingsToValidate);
            Assert.AreSame(demoSettings, settingsToValidate);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ValidationFails_PreventsStart()
        {
            var builder = new JobbrBuilder(NullLoggerFactory.Instance);

            builder.Add<DemoSettings>(new DemoSettings());

            builder.AddForCollection<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: true, throwException: false));

            var jobbr = builder.Create();

            jobbr.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void ValidatorForSettings_ThrowsException_PreventsStart()
        {
            var builder = new JobbrBuilder(NullLoggerFactory.Instance);

            builder.Add<DemoSettings>(new DemoSettings());

            builder.AddForCollection<IConfigurationValidator>(new DemoComponentValidator(validationShouldFail: false, throwException: true));

            var jobbr = builder.Create();

            jobbr.Start();
        }
    }
}
