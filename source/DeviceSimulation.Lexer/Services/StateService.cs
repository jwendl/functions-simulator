using DeviceSimulation.Lexer.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DeviceSimulation.Lexer.Services
{
    public class StateService
        : IStateService
    {
        private readonly ConcurrentDictionary<string, object> values;

        public StateService()
        {
            values = new ConcurrentDictionary<string, object>();
        }

        public void AddOrUpdateValue(string key, object value)
        {
            values.AddOrUpdate(key, value, (k, v) =>
            {
                return values.GetOrAdd(k, v);
            });
        }

        public T GetOrAddValue<T>(string key)
        {
            return (T)values.GetOrAdd(key, default(T));
        }

        public IDictionary<string, object> ToDictionary()
        {
            return values.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
