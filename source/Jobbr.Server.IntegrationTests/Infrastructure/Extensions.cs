using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Jobbr.Server.IntegrationTests.Infrastructure
{
    public static class Extensions
    {
        // Deep clone
        public static T Clone<T>(this T a)
        {
            if (a == null)
            {
                return default;
            }

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T)formatter.Deserialize(stream);
            }
        }
    }
}
