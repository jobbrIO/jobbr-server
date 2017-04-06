namespace Jobbr.Server.Core.Models
{
    public enum JobRunStates
    {
        Null = 0,
        Preparing = 1,
        Starting = 2,
        Started = 3,
        Connected = 4,
        Initializing = 5,
        Processing = 6,
        Finishing = 7,
        Collecting = 8,
        Completed = 9,
        Failed = 10
    }
}
