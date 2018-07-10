using Xunit;

namespace DeviceSimulation.Lexer.Tests
{
    public class DeviceLexerTests
    {
        [Fact]
        public void SetNewPropertyValue()
        {
            // Essentially start at 72 degrees
            // Set a random offset by 5 degrees + or -
            // Return value
            var configuration = @"
START 72;
OFFSET BY 5;
RETURN START;
";

            var lexerProvider = new LexerProvider();
            var result = lexerProvider.RunLexer(@"3+3");

            Assert.Equal(6, result);
        }
    }
}
