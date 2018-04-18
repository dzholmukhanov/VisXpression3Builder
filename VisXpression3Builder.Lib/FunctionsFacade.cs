using System;
using System.Linq.Expressions;
using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Lib
{
    public class FunctionsFacade
    {
        internal IFunctionsRepository BasicsRepo;
        internal IFunctionsRepository StaticsRepo;
        internal IFunctionsRepository FlowControlsRepo;
        internal IUserDefinedFunctionsRepository UserDefinedsRepo;
        internal ADomainFunctionsRepository DomainFuncsRepo;

        public FunctionsFacade(
            IUserDefinedFunctionsRepository userDefinedFuncsRepo,
            ABuiltInFunctionRepository<StaticFunctionAttribute> staticFuncsRepo,
            ADomainFunctionsRepository domainFuncsRepo
        ){
            BasicsRepo = new BasicFunctionsRepository();
            StaticsRepo = staticFuncsRepo;
            FlowControlsRepo = new FlowControlFunctionsRepository();
            UserDefinedsRepo = userDefinedFuncsRepo;
            DomainFuncsRepo = domainFuncsRepo;
        }

        public D3NEGraph GetFunctionGraph(string functionName)
        {
            if (IsStaticFunction(functionName)) return StaticsRepo.GetFunctionGraph(functionName);
            if (IsBasicFunction(functionName)) return BasicsRepo.GetFunctionGraph(functionName);
            if (IsFlowControlFunction(functionName)) return FlowControlsRepo.GetFunctionGraph(functionName);
            if (IsUserDefinedFunction(functionName)) return UserDefinedsRepo.GetFunctionGraph(functionName);
            if (IsDomainFunction(functionName)) return DomainFuncsRepo.GetFunctionGraph(functionName);

            throw new ArgumentException($"Function {functionName} does not exist");
        }

        public bool Exists(string functionName)
        {
            return StaticsRepo.Exists(functionName) || BasicsRepo.Exists(functionName) || UserDefinedsRepo.Exists(functionName) || FlowControlsRepo.Exists(functionName) || DomainFuncsRepo.Exists(functionName) || functionName == FunctionNames.Get;
        }

        public bool IsPureFunction(string functionName)
        {
            return StaticsRepo.Exists(functionName) || BasicsRepo.Exists(functionName) || UserDefinedsRepo.Exists(functionName) || DomainFuncsRepo.Exists(functionName) || functionName == FunctionNames.Get;
        }

        public bool IsControlFunction(string functionName)
        {
            return FlowControlsRepo.Exists(functionName);
        }

        public bool IsBasicFunction(string functionName)
        {
            return BasicsRepo.Exists(functionName);
        }

        public bool IsStaticFunction(string functionName)
        {
            return StaticsRepo.Exists(functionName);
        }

        public bool IsFlowControlFunction(string functionName)
        {
            return FlowControlsRepo.Exists(functionName);
        }

        public bool IsUserDefinedFunction(string functionName)
        {
            return UserDefinedsRepo.Exists(functionName);
        }

        public bool IsDomainFunction(string functionName)
        {
            return DomainFuncsRepo.Exists(functionName);
        }

        public bool IsLocalScopeFunction(string functionName)
        {
            return functionName == FunctionNames.Entry || functionName == FunctionNames.Return || functionName == FunctionNames.Set || functionName == FunctionNames.Get;
        }

        public Type GetInputParameterType(string functionName, int parameterIndex)
        {
            if (IsStaticFunction(functionName)) return StaticsRepo.GetInputParameterType(functionName, parameterIndex);
            if (IsBasicFunction(functionName)) return BasicsRepo.GetInputParameterType(functionName, parameterIndex);
            if (IsFlowControlFunction(functionName)) return FlowControlsRepo.GetInputParameterType(functionName, parameterIndex);
            if (IsUserDefinedFunction(functionName)) return UserDefinedsRepo.GetInputParameterType(functionName, parameterIndex);
            if (IsDomainFunction(functionName)) return DomainFuncsRepo.GetInputParameterType(functionName, parameterIndex);

            throw new ArgumentException($"Function {functionName} does not exist");
        }

        public Expression GetExpressionTree(string functionName, params Expression[] arguments)
        {
            if (IsBasicFunction(functionName)) return BasicsRepo.GetExpressionTree(functionName, arguments);
            if (IsStaticFunction(functionName)) return StaticsRepo.GetExpressionTree(functionName, arguments);
            if (IsUserDefinedFunction(functionName)) return UserDefinedsRepo.GetExpressionTree(functionName, arguments);
            if (IsFlowControlFunction(functionName)) return FlowControlsRepo.GetExpressionTree(functionName, arguments);
            if (IsDomainFunction(functionName)) return DomainFuncsRepo.GetExpressionTree(functionName, arguments);

            throw new ArgumentException($"Function {functionName} does not exist");
        }
    }
}