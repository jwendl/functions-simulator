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
            var configuration = @"start 72 vary 5";

            var lexerProvider = new LexerProvider();
            var result = lexerProvider.RunLexer(configuration);

            Assert.NotEqual(0, result);
            Assert.NotEqual(72, result);
        }
    }
}
