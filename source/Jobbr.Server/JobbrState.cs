namespace Jobbr.Server
{
    public enum JobbrState
    {
        Unknown = 0,
        Initializing = 11,
        Validating = 12,
        Starting = 13,
        Running = 14,

        Stopping = 21,
        Stopped = 29,
        Error = 99,
    }
}