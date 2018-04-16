using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Lib.Repositories
{
    public interface IFunctionsRepository
    {
        D3NEGraph GetFunctionGraph(string name);
        IEnumerable<FunctionDeclaration> GetFunctions();
        bool Exists(string functionName);
        Expression GetExpressionTree(string functionName, params Expression[] arguments);
        Type GetInputParameterType(string functionName, int parameterIndex);
    }
}
