using System.ComponentModel;
using System.Data.Common;
using System.IO;

using Jobbr.Server.Common;
using Jobbr.Server.Model;

namespace Jobbr.Server.Core
{
    /// <summary>
    /// The runner process.
    /// </summary>
    internal class RunnerProcess
    {
        private readonly IJobService jobService;

        private readonly IJobbrConfiguration configuration;

        private JobRun jobRun;

        public RunnerProcess(IJobService jobService, IJobbrConfiguration configuration)
        {
            this.jobService = jobService;
            this.configuration = configuration;
        }

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

            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = true;
            proc.StartInfo.FileName = runnerFileExe;
#if DEBUG            
            proc.StartInfo.Arguments = "--debug --jobRunId " + jobRun.Id;
#else
            proc.StartInfo.Arguments = "--jobRunId " + jobRun.Id;
#endif            
            proc.StartInfo.WorkingDirectory = workDir;

            this.jobService.UpdateJobRunState(jobRun, JobRunState.Starting);

            proc.Start();

            this.jobService.SetPidForJobRun(jobRun, proc.Id);

        }
    }
}