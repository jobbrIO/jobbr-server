using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Jobbr.Server.Web
{
    /// <summary>
    /// Inspired by code from Michael Schnyder.
    /// Deserializes JSON to an .NET object with type <paramref name="TType"/>
    /// </summary>
    /// <typeparam name="TType">
    /// </typeparam>
    public class JsonTypeConverter<TType> : JsonConverter
    {
        /// <summary>
        /// The cached types.
        /// </summary>
        private static List<Type> possibleTypes;

        /// <summary>
        /// The prop selector name.
        /// </summary>
        private readonly string propSelectorName;

        private readonly Func<List<Type>, string, Type> resolver;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeConverter{TType}"/> class.
        /// </summary>
        /// <param name="propSelectorName">
        /// The prop selector name.
        /// </param>
        public JsonTypeConverter(string propSelectorName, Func<List<Type>, string, Type> resolver)
        {
            this.propSelectorName = propSelectorName;
            this.resolver = resolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeConverter{TType}"/> class.
        /// </summary>
        /// <param name="propSelectorName">
        /// The prop selector name.
        /// </param>
        public JsonTypeConverter(string propSelectorName)
        {
            this.propSelectorName = propSelectorName;
            this.resolver = DefaultResolver;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var seri = new JsonSerializer();

            seri.ContractResolver = new CamelCasePropertyNamesContractResolver();
            seri.NullValueHandling = NullValueHandling.Ignore;

            seri.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Load JObject from stream 
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject 
            var target = this.CreateInstanceForType(jObject);

            // Populate the object properties 
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(TType).IsAssignableFrom(objectType);
        }

        private static List<Type> GetTypesFromAllAssemblies()
        {
            if (possibleTypes == null)
            {
                possibleTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).Where(t => t.IsSubclassOf(typeof(TType)) || typeof(TType).IsAssignableFrom(t)).ToList();
            }

            return possibleTypes;
        }

        private TType CreateInstanceForType(JObject jObject)
        {
            var property = jObject.Properties().FirstOrDefault(p => p.Name.ToLowerInvariant() == this.propSelectorName.ToLowerInvariant());

            if (property == null)
            {
                throw new ArgumentException(string.Format("The json didn't contain a property named '{0}'!", this.propSelectorName));
            }

            var typeValue = (string)property.Value;

            if (typeValue == null)
            {
                throw new ArgumentException(string.Format("The property '{0}' was null!", this.propSelectorName));
            }

            var typesFromAllAssemblies = GetTypesFromAllAssemblies();

            var type = this.resolver(typesFromAllAssemblies, typeValue);

            if (type == null)
            {
                throw new ArgumentException(string.Format("The cannot create object for empty type", typeValue));
            }

            return (TType)Activator.CreateInstance(type);
        }

        private static Type DefaultResolver(IEnumerable<Type> typesFromAllAssemblies, string type)
        {
            // t.Name.ToLowerInvariant() == type.ToLowerInvariant()
            // dotTrace shows that calling ToLowerInvariant is slow.
            // https://msdn.microsoft.com/en-us/library/dd465121(v=vs.110).aspx says Use comparisons with StringComparison.Ordinal or StringComparison.OrdinalIgnoreCase for better performance.
            var types = typesFromAllAssemblies.Where(t => t.Name.Equals(type, StringComparison.OrdinalIgnoreCase)).ToList();

            if (!types.Any())
            {
                throw new ArgumentException(string.Format("The Message-type '{0}' is not supported!", type));
            }

            if (types.Count() > 1)
            {
                throw new ArgumentException(string.Format("multiple types for typename '{0}' found!", type));
            }
            
            return types.First();
        }
    }
}
