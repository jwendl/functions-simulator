using System.Collections.Generic;
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

            ServiceLocator.BuildServiceProvider();
            var configuration = @"begin
    add 3 to temperature
end";

            var lexerProvider = ServiceLocator.GetRequiredService<ILexerProvider>();
            var properties = new Dictionary<string, object>
            {
                { "temperature", 70 }
            };

            var result = lexerProvider.RunLexer(properties, configuration);

            var actual = result["temperature"];

            Assert.Equal(properties["temperature"], actual);
        }
    }
}
