using Antlr4.Runtime;
using DeviceSimulation.Lexer.Listeners;
using System.Collections.Generic;

namespace DeviceSimulation.Lexer
{
    public interface ILexerProvider
    {
        object RunLexer(string configuration);
    }

    public class LexerProvider
    {
        public IDictionary<string, object> RunLexer(string configuration)
        {
            ServiceLocator.BuildServiceProvider();

            var antlrInputStream = new AntlrInputStream(configuration);
            var deviceSimulationLexer = new DeviceSimulationLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(deviceSimulationLexer);
            var deviceSimulationParser = new DeviceSimulationParser(commonTokenStream);
            deviceSimulationParser.AddParseListener(new DeviceListener());

            deviceSimulationParser.program();

            var stateService = ServiceLocator.GetRequiredService<IStateService>();
            return stateService.ToDictionary();
        }
    }
}
