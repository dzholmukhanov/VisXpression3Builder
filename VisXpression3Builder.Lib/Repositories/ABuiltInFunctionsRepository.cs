using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Lib.Repositories
{
    public abstract class ABuiltInFunctionRepository<A> : IFunctionsRepository where A : FunctionAttribute
    {
        public bool Exists(string functionName)
        {
            return GetFunctionMethods().Any(mi => mi.Name == functionName);
        }

        public virtual D3NEGraph GetFunctionGraph(string name)
        {
            throw new NotSupportedException();
        }

        public IEnumerable<FunctionDeclaration> GetFunctions()
        {
            return GetFunctionMethods()
                .Select(mi => new FunctionDeclaration
                {
                    Name = mi.Name,
                    Title = ((A)mi.GetCustomAttributes(typeof(A), false).First()).Title,
                    Inputs = mi.GetParameters().Select(p => new Parameter
                    {
                        Name = p.Name,
                        Type = DataTypes.GetSystemType(p.ParameterType)
                    }).ToArray(),
                    Output = new Parameter
                    {
                        Name = ((A)mi.GetCustomAttributes(typeof(A), false).First()).OutputName,
                        Type = DataTypes.GetSystemType(mi.ReturnType)
                    }
                })
                .ToArray();
        }

        public Expression GetExpressionTree(string functionName, params Expression[] arguments)
        {
            var methodInfo = GetFunctionMethods()
                .FirstOrDefault(mi =>
                    mi.Name == functionName &&
                    mi.GetParameters()
                        .Select(p => p.ParameterType)
                        .SequenceEqual(arguments.Select(a => a.Type))
                );
            if (methodInfo == null) throw new ArgumentException($"Function {functionName} does not exist in {GetType().ToString()}");
            return Expression.Call(methodInfo, arguments);
        }

        public Type GetInputParameterType(string functionName, int parameterIndex)
        {
            var methodInfo = GetFunctionMethods().FirstOrDefault(mi => mi.Name == functionName);
            if (methodInfo == null) throw new ArgumentException($"Function {functionName} does not exist in {GetType().ToString()}");
            if (parameterIndex >= methodInfo.GetParameters().Length) throw new ArgumentException($"Function {functionName} doesn't have such a parameter");
            return methodInfo.GetParameters()[parameterIndex].ParameterType;
        }

        protected IEnumerable<MethodInfo> GetFunctionMethods()
        {
            return GetType()
                .GetMethods()
                .Where(m => m.CustomAttributes.Any(att => att.AttributeType == typeof(A)));
        }
    }
}