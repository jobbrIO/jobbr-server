using System;
using System.Diagnostics;
using System.IO;
using Jobbr.Common.Model;
using Jobbr.Server.Common;
using Jobbr.Server.Logging;
using Jobbr.Server.Model;
using Jobbr.Server.ServiceMessaging;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The runner process.
    /// </summary>
    internal class JobRunContext
    {
        private static readonly ILog Logger = LogProvider.For<JobRunContext>();

        /// <summary>
        /// The job service.
        /// </summary>
        private readonly IJobService jobService;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IJobbrConfiguration configuration;

        private readonly IJobStorageProvider jobStorageProvider;

        private readonly ServiceMessageParser serviceMessageParser;

        public event EventHandler<JobRunEndedEventArgs> Ended;

        /// <summary>
        /// The job run.
        /// </summary>
        private JobRun jobRun;

        /// <summary>
        /// Initializes a new instance of the <see cref="JobRunContext"/> class.
        /// </summary>
        /// <param name="jobService">
        /// The job service.
        /// </param>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public JobRunContext(IJobService jobService, IJobbrConfiguration configuration, IJobStorageProvider jobStorageProvider)
        {
            this.jobService = jobService;
            this.configuration = configuration;
            this.jobStorageProvider = jobStorageProvider;
            this.serviceMessageParser = new ServiceMessageParser();
        }

        /// <summary>
        /// The start.
        /// </summary>
        /// <param name="jobRun">
        /// The job run.
        /// </param>
        public void Start(JobRun jobRun)
        {
            Logger.InfoFormat("[{0}] A new JobRunContext is starting for JobRun with id: '{1}'. (JobId: '{2}', TriggerId: '{3}'", jobRun.UniqueId, jobRun.Id, jobRun.JobId, jobRun.TriggerId);

            try
            {
                this.jobRun = jobRun;

                var workDir = this.SetupDirectories(jobRun);

                var proc = this.StartProcess(jobRun, workDir);

                this.UpdateState(jobRun, proc);
            }
            catch (Exception e)
            {
                Logger.ErrorException(string.Format("[{0}] Exception thrown while starting JobRun with id: '{1}'. (JobId: '{2}', TriggerId: '{3}'", jobRun.UniqueId, jobRun.Id, jobRun.JobId, jobRun.TriggerId), e);
            }
        }

        private void UpdateState(JobRun jobRun, Process proc)
        {
            this.jobService.SetPidForJobRun(jobRun, proc.Id);
            this.jobService.SetJobRunStartTime(jobRun, DateTime.UtcNow);

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Started);
        }

        private Process StartProcess(JobRun jobRun, string workDir)
        {
            var runnerFileExe = Path.GetFullPath(this.configuration.JobRunnerExeResolver());
            Logger.InfoFormat("[{0}] Preparing to start the runner from '{1}' in '{2}'", jobRun.UniqueId, runnerFileExe, workDir);

            var proc = new Process { EnableRaisingEvents = true, StartInfo = { FileName = runnerFileExe } };

            var arguments = $"--jobRunId {jobRun.Id} --server {this.configuration.BackendAddress}";

            if (this.configuration.IsRuntimeWaitingForDebugger)
            {
                arguments += " --debug";
            }

            if (this.configuration.CustomJobRunnerParameters != null)
            {
                var job = jobStorageProvider.GetJobById(jobRun.JobId);
                var customParameters = this.configuration.CustomJobRunnerParameters(job.Type, job.UniqueName);

                foreach (var customParameter in customParameters)
                {
                    if (customParameter.Value.Contains(" "))
                    {
                        arguments += $" --{customParameter.Key} \"{customParameter.Value}\"";
                    }
                    else
                    {
                        arguments += $" --{customParameter.Key} {customParameter.Value}";
                    }
                }
            }

            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.WorkingDirectory = workDir;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            // Wire events
            proc.OutputDataReceived += this.ProcOnOutputDataReceived;
            proc.Exited += (o, args) => this.OnEnded(new JobRunEndedEventArgs() { ExitCode = proc.ExitCode, JobRun = jobRun, ProcInfo = proc });

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Starting);

            Logger.InfoFormat("[{0}] Starting '{1} {2}' in '{3}'", jobRun.UniqueId, runnerFileExe, arguments, workDir);
            proc.Start();

            Logger.InfoFormat("[{0}] Started Runner with ProcessId '{1}' at '{2}'", jobRun.UniqueId, proc.Id, proc.StartTime);

            proc.BeginOutputReadLine();
            return proc;
        }

        private string SetupDirectories(JobRun jobRun)
        {
            // Create the WorkingDir and TempDir for the execution
            var jobRunPath = Path.Combine(this.configuration.JobRunDirectory, "jobbr-" + jobRun.UniqueId);

            Logger.InfoFormat("[{0}] Preparing filesytem directories in '{1}'", jobRun.UniqueId, this.configuration.JobRunDirectory);

            var tempDir = Path.Combine(jobRunPath, "temp");
            var workDir = Path.Combine(jobRunPath, "work");

            Directory.CreateDirectory(tempDir);
            Logger.InfoFormat("[{0}] Created Temp-Directory '{1}'", jobRun.UniqueId, tempDir);

            Directory.CreateDirectory(workDir);
            Logger.InfoFormat("[{0}] Created Working-Directory '{1}'", jobRun.UniqueId, workDir);

            this.jobService.UpdateJobRunDirectories(this.jobRun, workDir, tempDir);
            return workDir;
        }

        protected virtual void OnEnded(JobRunEndedEventArgs e)
        {
            Logger.InfoFormat("[{0}] The Runner with ProcessId '{1}' has ended at '{2}'. ExitCode: '{3}'", this.jobRun.UniqueId, e.ProcInfo.Id, e.ProcInfo.ExitTime, e.ProcInfo.ExitCode);

            var handler = this.Ended;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void ProcOnOutputDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            string data = dataReceivedEventArgs.Data;

            if (data == null)
            {
                return;
            }

            var lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                // Detect ServiceMessage
                if (line.StartsWith("##jobbr"))
                {
                    var message = this.serviceMessageParser.Parse(line);

                    try
                    {
                        if (message != null)
                        {
                            if (this.HandleMessage(message as dynamic))
                            {
                                Logger.DebugFormat("[{0}] Handled service-message of type '{1}'. RawValue: '{2}'", this.jobRun.UniqueId, message.GetType().Name, line);
                            }
                            else
                            {
                                // TODO: Implement this type!
                                Logger.WarnFormat("[{0}] Cannot handle messages of type '{1}'. RawValue: '{2}'", this.jobRun.UniqueId, message.GetType().Name, line);
                            }
                        }
                        else
                        {
                            Logger.WarnFormat("[{0}] Parsing Error while processing service message '{1}'", this.jobRun.UniqueId, line);
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.ErrorException(string.Format("[{0}] Exception while processing service message '{1}'", this.jobRun.UniqueId, line), e);
                    }
                }
            }
        }

        private bool HandleMessage(ProgressServiceMessage message)
        {
            this.jobRun.Progress = message.Percent;
            this.jobService.UpdateJobRunProgress(this.jobRun, message.Percent);

            return true;
        }
    }
}