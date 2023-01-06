using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jobbr.Server.Storage
{
    /// <summary>
    /// Helper class for generic extension methods.
    /// </summary>
    internal static class ExtensionMethods
    {
        /// <summary>
        /// Deep clone an object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="a">Object instance.</param>
        /// <returns>A deep clone of the given object.</returns>
        public static T Clone<T>(this T a)
        {
            if (a == null)
            {
                return default;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;

                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
