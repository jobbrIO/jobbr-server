namespace Jobbr.Server.ServiceMessaging
{
    /// <summary>
    /// The service message.
    /// </summary>
    public abstract class ServiceMessage
    {
    }

    /// <summary>
    /// The progress service message.
    /// </summary>
    public class ProgressServiceMessage : ServiceMessage
    {
        /// <summary>
        /// Gets or sets the percent.
        /// </summary>
        public double Percent { get; set; }
    }
}