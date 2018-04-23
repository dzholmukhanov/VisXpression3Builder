using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Lib.Repositories
{
    public abstract class ADomainFunctionsRepository : ABuiltInFunctionRepository<DomainFunctionAttribute>
    {
        public abstract D3NEGraph UpdateFunction(string name, D3NEGraph graph, string updatedBy);
        public abstract override D3NEGraph GetFunctionGraph(string name);

        public FunctionSignature GetFunctionSignature(string name)
        {
            var methodInfo = GetFunctionMethods().FirstOrDefault(mi => mi.Name == name);
            if (methodInfo == null) throw new ArgumentException($"Function {name} does not exist in {GetType().ToString()}");

            var attribute = (DomainFunctionAttribute)methodInfo.GetCustomAttributes(typeof(DomainFunctionAttribute), false).First();
            var res = new FunctionSignature
            {
                OutputName = attribute.OutputName,
                Title = attribute.Title,
                InputSetTables = attribute.InputSetTables,
                InputSetColumns = attribute.InputSetColumns,
                InputSetTypes = methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(),
                Parameters = methodInfo.GetParameters()
            };

            return res;
        }

        public void SaveFunctionResult(string name, object[] columnValues)
        {
            var methodInfo =  GetType()
                .GetMethods()
                .FirstOrDefault(m => m.Name == name && m.CustomAttributes.Any(att => att.AttributeType == typeof(DomainFunctionSaveResult)));
            if (methodInfo == null) throw new ArgumentException($"Save result function {name} does not exist in {GetType().ToString()}");

            methodInfo.Invoke(null, columnValues);
        }
    }
}