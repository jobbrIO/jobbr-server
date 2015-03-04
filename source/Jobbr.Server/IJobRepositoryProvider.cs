using System.Collections.Generic;

using Jobbr.Server.Model;

namespace Jobbr.Server
{
    public interface IJobRepositoryProvider
    {
        List<Job> GetJobs();

        long AddJob(Job job);

        List<JobTriggerBase> GetTriggers(long jobId);
    }
}