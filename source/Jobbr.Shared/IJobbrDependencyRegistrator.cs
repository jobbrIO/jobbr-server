namespace Jobbr.Shared
{
    public interface IJobbrDependencyRegistrator : IJobbrDependencyResolver
    {
        void RegisterInstance<T>(T instance);
    }
}