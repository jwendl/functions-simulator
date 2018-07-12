using System.Collections.Generic;

namespace DeviceSimulation.Lexer.Interfaces
{
    public interface IStateService
    {
        void AddOrUpdateValue(string key, object value);

        T GetOrAddValue<T>(string key);

        IDictionary<string, object> ToDictionary();
    }
}
