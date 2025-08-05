using Common.Libraries.EventSourcing;
using System.Text.Json;

namespace Common.Libraries.EventStore.EF.TestApi
{
    public class EventDeserializer : IEventDeserializer
    {
        private readonly Dictionary<string, Type> _typeRegistry;

        public EventDeserializer(IEnumerable<Type> knownEventTypes = null)
        {
            // Build registry from supplied types or scan by convention
            _typeRegistry = knownEventTypes?.ToDictionary(t => t.Name)
                           ?? DiscoverEventTypes("YourNamespace.Events", "YourAssemblyName");
        }

        public object Deserialize(string eventData, string eventType)
        {
            if (!_typeRegistry.TryGetValue(eventType, out var type))
                throw new InvalidOperationException($"Unknown event type: {eventType}");
            //return JsonConvert.DeserializeObject(eventData, type);
            return JsonSerializer.Deserialize(eventData, type, _options);
        }

        public T Deserialize<T>(string eventData)
        {
            //return JsonConvert.DeserializeObject<T>(eventData);
            return JsonSerializer.Deserialize<T>(eventData, _options);
        }

        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static Dictionary<string, Type> DiscoverEventTypes(string @namespace, string assemblyName)
        {
            var assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == assemblyName);

            if (assembly == null)
                throw new InvalidOperationException($"Assembly '{assemblyName}' not found");

            return assembly.GetTypes()
                .Where(t => t.IsClass && t.Namespace == @namespace)
                .ToDictionary(t => t.Name, t => t);
        }
    }

}
