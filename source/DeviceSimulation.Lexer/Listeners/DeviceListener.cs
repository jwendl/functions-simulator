using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace DeviceSimulation.Lexer.Listeners
{
    public class DeviceListener
        : DeviceSimulationBaseListener
    {
        private readonly IDictionary<string, double> variables;

        public DeviceListener()
        {
            variables = new Dictionary<string, double>();
        }

        public override void ExitAdd([NotNull] DeviceSimulationParser.AddContext context)
        {
            if (context.ID().Length > 1)
            {
                var variableName = context.ID(1).GetText();
                var variableValue = variables[context.ID(0).GetText()];
                AddOrSetValue(variableName, variables[variableName] + variableValue);
            }
            else
            {
                var variableName = context.ID(0).GetText();
                var variableValue = Int32.Parse(context.NUMBER().GetText());
                AddOrSetValue(variableName, variables[variableName] + variableValue);
            }
        }

        public override void ExitAssign([NotNull] DeviceSimulationParser.AssignContext context)
        {
            var variableName = context.ID(0).GetText();
            if (context.ID().Length > 1)
            {
                var variableValue = context.ID(1).GetText();
                AddOrSetValue(variableName, variables[variableValue]);
            }
            else
            {
                var variableValue = context.NUMBER().GetText();
                AddOrSetValue(variableName, Int32.Parse(variableValue));
            }
        }

        public override void ExitProgram([NotNull] DeviceSimulationParser.ProgramContext context)
        {
            var stateService = ServiceLocator.GetRequiredService<IStateService>();

            foreach (var keyValuePair in variables)
            {
                stateService.AddOrUpdateValue(keyValuePair.Key, keyValuePair.Value);
            }

            base.ExitProgram(context);
        }

        private void AddOrSetValue(string variableName, double variableValue)
        {
            if (variables.ContainsKey(variableName))
            {
                variables[variableName] = variableValue;
            }
            else
            {
                variables.Add(variableName, variableValue);
            }
        }
    }
}
