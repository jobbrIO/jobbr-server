using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

using Jobbr.Common;
using Jobbr.Runtime.Logging;

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
            Logger.InfoFormat("JobbrRuntime is now running with the following parameters: '{0}'", string.Join(" ", args));

            this.ParseArguments(args);

            this.InitializeClient();
            
            try
            {
                this.DisplayWelcomeBannerIfEnabled(args);

                this.WaitForDebuggerIfEnabled();

                this.InitializeJob();

                this.StartJob();

                this.WaitForCompletion();

                this.Collect();
            }
            catch (Exception e)
            {
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

        private void WaitForCompletion()
        {
            if (this.jobRunTask != null)
            {
                this.jobRunTask.Wait(this.cancellationTokenSource.Token);

                if (this.jobRunTask.IsFaulted)
                {
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
            if (this.jobRunTask != null)
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
                var specificMethod = runMethods.FirstOrDefault(m => m.GetParameters().Count() == 2);
                if (specificMethod != null)
                {
                    // TODO: Log
                    var allParams = specificMethod.GetParameters().OrderBy(p => p.Position).ToList();

                    var param1Type = allParams[0].ParameterType;
                    var param2Type = allParams[1].ParameterType;

                    object param1Value = this.jobInfo.JobParameter;
                    object param2Value = this.jobInfo.InstanceParameter;

                    // Try to cast them to specific types
                    if (param1Type != typeof(object))
                    {
                        param1Value = JsonConvert.DeserializeObject(param1Value.ToString(), param1Type);
                    }

                    if (param2Type != typeof(object))
                    {
                        param2Value = JsonConvert.DeserializeObject(param2Value.ToString(), param2Type);
                    }

                    this.jobRunTask = new Task(() => { specificMethod.Invoke(this.jobInstance, new[] { param1Value, param2Value }); }, this.cancellationTokenSource.Token);
                }
                else
                {
                    var theRightOne = runMethods.First();

                    this.jobRunTask = new Task(() => theRightOne.Invoke(this.jobInstance, null), this.cancellationTokenSource.Token);
                }


                this.jobRunTask.Start();
                this.client.PublishState(JobRunState.Processing);
            }
            else
            {
                // TODO: Logging
                this.client.PublishState(JobRunState.Failed);
            }
        }

        private void InitializeJob()
        {
            this.client.PublishState(JobRunState.Initializing);
            this.jobInfo = this.client.GetJobRunInfo();

            if (this.jobInfo == null)
            {
                
            }

            var typeName = this.jobInfo.JobType;

            var type = this.ResolveType(typeName);

            if (type == null)
            {
                this.client.PublishState(JobRunState.Failed);
            }
            else
            {
                this.jobInstance = this.dependencyResolver.GetService(type);
            }
        }

        private Type ResolveType(string typeName)
        {
            var type = Type.GetType(typeName);

            if (type == null && this.defaultAssembly != null)
            {
                type = this.defaultAssembly.GetType(typeName);
            }

            if (type == null)
            {
                // Search in all Assemblies
                var allReferenced = Assembly.GetExecutingAssembly().GetReferencedAssemblies();

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
                // Absolutely no clue
                var matchingTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => x.Name == typeName && x.IsClass && !x.IsAbstract).ToList();

                if (matchingTypes.Count() == 1)
                {
                    type = matchingTypes.First();
                }
                else if (matchingTypes.Count() > 1)
                {
                    // TODO: Log
                }
                else
                {
                    // TODO: Log
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

        private void DisplayWelcomeBannerIfEnabled(string[] args)
        {
            if (this.commandlineOptions.IsChatty)
            {
                Console.Write(
                    "This is the runner started at " + DateTime.UtcNow + " (UTC) with arguments " + string.Join(" ", args));

                Console.WriteLine();
                Console.WriteLine("JobRunId:  " + this.commandlineOptions.JobRunId);
                Console.WriteLine("JobServer: " + this.commandlineOptions.JobServer);
                Console.WriteLine("IsDebug:   " + this.commandlineOptions.IsDebug);
            }
        }

        private void ParseArguments(string[] args)
        {
            this.commandlineOptions = new CommandlineOptions();
            CommandLine.Parser.Default.ParseArguments(args, this.commandlineOptions);
        }
    }
}
