using System.Diagnostics;
using System.IO;

using Jobbr.Common;
using Jobbr.Server.Common;
using Jobbr.Server.Model;

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

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Starting);

            proc.Start();

            this.jobService.SetPidForJobRun(jobRun, proc.Id);

        }
    }
}