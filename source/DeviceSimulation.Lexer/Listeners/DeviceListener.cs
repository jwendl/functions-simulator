using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;

namespace DeviceSimulation.Lexer.Listeners
{
    public class DeviceListener
        : DeviceSimulationBaseListener
    {
        private readonly IDictionary<string, object> variables;

        public DeviceListener()
        {
            variables = new Dictionary<string, object>();
        }

        public override void ExitAdd([NotNull] DeviceSimulationParser.AddContext context)
        {
            if (context.ID().Length > 1)
            {
                var variableName = context.ID(1).GetText();
                var variableValue = variables[context.ID(0).GetText()];
                if (variables.ContainsKey(variableName))
                {
                    if (variableValue is double addedValue && variables[variableName] is double currentValue)
                    {
                        AddOrSetValue(variableName, currentValue + addedValue);
                    }
                }
            }
            else
            {
                var variableName = context.ID(0).GetText();
                double addedDouble = 0;
                if (double.TryParse(context.NUMBER().GetText(), out addedDouble))
                {
                    if (variables[variableName] is double currentDouble)
                    {
                        AddOrSetValue(variableName, currentDouble + addedDouble);
                    }
                    else if (variables[variableName] is int currentInt)
                    {
                        AddOrSetValue(variableName, currentInt + addedDouble);
                    }
                }
            }
        }

        public override void ExitAssign([NotNull] DeviceSimulationParser.AssignContext context)
        {
            var variableName = context.ID(0).GetText();
            if (context.ID().Length > 1)
            {
                var variableValue = context.ID(1).GetText();
                if (variables.ContainsKey(variableName))
                {
                    if (variables[variableName] is double currentValue)
                    {
                        AddOrSetValue(variableName, currentValue);
                    }
                }
            }
            else
            {
                var variableValue = context.NUMBER().GetText();
                AddOrSetValue(variableName, Int32.Parse(variableValue));
            }
        }

        public override void EnterProgram([NotNull] DeviceSimulationParser.ProgramContext context)
        {
            var stateService = ServiceLocator.GetRequiredService<IStateService>();
            var properties = stateService.ToDictionary();
            foreach (var kvp in properties)
            {
                variables.Add(kvp.Key, kvp.Value);
            }

            base.EnterProgram(context);
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
