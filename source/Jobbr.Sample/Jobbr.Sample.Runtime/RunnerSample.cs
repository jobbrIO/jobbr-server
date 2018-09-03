using System;
using System.Diagnostics;
using Jobbr.Runtime;
using Jobbr.Runtime.ForkedExecution;
using Jobbr.Runtime.Logging;
using Jobbr.Sample.Jobs;

namespace Jobbr.Sample.Runner
{
    public class RunnerSample
    {
        public static void Main(string[] args)
        {
            // Assembly, where our jobs are stored
            var jobAssembly = typeof(InTimeClass).Assembly;

            LogProvider.SetCurrentLogProvider(new TraceLogProvider());

            var runtimeConfiguration = new RuntimeConfiguration {JobTypeSearchAssembly = jobAssembly};
            var runtime = new ForkedRuntime(runtimeConfiguration);

            runtime.Run(args);
        }

        public class TraceLogProvider : ILogProvider
        {
            public ILog GetLogger(string name)
            {
                return new TraceLogger(name);
            }

            public IDisposable OpenNestedContext(string message)
            {
                return default(IDisposable);
            }

            public IDisposable OpenMappedContext(string key, string value)
            {
                return default(IDisposable);
            }
        }

        public class TraceLogger : ILog
        {
            private readonly string _name;

            public TraceLogger(string name)
            {
                this._name = name;
            }

            public bool Log(LogLevel logLevel, Func<string> messageFunc, Exception exception = null, params object[] formatParameters)
            {
                if (messageFunc == null)
                {
                    return true;
                }

                Trace.WriteLine($"[{logLevel}] ({this._name}) {string.Format(messageFunc(), formatParameters)} {exception}");
                return true;
            }
        }
    }
}
