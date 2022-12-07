namespace Jobbr.Server.Core.Models
{
    /// <summary>
    /// Model for an instant trigger.
    /// </summary>
    public class InstantTriggerModel : TriggerModelBase
    {
        /// <summary>
        /// Delay in minutes.
        /// </summary>
        public int DelayedMinutes { get; set; }
    }
}