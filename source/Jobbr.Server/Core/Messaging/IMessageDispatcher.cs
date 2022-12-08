namespace Jobbr.Server.Core.Messaging
{
    /// <summary>
    /// Interface for message dispatchers.
    /// </summary>
    public interface IMessageDispatcher
    {
        /// <summary>
        /// Subscribers all the different message types.
        /// </summary>
        void WireUp();
    }
}