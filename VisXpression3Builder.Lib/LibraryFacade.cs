using System.Collections.Generic;
using VisXpression3Builder.Lib.Attributes;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Constructor;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Lib
{
    public class LibraryFacade
    {
        public FunctionsFacade FunctionsFacade { get; set; }
        private IUserDefinedFunctionsRepository UserDefinedFuncsRepo { get; set; }

        public LibraryFacade(IUserDefinedFunctionsRepository userDefinedFuncsRepo, ABuiltInFunctionRepository<StaticFunctionAttribute> staticFuncsRepo)
        {
            UserDefinedFuncsRepo = userDefinedFuncsRepo;
            FunctionsFacade = new FunctionsFacade(userDefinedFuncsRepo, staticFuncsRepo);
        }

        public D3NEGraph CreateUserDefinedFunction(string name, D3NEGraph graph, string createdBy)
        {
            var constructor = new ExpressionTreeConstructor(name, graph, FunctionsFacade);
            // Graph Validation
            var tree = constructor.Construct();
            return UserDefinedFuncsRepo.CreateFunction(name, graph, createdBy);
        }

        public D3NEGraph UpdateUserDefinedFunction(string name, D3NEGraph graph, string updatedBy)
        {
            var constructor = new ExpressionTreeConstructor(name, graph, FunctionsFacade);
            // Graph Validation
            var tree = constructor.Construct();
            return UserDefinedFuncsRepo.UpdateFunction(name, graph, updatedBy);
        }

        public object ExecuteUserDefinedFunction(string name, FunctionExecutionRequest request)
        {
            var function = UserDefinedFuncsRepo.GetFunctionGraph(name);
            if (function == null) return null;

            object[] objArgs = new object[request.Arguments.Length];
            for (int i = 0; i < request.Arguments.Length; i++)
            {
                objArgs[i] = DataTypes.Parse(request.Arguments[i].Value, request.Arguments[i].Type);
            }
            var constructor = new ExpressionTreeConstructor(name, function, FunctionsFacade);
            var expTree = constructor.Construct();
            var lambda = expTree.Compile();
            var result = lambda.DynamicInvoke(objArgs);

            return result;
        }

        public bool DeleteUserDefinedFunction(string name)
        {
            return FunctionsFacade.UserDefinedsRepo.DeleteFunction(name);
        }

        public IEnumerable<FunctionDeclaration> GetBasicFunctions()
        {
            return FunctionsFacade.BasicsRepo.GetFunctions();
        }

        public IEnumerable<FunctionDeclaration> GetStaticFunctions()
        {
            return FunctionsFacade.StaticsRepo.GetFunctions();
        }

        public IEnumerable<FunctionDeclaration> GetFlowControlFunctions()
        {
            return FunctionsFacade.FlowControlsRepo.GetFunctions();
        }

        public IEnumerable<FunctionDeclaration> GetUserDefinedFunctions()
        {
            return FunctionsFacade.UserDefinedsRepo.GetFunctions();
        }
    }
}
