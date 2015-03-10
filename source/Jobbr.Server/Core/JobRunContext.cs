using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Model;
using Jobbr.Server.ServiceMessaging;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The runner process.
    /// </summary>
    internal class JobRunContext
    {
        /// <summary>
        /// The job service.
        /// </summary>
        private readonly IJobService jobService;

        /// <summary>
        /// The configuration.
        /// </summary>
        private readonly IJobbrConfiguration configuration;

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
        public JobRunContext(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;
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
            this.jobRun = jobRun;

            // Create the WorkingDir and TempDir for the execution
            var jobRunPath = Path.Combine(this.configuration.JobRunDirectory, jobRun.UniqueId);

            var tempDir = Path.Combine(jobRunPath, "temp");
            var workDir = Path.Combine(jobRunPath, "work");

            Directory.CreateDirectory(tempDir);
            Directory.CreateDirectory(workDir);

            this.jobService.UpdateJobRunDirectories(this.jobRun, workDir, tempDir);

            var runnerFileExe = Path.GetFullPath(this.configuration.JobRunnerExeResolver());

            Process proc = new Process();
            proc.EnableRaisingEvents = true;
            proc.StartInfo.FileName = runnerFileExe;

            var arguments = string.Format("--jobRunId {0} --server {1}", jobRun.Id, this.configuration.BackendAddress);

            if (this.configuration.BeChatty)
            {
                arguments += " --chatty";
            }

#if DEBUG
            arguments += " --debug";
#endif
            proc.StartInfo.Arguments = arguments;
            proc.StartInfo.WorkingDirectory = workDir;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.CreateNoWindow = true;

            proc.OutputDataReceived += ProcOnOutputDataReceived;

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Starting);

            proc.Exited += (o, args) => this.OnEnded(new JobRunEndedEventArgs() { ExitCode = proc.ExitCode, JobRun = jobRun });

            proc.Start();
            proc.BeginOutputReadLine();

            this.jobService.SetPidForJobRun(jobRun, proc.Id);
            this.jobService.SetJobRunStartTime(jobRun, DateTime.UtcNow);

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Started);
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

                    this.HandleMessage(message as dynamic);

                    // TODO: Log
                }
            }
        }

        private void HandleMessage(ProgressServiceMessage message)
        {
            this.jobRun.Progress = message.Percent;
            this.jobService.UpdateJobRunProgress(this.jobRun, message.Percent);
        }

        protected virtual void OnEnded(JobRunEndedEventArgs e)
        {
            var handler = this.Ended;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}