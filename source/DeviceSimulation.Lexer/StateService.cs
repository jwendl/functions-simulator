using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace DeviceSimulation.Lexer
{
    public interface IStateService
    {
        void AddOrUpdateValue(string key, object value);

        T GetOrAddValue<T>(string key);

        IDictionary<string, object> ToDictionary();
    }

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
