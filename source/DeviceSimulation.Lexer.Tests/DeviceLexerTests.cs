using Xunit;

namespace DeviceSimulation.Lexer.Tests
{
    public class DeviceLexerTests
    {
        [Fact]
        public void SetNewPropertyValue()
        {
            /*
             * Examples
begin
    let a be 5
    let b be 10
    add 3 to b
    add b to a
    add a to b
    print b
    print 3
end
             */
            var configuration = @"begin
    let a be 3
    let b be 5
    add 3 to b
    add b to a
    print b
    print a
end";

            var lexerProvider = new LexerProvider();
            var result = lexerProvider.RunLexer(configuration);

            var a = result["a"];
            var b = result["b"];

            Assert.Equal(11d, a);
            Assert.Equal(8d, b);
        }
    }
}
