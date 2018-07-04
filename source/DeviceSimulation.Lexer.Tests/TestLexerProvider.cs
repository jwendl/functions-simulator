using Xunit;

namespace DeviceSimulation.Lexer.Tests
{
    public class TestLexerProvider
    {
        [Fact]
        public void GenericTest()
        {
            var lexerProvider = new LexerProvider();
            var result = lexerProvider.RunLexer(@"3+3");

            Assert.Equal(6, result);
        }
    }
}
