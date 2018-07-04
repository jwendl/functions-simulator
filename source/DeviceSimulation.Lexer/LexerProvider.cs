using Antlr4.Runtime;
using DeviceSimulation.Lexer.Visitors;

namespace DeviceSimulation.Lexer
{
    public interface ILexerProvider
    {
        object RunLexer(string configuration);
    }

    public class LexerProvider
    {
        public object RunLexer(string configuration)
        {
            var antlrInputStream = new AntlrInputStream(configuration);
            var deviceSimulationLexer = new DeviceSimulationLexer(antlrInputStream);
            var commonTokenStream = new CommonTokenStream(deviceSimulationLexer);
            var deviceSimulationParser = new DeviceSimulationParser(commonTokenStream);

            var tree = deviceSimulationParser.compileUnit();
            var visitor = new SimpleDeviceSimulationVisitor();
            return visitor.Visit(tree);
        }
    }
}
