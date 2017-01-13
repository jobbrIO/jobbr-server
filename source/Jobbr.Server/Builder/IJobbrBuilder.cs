using System;

namespace Jobbr.Server.Builder
{
    public interface IJobbrBuilder
    {
        void Register<T>(Type type);
    }
}