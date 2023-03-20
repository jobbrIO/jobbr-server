using System.Reflection;

namespace Jobbr.Server.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Assembly"/>.
    /// </summary>
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get the version of the <see cref="Assembly"/>.
        /// </summary>
        /// <param name="assembly">Target <see cref="Assembly"/>.</param>
        /// <returns>Version.</returns>
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
