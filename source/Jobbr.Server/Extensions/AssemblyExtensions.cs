using System;
using System.Reflection;

namespace Jobbr.Server.Extensions
{
    public static class AssemblyExtensions
    {
        public static string GetVersion(this Assembly assembly)
        {
            var versionAttribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            if (versionAttribute == null)
            {
                return "Unknown Version -> missing AssemblyInformationalVersionAttribute";
            }

            return versionAttribute.InformationalVersion;
        }
    }
}
