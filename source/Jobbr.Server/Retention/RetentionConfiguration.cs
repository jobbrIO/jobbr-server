using Jobbr.ComponentModel.Registration;

namespace Jobbr.Server.Retention
{
    public class RetentionConfiguration : IFeatureConfiguration
    {
        public int RetentionInDays { get; set; } = 30;
    }
}