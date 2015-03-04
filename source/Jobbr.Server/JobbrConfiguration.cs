namespace Jobbr.Server
{
    public class JobbrConfiguration
    {
        public IJobQueueProvider JobQueueProvider;

        public IJobRepositoryProvider JobRepositoryProvider;
    }
}