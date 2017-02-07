using System;
using AutoMapper;
using Ninject.Activation;

namespace Jobbr.Server.Builder
{
    internal class AutoMapperProvider : IProvider
    {
        private readonly MapperConfiguration mapperConfiguration;

        public AutoMapperProvider(MapperConfiguration mapperConfiguration)
        {
            this.mapperConfiguration = mapperConfiguration;
        }

        public Type Type => typeof(IMapper);

        public object Create(IContext context)
        {
            return this.mapperConfiguration.CreateMapper();
        }
    }
}