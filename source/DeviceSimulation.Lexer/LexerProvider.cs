using Antlr4.Runtime;
using DeviceSimulation.Lexer.Listeners;
using System.Collections.Generic;

namespace DeviceSimulation.Lexer
{
    public interface ILexerProvider
    {
        IDictionary<string, object> RunLexer(string json, string configuration);
    }

    public class LexerProvider
    {
        public IDictionary<string, object> RunLexer(IDictionary<string, object> properties, string configuration)
        {
            ServiceLocator.BuildServiceProvider();

            var stateService = ServiceLocator.GetRequiredService<IStateService>();
            foreach (var kvp in properties) stateService.AddOrUpdateValue(kvp.Key, kvp.Value);

            var antlrInputStream = new AntlrInputStream(configuration);
            var deviceSimulationLexer = new DeviceSimulationLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(deviceSimulationLexer);
            var deviceSimulationParser = new DeviceSimulationParser(commonTokenStream);
            deviceSimulationParser.AddParseListener(new DeviceListener());

            deviceSimulationParser.program();

            return stateService.ToDictionary();
        }
    }
}
