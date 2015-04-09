using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;
using Jobbr.Common.Model;
using Jobbr.Runtime.Logging;
using Jobbr.Shared;

using Newtonsoft.Json;

namespace Jobbr.Runtime
{
    /// <summary>
    /// The jobbr runtime.
    /// </summary>
    public class JobbrRuntime
    {
        private static readonly ILog Logger = LogProvider.For<JobbrRuntime>();
        
        private readonly Assembly defaultAssembly;

        private readonly IJobbrDependencyResolver dependencyResolver;

        private JobbrRuntimeClient client;

        private CommandlineOptions commandlineOptions;

        private object jobInstance;

        private CancellationTokenSource cancellationTokenSource;

        private Task jobRunTask;

        private JobRunInfoDto jobInfo;

        public JobbrRuntime(Assembly defaultAssembly, IJobbrDependencyResolver dependencyResolver)
        {
            this.defaultAssembly = defaultAssembly;
            this.dependencyResolver = dependencyResolver;
        }

        public JobbrRuntime(Assembly defaultAssembly)
        {
            this.defaultAssembly = defaultAssembly;
            this.dependencyResolver = new NoDependencyResolver();
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public void Run(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;

            Logger.InfoFormat("JobbrRuntime started at {0} (UTC) with cmd-arguments {1}", DateTime.UtcNow, string.Join(" ", args));

            this.ParseArguments(args);

            Logger.InfoFormat("JobRunId:  {0}", this.commandlineOptions.JobRunId);
            Logger.InfoFormat("JobServer: {0}", this.commandlineOptions.JobServer);
            Logger.InfoFormat("IsDebug:   {0}", this.commandlineOptions.IsDebug);

            this.InitializeClient();
            
            try
            {
                this.WaitForDebuggerIfEnabled();

                this.InitializeJob();

                this.StartJob();

                this.WaitForCompletion();

                this.Collect();
            }
            catch (Exception e)
            {
                Logger.FatalException("Exception in the Jobbr-Runtime. Please see details: ", e);
                Environment.ExitCode = 1;

                try
                {
                    this.Collect();
                }
                catch (Exception)
                {
                }
            }

            this.End();
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            Logger.FatalException("Unhandled Infrastructure Exception in Jobbr-Runtime. Please contact the developers!", (Exception)unhandledExceptionEventArgs.ExceptionObject);
        }

        private void WaitForCompletion()
        {
            if (this.jobRunTask != null)
            {
                try
                {
                    this.jobRunTask.Wait(this.cancellationTokenSource.Token);
                }
                catch (Exception e)
                {
                    Logger.ErrorException("Exception while waiting for completion of job", e);
                }

                if (this.jobRunTask.IsFaulted)
                {
                    Logger.ErrorException("The execution of the job has faulted. See Exception for details.", this.jobRunTask.Exception);
                    this.client.PublishState(JobRunState.Failed);
                }
                else
                {
                    this.client.PublishState(JobRunState.Finishing);
                }
            }
        }

        private void End()
        {
            if (this.jobRunTask == null || this.jobRunTask.IsFaulted)
            {
                this.client.PublishState(JobRunState.Failed);
            }
            else
            {
                this.client.PublishState(JobRunState.Completed);
            }
        }

        private void Collect()
        {
            if (this.jobRunTask != null)
            {
                this.client.PublishState(JobRunState.Collecting);

                var allFiles = Directory.GetFiles(this.jobInfo.WorkingDir);

                this.client.SendFiles(allFiles);
            }
        }

        private void StartJob()
        {
            if (this.jobInstance == null)
            {
                return;
            }

            var runMethods = this.jobInstance.GetType().GetMethods().Where(m => m.Name == "Run" && m.IsPublic).ToList();

            this.cancellationTokenSource = new CancellationTokenSource();

            if (runMethods.Any())
            {
                // Try to use the method with 2 concrete parameters
                var parameterizedMethod = runMethods.FirstOrDefault(m => m.GetParameters().Count() == 2);
                if (parameterizedMethod != null)
                {
                    Logger.DebugFormat("Decided to use parameterized method '{0}' with JobParameter '{1}' and InstanceParameters '{2}'.", parameterizedMethod, this.jobInfo.JobParameter ?? "<null>", this.jobInfo.InstanceParameter ?? "<null>");
                    var allParams = parameterizedMethod.GetParameters().OrderBy(p => p.Position).ToList();

                    var param1Type = allParams[0].ParameterType;
                    var param2Type = allParams[1].ParameterType;

                    var param1Name = allParams[0].Name;
                    var param2Name = allParams[1].Name;

                    // Casting in the most preferrable type
                    var jobParameterValue = this.GetCastedParameterValue(param1Name, param1Type, "job", this.jobInfo.JobParameter);
                    var instanceParamaterValue = this.GetCastedParameterValue(param2Name, param2Type, "instance", this.jobInfo.InstanceParameter);

                    Logger.Debug("Initializing task for JobRun");
                    this.jobRunTask = new Task(() => { parameterizedMethod.Invoke(this.jobInstance, new[] { jobParameterValue, instanceParamaterValue }); }, this.cancellationTokenSource.Token);
                }
                else
                {
                    var fallBackMethod = runMethods.FirstOrDefault(m => !m.GetParameters().Any());

                    if (fallBackMethod != null)
                    {
                        Logger.DebugFormat("Decided to use parameterless method '{0}'", fallBackMethod);
                        this.jobRunTask = new Task(() => fallBackMethod.Invoke(this.jobInstance, null), this.cancellationTokenSource.Token);
                    }
                }

                if (this.jobRunTask != null)
                {
                    Logger.Debug("Starting Task to execute the Run()-Method.");
                    this.jobRunTask.Start();
                    this.client.PublishState(JobRunState.Processing);
                }
                else
                {
                    Logger.ErrorFormat("None of your Run()-Methods are compatible with Jobbr. Please see coeumentation");
                }
            }
            else
            {
                Logger.Error("Unable to find an entrypoint to call your job. Is there at least a public Run()-Method?");
                this.client.PublishState(JobRunState.Failed);
            }
        }

        private object GetCastedParameterValue(string parameterName, Type targetType, string jobbrParamName, object value)
        {
            object castedValue;

            Logger.InfoFormat("Casting {0}-parameter to its target value '{1}' based on the Run()-Parameter {2}", jobbrParamName, targetType, parameterName);

            // Try to cast them to specific types
            if (value == null)
            {
                Logger.DebugFormat("The {0}-parameter is null - no cast needed.", jobbrParamName);
                castedValue = null;
            }
            else if (targetType == typeof(object))
            {
                Logger.DebugFormat("The {0}-parameter is of type 'object' - no cast needed.", jobbrParamName);
                castedValue = value;
            }
            else
            {
                Logger.DebugFormat("The {0}-parameter '{1}' is from type '{2}'. Casting this value to '{2}'", jobbrParamName, parameterName, targetType);
                castedValue = JsonConvert.DeserializeObject(value.ToString(), targetType);
            }

            return castedValue;
        }

        private void InitializeJob()
        {
            this.client.PublishState(JobRunState.Initializing);
            this.jobInfo = this.client.GetJobRunInfo();

            var typeName = this.jobInfo.JobType;

            Logger.DebugFormat("Trying to resolve the specified type '{0}'...", this.jobInfo.JobType);
            
            var type = this.ResolveType(typeName);

            if (type == null)
            {
                Logger.ErrorFormat("Unable to resolve the type '{0}'!", this.jobInfo.JobType);

                this.client.PublishState(JobRunState.Failed);
            }
            else
            {
                Logger.InfoFormat("Type '{0}' has been resolved to '{1}'. Activating now.", this.jobInfo.JobType, type);

                try
                {
                    this.jobInstance = this.dependencyResolver.GetService(type);
                }
                catch (Exception exception)
                {
                    Logger.ErrorException("Failed while activating type '{0}'. See Exception for details!", exception);
                }
            }
        }

        private Type ResolveType(string typeName)
        {
            Logger.DebugFormat("Resolve type using '{0}' like a full qualified CLR-Name", typeName);
            var type = Type.GetType(typeName);

            if (type == null && this.defaultAssembly != null)
            {
                Logger.DebugFormat("Trying to resolve '{0}' by the assembly '{1}'", typeName, this.defaultAssembly.FullName);
                type = this.defaultAssembly.GetType(typeName);
            }

            if (type == null)
            {
                // Search in all Assemblies
                var allReferenced = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

                Logger.DebugFormat("Trying to resolve type by asking all referenced assemblies ('{0}')", string.Join(", ", allReferenced.Select(a => a.Name)));

                foreach (var assemblyName in allReferenced)
                {
                    var assembly = Assembly.Load(assemblyName);

                    var foundType = assembly.GetType(typeName, false, true);

                    if (foundType != null)
                    {
                        type = foundType;
                    }
                }
            }

            if (type == null)
            {
                Logger.DebugFormat("Still no luck finding '{0}' somewhere. Iterating through all types and comparing class-names. Please hold on", typeName);

                // Absolutely no clue
                var matchingTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.Name == typeName && x.IsClass && !x.IsAbstract).ToList();

                if (matchingTypes.Count() == 1)
                {
                    Logger.DebugFormat("Found matching type: '{0}'", matchingTypes[0]);
                    type = matchingTypes.First();
                }
                else if (matchingTypes.Count() > 1)
                {
                    Logger.WarnFormat("More than one matching type found for '{0}'. Matches: ", typeName, string.Join(", ", matchingTypes.Select(t => t.FullName)));
                }
                else
                {
                    Logger.WarnFormat("No matching type found for '{0}'.", typeName);
                }
            }

            return type;
        }

        private void InitializeClient()
        {
            this.client = new JobbrRuntimeClient(this.commandlineOptions.JobServer, this.commandlineOptions.JobRunId);
            this.client.PublishState(JobRunState.Connected);
        }

        private void WaitForDebuggerIfEnabled()
        {
            if (this.commandlineOptions.IsDebug)
            {
                var beginWaitForDebugger = DateTime.Now;
                var endWaitForDebugger = beginWaitForDebugger.AddSeconds(10);
                var pressedEnter = false;

                Console.WriteLine(string.Empty);
                Console.WriteLine(">>> DEBUG-Mode is enabled. You have 10s to attach a Debugger");
                Console.Write("    or press enter to continue. Counting...");

                new TaskFactory().StartNew(
                    () =>
                        {
                            Console.ReadLine();
                            pressedEnter = true;
                        });

                while (!(pressedEnter || Debugger.IsAttached || endWaitForDebugger < DateTime.Now))
                {
                    Thread.Sleep(500);
                }
            }

            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }

        private void ParseArguments(string[] args)
        {
            this.commandlineOptions = new CommandlineOptions();
            CommandLine.Parser.Default.ParseArguments(args, this.commandlineOptions);
        }
    }
}
