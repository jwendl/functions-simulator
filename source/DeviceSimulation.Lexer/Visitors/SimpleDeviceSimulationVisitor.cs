using Antlr4.Runtime.Misc;

namespace DeviceSimulation.Lexer.Visitors
{
    public class SimpleDeviceSimulationVisitor
        : DeviceSimulationBaseVisitor<int>
    {
        public override int VisitCompileUnit([NotNull] DeviceSimulationParser.CompileUnitContext context)
        {
            return Visit(context.expression(0));
        }

        public override int VisitNumber(DeviceSimulationParser.NumberContext context)
        {
            return int.Parse(context.GetText());
        }

        public override int VisitAddition(DeviceSimulationParser.AdditionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left + right;
        }

        public override int VisitSubtraction(DeviceSimulationParser.SubtractionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left - right;
        }

        public override int VisitMultiplication(DeviceSimulationParser.MultiplicationContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left * right;
        }

        public override int VisitDivision(DeviceSimulationParser.DivisionContext context)
        {
            var left = WalkLeft(context);
            var right = WalkRight(context);

            return left / right;
        }

        private int WalkLeft(DeviceSimulationParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<DeviceSimulationParser.ExpressionContext>(0));
        }

        private int WalkRight(DeviceSimulationParser.ExpressionContext context)
        {
            return Visit(context.GetRuleContext<DeviceSimulationParser.ExpressionContext>(1));
        }
    }
}
