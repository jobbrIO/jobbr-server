using System;
using System.Threading;
using Jobbr.ComponentModel.JobStorage;
using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Retention
{
    public class RetentionComponent : IJobbrComponent
    {
        private Timer timer;

        public RetentionComponent(IJobStorageProvider storageProvider, RetentionConfiguration retentionConfiguration)
        {
            this.timer = new Timer(state =>
            {
                var date = DateTimeOffset.UtcNow.AddDays(-retentionConfiguration.RetentionInDays);
                storageProvider.ApplyRetention(date);
            });
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Start()
        {
            this.timer.Change(TimeSpan.Zero, TimeSpan.FromDays(1));
        }

        public void Stop()
        {
            this.timer.Change(int.MaxValue, int.MaxValue);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                if (this.timer != null)
                {
                    this.timer.Dispose();
                    this.timer = null;
                }
            }
        }
    }
}