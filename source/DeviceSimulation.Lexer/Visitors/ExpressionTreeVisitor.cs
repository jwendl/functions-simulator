using Antlr4.Runtime.Misc;
using System;

namespace DeviceSimulation.Lexer.Visitors
{
    public class ExpressionTreeVisitor
        : DeviceSimulationBaseVisitor<int>
    {
        private int startingValue;
        private int endingValue;

        public override int VisitStart([NotNull] DeviceSimulationParser.StartContext context)
        {
            startingValue = Int32.Parse(context.GetChild(1).GetText());

            return base.VisitStart(context);
        }

        public override int VisitVary([NotNull] DeviceSimulationParser.VaryContext context)
        {
            var value = Int32.Parse(context.GetChild(1).GetText());
            var random = new Random();
            var chance = random.Next(0, 100);
            var offset = random.Next(0, value);

            if (chance % 2 == 0)
            {
                endingValue = startingValue + offset;
            }
            else
            {
                endingValue = startingValue - offset;
            }

            return base.VisitVary(context);
        }
    }
}
