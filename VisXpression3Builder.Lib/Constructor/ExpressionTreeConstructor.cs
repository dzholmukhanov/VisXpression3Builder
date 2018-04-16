using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Utils;

namespace VisXpression3Builder.Lib.Constructor
{
    public class ExpressionTreeConstructor
    {
        public IEnumerable<D3NEGraph.Node> Nodes { get; set; }
        public LambdaExpression ConstructedExpression { get; set; }
        public string FunctionName { get; set; }
        private GraphValidator Validator { get; set; }
        private FunctionsFacade FunctionsFacade { get; set; }

        private Dictionary<string, ParameterExpression> LocalVars { get; set; }

        /// <summary>
        /// Used for caching and to prevent infinite loop of parsing when there are circular dependencies
        /// </summary>
        private Dictionary<string, LambdaExpression> CachedTrees { get; set; }

        private ParameterExpression[] EntryParams { get; set; }
        private Dictionary<Tuple<int, int>, ParameterExpression> LoopVars { get; set; }
        private ParameterExpression Self { get; set; }
        private ParameterExpression ReturnParam { get; set; }
        private LabelTarget ReturnTarget { get; set; }

        public ExpressionTreeConstructor (
            string functionName, 
            D3NEGraph graph,
            FunctionsFacade functionFacade, 
            Dictionary<string, LambdaExpression> cachedTrees = null
        ) {
            FunctionName = functionName;
            FunctionsFacade = functionFacade;
            CachedTrees = cachedTrees ?? new Dictionary<string, LambdaExpression>();
            Validator = new GraphValidator(graph, functionFacade);
            Validator.Validate();

            Nodes = graph.Nodes;

            LocalVars = new Dictionary<string, ParameterExpression>();
            foreach (var localVar in graph.LocalVariables)
            {
                LocalVars[localVar.Name] = Expression.Variable(DataTypes.GetType(localVar.Type), $"{functionName}-{localVar.Name}");
            }

            EntryParams = new ParameterExpression[graph.Inputs.Length];
            int index = 0;
            foreach (var param in graph.Inputs)
            {
                EntryParams[index++] = Expression.Parameter(DataTypes.GetType(param.Type), $"{functionName}-{param.Name}");
            }

            ReturnParam = Expression.Parameter(DataTypes.GetType(graph.Output.Type), $"{functionName}-{graph.Output.Name}");

            LoopVars = new Dictionary<Tuple<int, int>, ParameterExpression>();
            var loopNodes = graph.Nodes.Where(n => n.Type == FunctionNames.For || n.Type == FunctionNames.ForEach);
            foreach (var loopNode in loopNodes)
            {
                if (loopNode.Type == FunctionNames.For)
                {
                    LoopVars[Tuple.Create(loopNode.Id, 1)] = Expression.Variable(typeof(double?), $"{functionName}-LoopVar{loopNode.Id}");
                }
                else if (loopNode.Type == FunctionNames.ForEach)
                {
                    LoopVars[Tuple.Create(loopNode.Id, 1)] = Expression.Variable(typeof(double?), $"{functionName}-LoopVar{loopNode.Id}");
                    LoopVars[Tuple.Create(loopNode.Id, 2)] = Expression.Variable(typeof(double?), $"{functionName}-LoopVar{loopNode.Id}");
                }
            }

            Type[] typeArgs = graph.Inputs.Select(p => DataTypes.GetType(p.Type)).Concat(new Type[]{ DataTypes.GetType(graph.Output.Type) }).ToArray();
            Type lambdaType = Expression.GetFuncType(typeArgs);
            Self = Expression.Variable(lambdaType, $"{functionName}-Self");
            
            ReturnTarget = Expression.Label(ReturnParam.Type, $"{functionName}-Return");
        }

        public LambdaExpression Construct()
        {
            var entry = Nodes.First(n => n.Type == FunctionNames.Entry);

            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            visited[entry.Id] = true;

            Expression body = GetControlExpression(entry.Outputs[0][0].Node, visited);

            ConstructedExpression = Expression.Lambda(
                Self.Type,
                Expression.Block(
                    LocalVars.Values.Concat(new[] { Self }),
                    Expression.Assign(
                        Self,
                        Expression.Lambda(
                            Expression.Block(
                                ReturnParam.Type,
                                body,
                                Expression.Label(ReturnTarget, Expression.Default(ReturnParam.Type))
                            ),
                            EntryParams
                        )
                    ),
                    Expression.Invoke(Self, EntryParams)
                ),
                EntryParams
            );
            CachedTrees[FunctionName] = ConstructedExpression;

            return ConstructedExpression;
        }

        private D3NEGraph.Node GetNode(int id)
        {
            var node = Nodes.FirstOrDefault(n => n.Id == id);
            if (node == null) throw new InvalidBalanceGraphException($"Couldn't find a node with id {id}");
            return node;
        }

        private Expression GetControlExpression(int node, Dictionary<int, bool> visited)
        {
            if (visited.ContainsKey(node) && visited[node]) throw new InvalidBalanceGraphException("Infinite flow loop");

            visited[node] = true;

            var currentNode = GetNode(node);

            var inputDataExpressions = new Expression[currentNode.Inputs.Length];
            var outputFlowExpressions = new Expression[currentNode.Outputs.Length];

            // ASSUMPTION: first input socket is a flow socket, the rest are data sockets
            for (int i = 1; i < currentNode.Inputs.Length; i++)
            {
                var connection = currentNode.Inputs[i][0];
                if (connection == null)
                {
                    inputDataExpressions[i] = GetDefaultValueOfEmptyInput(currentNode.Type, i, currentNode.Data.Value);
                }
                else
                {
                    inputDataExpressions[i] = GetDataExpression(node, i);
                }
            }

            for (int i = 0; i < currentNode.Outputs.Length; i++)
            {
                var connection = currentNode.Outputs[i][0];
                if (connection != null && !FunctionsFacade.IsPureFunction(GetNode(connection.Node).Type))
                {
                    outputFlowExpressions[i] = GetControlExpression(connection.Node, visited);
                }
                else
                {
                    outputFlowExpressions[i] = Expression.Empty();
                }
            }

            visited[node] = false;

            switch (currentNode.Type)
            {
                case FunctionNames.If:
                    return Expression.IfThenElse(
                        Expression.Convert(inputDataExpressions.ElementAtOrDefault(1), typeof(bool)),
                        outputFlowExpressions.ElementAtOrDefault(0),
                        outputFlowExpressions.ElementAtOrDefault(1)
                    );
                case FunctionNames.ForEach:
                    return Expression.Block(
                        ExpressionUtils.ForEach(
                            inputDataExpressions.ElementAtOrDefault(1),
                            LoopVars[Tuple.Create(node, 1)],
                            outputFlowExpressions.ElementAtOrDefault(0)
                        ),
                        outputFlowExpressions.ElementAtOrDefault(3)
                    );
                case FunctionNames.For:
                    throw new NotImplementedException();
                case FunctionNames.While:
                    throw new NotImplementedException();
                case FunctionNames.Return:
                    return Expression.Return(ReturnTarget, inputDataExpressions.ElementAtOrDefault(1));
                case FunctionNames.Set:
                    return Expression.Block(
                        Expression.Assign(LocalVars[currentNode.Data.Value], inputDataExpressions.ElementAtOrDefault(1)),
                        outputFlowExpressions[0]
                    );
                default:
                    throw new InvalidBalanceGraphException("Not a control function or is entry");
            }
        }

        private Expression GetDataExpression(int node, int input)
        {
            var currentNode = GetNode(node);
            Dictionary<int, bool> visited = new Dictionary<int, bool>();
            return GetDataExpression(currentNode.Inputs[input][0].Node, currentNode.Inputs[input][0].Output, visited);
        }

        private Expression GetDataExpression(int? node, int? output, Dictionary<int, bool> visited)
        {
            if (visited.ContainsKey(node.Value) && visited[node.Value]) throw new InvalidBalanceGraphException("Infinite data loop");

            var currentNode = GetNode(node.Value);

            if (FunctionsFacade.IsPureFunction(currentNode.Type))
            {
                visited[node.Value] = true;
                var inputExpressions = new Expression[currentNode.Inputs.Length];
                for (int i = 0; i < currentNode.Inputs.Length; i++)
                {
                    var connection = currentNode.Inputs[i][0];
                    if (connection == null)
                    {
                        inputExpressions[i] = GetDefaultValueOfEmptyInput(currentNode.Type, i, currentNode.Data.Value);
                    }
                    else
                    {
                        inputExpressions[i] = GetDataExpression(connection.Node, connection.Output, visited);
                    }
                }
                visited[node.Value] = false;

                if (currentNode.Type == FunctionNames.Number)
                {
                    return Expression.Constant((double?)Convert.ToDouble(currentNode.Data.Value), typeof(double?));
                }
                else if (currentNode.Type == FunctionNames.Boolean)
                {
                    return Expression.Constant((bool?)Convert.ToBoolean(currentNode.Data.Value), typeof(bool?));
                }
                else if (currentNode.Type == FunctionNames.NumberArray)
                {
                    return Expression.Constant(JsonConvert.DeserializeObject<double?[]>(currentNode.Data.Value), typeof(double?[]));
                }
                if (currentNode.Type == FunctionNames.Get)
                {
                    return LocalVars[currentNode.Data.Value];
                }
                else
                {
                    try
                    {
                        var tree = FunctionsFacade.GetExpressionTree(currentNode.Type, inputExpressions);
                        return tree;
                    }
                    catch(NeedToConstructExpressionTreeException)
                    {
                        if (CachedTrees.ContainsKey(currentNode.Type)) return Expression.Invoke(CachedTrees[currentNode.Type], inputExpressions);
                        else
                        {
                            var graph = FunctionsFacade.GetFunctionGraph(currentNode.Type);
                            var constructor = new ExpressionTreeConstructor(currentNode.Type, graph, FunctionsFacade, CachedTrees);
                            var tree = constructor.Construct();
                            return Expression.Invoke(tree, inputExpressions);
                        }
                    }
                }
            }
            else
            {
                switch (currentNode.Type)
                {
                    case FunctionNames.Entry:
                        return EntryParams[output.Value - 1];
                    case FunctionNames.ForEach:
                    case FunctionNames.For:
                        return LoopVars[Tuple.Create(currentNode.Id, output.Value)];
                    case FunctionNames.Set:
                        return LocalVars[currentNode.Data.Value];
                }
            }

            throw new InvalidBalanceGraphException($"Couldn't recognize node {currentNode.Type}");
        }

        private Expression GetDefaultValueOfEmptyInput(string nodeType, int emptyInputIndex, string nodeValue)
        {
            if (FunctionsFacade.IsLocalScopeFunction(nodeType))
            {
                switch (nodeType)
                {
                    case FunctionNames.Return:
                        return Expression.Default(ReturnParam.Type);
                    case FunctionNames.Set:
                        return Expression.Default(LocalVars[nodeValue].Type);
                }
            }
            else
            {
                return Expression.Default(FunctionsFacade.GetInputParameterType(nodeType, emptyInputIndex));
            }

            throw new InvalidBalanceGraphException($"Couldn't recognize node {nodeType}");
        }
    }
}
