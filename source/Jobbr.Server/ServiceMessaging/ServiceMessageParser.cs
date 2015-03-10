using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Jobbr.Server.ServiceMessaging
{
    public class ServiceMessageParser
    {
        public ServiceMessage Parse(string serviceMessage)
        {
            var messageTypeRaw = string.Empty;
            var parametersRaw = string.Empty;

            Regex regex = new Regex(@"##jobbr\[([a-z]*[A-Z]*) (.*)\]");

            foreach (Match match in regex.Matches(serviceMessage))
            {
                GroupCollection collection = match.Groups;

                if (collection.Count == 3)
                {
                    messageTypeRaw = collection[1].Value;
                    parametersRaw = collection[2].Value;
                }
            }

            // Identity CLR-MessageType
            var typeNameLowerCase = messageTypeRaw + "servicemessage";
            var messageTypes = typeof(ServiceMessage).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(ServiceMessage)));
            var type = messageTypes.FirstOrDefault(t => t.Name.ToLowerInvariant() == typeNameLowerCase);

            // Identity Parameters
            var splitted = parametersRaw.Split(new char[] { '\'', '=' }, StringSplitOptions.RemoveEmptyEntries);
            var parameters = new Dictionary<string, string>();

            for (int i = 0; i < splitted.Count() - 1; i = i + 2)
            {
                parameters.Add(splitted[i], splitted[i + 1]);
            }

            var instance = (ServiceMessage)Activator.CreateInstance(type);

            foreach (var key in parameters.Keys)
            {
                var prop = type.GetProperties().FirstOrDefault(p => p.Name.ToLowerInvariant() == key.ToLowerInvariant());

                if (prop != null)
                {
                    if (prop.PropertyType == typeof(double))
                    {
                        prop.SetValue(instance, double.Parse(parameters[key]));
                    }

                    if (prop.PropertyType == typeof(int))
                    {
                        prop.SetValue(instance, int.Parse(parameters[key]));
                    }

                    if (prop.PropertyType == typeof(string))
                    {
                        prop.SetValue(instance, parameters[key]);
                    }
                }
            }

            return instance;
        }
    }
}