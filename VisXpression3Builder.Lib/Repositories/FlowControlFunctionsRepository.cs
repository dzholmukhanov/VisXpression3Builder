using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Lib.Repositories
{
    internal class FlowControlFunctionsRepository : IFunctionsRepository
    {
        [FlowControlFunction]
        public const string Entry = "Entry";
        [FlowControlFunction]
        public const string Set = "Set";
        [FlowControlFunction]
        public const string Return = "Return";

        [FlowControlFunction]
        public const string If = "If";
        [FlowControlFunction]
        public const string Foreach = "Foreach";
        [FlowControlFunction]
        public const string For = "For";
        [FlowControlFunction]
        public const string While = "While";

        private static readonly Dictionary<string, string> Titles = new Dictionary<string, string>
        {
            { Entry, "Точка входа" },
            { Set, "Присвоение" },
            { Return, "Возврат" },

            { If, "Если" },
            { Foreach, "Цикл - Для каждого" },
            { For, "Цикл" },
            { While, "Цикл - Пока" }
        };

        public bool Exists(string functionName)
        {
            return GetFunctions().Any(fn => fn.Name == functionName);
        }

        public D3NEGraph GetFunctionGraph(string name)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<FunctionDeclaration> GetFunctions()
        {
            return typeof(FlowControlFunctionsRepository)
                .GetFields()
                .Where(f => f.CustomAttributes.Any(att => att.AttributeType == typeof(FlowControlFunctionAttribute)))
                .Select(f => new FunctionDeclaration { Name = f.Name, Title = Titles[f.Name] })
                .ToArray();
        }

        public Expression GetExpressionTree(string functionName, params Expression[] arguments)
        {
            throw new NotSupportedException();
        }

        public Type GetInputParameterType(string functionName, int parameterIndex)
        {
            switch (functionName)
            {
                case If:
                    return typeof(bool?);
                case Foreach:
                    return typeof(double?[]);
                case For:
                    return typeof(double?);
                case While:
                    return typeof(bool?);
                default:
                    throw new ArgumentException($"Flow control function {functionName} does not exist");
            }
        }
    }
}