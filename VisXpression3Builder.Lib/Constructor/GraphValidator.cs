using System;
using System.Collections.Generic;
using System.Linq;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Lib.Constructor
{
    public class GraphValidator
    {
        private D3NEGraph Model { get; set; }
        private FunctionsFacade Facade { get; set; }

        public GraphValidator(D3NEGraph graphModel, FunctionsFacade functionFacade)
        {
            Model = graphModel;
            Facade = functionFacade;
        }

        public void Validate()
        {
            if (Model.Nodes.Count(n => n.Type == FunctionNames.Entry) != 1) throw new InvalidGraphException("Zero or multiple entry points");
            if (!Model.Nodes.Any(n => n.Type == FunctionNames.Return)) throw new InvalidGraphException("No return nodes");

            foreach(var i in Model.Inputs)
            {
                ValidateParameter(i, "input parameter");
            }
            foreach (var l in Model.LocalVariables)
            {
                ValidateParameter(l, "local variable");
            }
            ValidateParameter(Model.Output, "output parameter");

            foreach (var node in Model.Nodes)
            {
                if (node == null) throw new InvalidGraphException("Null node");
                if (node.Data == null) throw new InvalidGraphException($"Data property of node {node.DisplayName} is null");
                //if (node.Inputs.Length == 0 && node.Type != NodeTypes.Entry) throw new InvalidBalanceGraphException($"Node {node.DisplayName} has no inputs");
                if (node.Outputs.Length == 0 && node.Type != FunctionNames.Return) throw new InvalidGraphException($"Node {node.DisplayName} has no output");

                // TODO: Check for type validity
                //if (!NodeTypes.IsValidType(node.Type)) throw new InvalidBalanceGraphException($"Invalid type of node {node.DisplayName}");

                if (Facade.IsPureFunction(node.Type) && node.Outputs.Length != 1) throw new InvalidGraphException("A pure function node must have exactly one output");

                int index = 0;
                foreach (var input in node.Inputs)
                {
                    ValidateInputSocket(input, index, node.DisplayName, node.Type);
                    index++;
                }

                index = 0;
                foreach (var output in node.Outputs)
                {
                    ValidateOutputSocket(output, index, node.DisplayName, node.Type);
                    index++;
                }
            }

            // Checking for connectivity
            Stack<int> stack = new Stack<int>();
            Dictionary<int, bool> visited = new Dictionary<int, bool>();

            foreach(var node in Model.Nodes)
            {
                visited[node.Id] = false;
            }
            var entryNode = Model.Nodes.First(n => n.Type == FunctionNames.Entry);
            stack.Push(entryNode.Id);
            visited[entryNode.Id] = true;

            while (stack.Count > 0)
            {
                var current = GetNode(stack.Pop());
                foreach(var output in current.Outputs)
                {
                    foreach(var conn in output.Connections)
                    {
                        if (!visited[conn.Node])
                        {
                            stack.Push(conn.Node);
                            visited[conn.Node] = true;
                        }
                    }
                }
                foreach (var input in current.Inputs)
                {
                    foreach (var conn in input.Connections)
                    {
                        if (!visited[conn.Node])
                        {
                            stack.Push(conn.Node);
                            visited[conn.Node] = true;
                        }
                    }
                }
            }

            if (visited.Any(kv => kv.Value == false)) throw new InvalidGraphException($"Node {GetNode(visited.First(kv => kv.Value == false).Key).DisplayName} is unreachable");
        }

        private D3NEGraph.Node GetNode(int id)
        {
            var node = Model.Nodes.FirstOrDefault(n => n.Id == id);
            if (node == null) throw new InvalidGraphException($"Couldn't find a node with id {id}");
            return node;
        }

        private void ValidateParameter(Parameter param, string whatParameter)
        {
            if (param == null) throw new InvalidGraphException($"Null {whatParameter}");
            if (param.Name == null) throw new InvalidGraphException($"Unnamed {whatParameter}");
            if (!DataTypes.IsValidType(param.Type)) throw new InvalidGraphException($"Type of {whatParameter} {param.Name} is invalid");
        }

        private void ValidateInputSocket(D3NEGraph.InputSocket socket, int socketIndex, string nodeName, string nodeType)
        {
            if (socket.Connections.Any(c => c == null)) throw new InvalidGraphException($"Null connection on input socket {socketIndex} of the node {nodeName}");

            if (Facade.IsPureFunction(nodeType))
            {
                if (socket.Connections.Length > 1) throw new InvalidGraphException("Ambigious data connection");
            }
            else
            {
                if (socketIndex == 0 && socket.Connections.Any(c => Facade.IsPureFunction(GetNode(c.Node).Type))) throw new InvalidGraphException($"Wrong flow connection on node {nodeName}");
                if (socketIndex > 0 && socket.Connections.Length > 1) throw new InvalidGraphException("Ambigious data connection");
            }
        }

        private void ValidateOutputSocket(D3NEGraph.OutputSocket socket, int socketIndex, string nodeName, string nodeType)
        {
            if (socket.Connections.Any(c => c == null)) throw new InvalidGraphException($"Null connection on output socket {socketIndex} of the node {nodeName}");
            
            if (Facade.IsControlFunction(nodeType))
            {
                switch(nodeType)
                {
                    case FunctionNames.Entry:
                    case FunctionNames.Set:
                        if (socketIndex == 0 && socket.Connections.Length > 1) throw new InvalidGraphException($"Node {nodeName} can't have more than one connection per flow socket");
                        break;
                    case FunctionNames.Return:
                        throw new InvalidGraphException($"Node {nodeName} can't have output sockets");
                    case FunctionNames.If:
                        if ((socketIndex == 0 || socketIndex == 1) && socket.Connections.Length > 1) throw new InvalidGraphException($"Node {nodeName} can't have more than one connection per flow socket");
                        break;
                    case FunctionNames.ForEach:
                        if ((socketIndex == 0 || socketIndex == 3) && socket.Connections.Length > 1) throw new InvalidGraphException($"Node {nodeName} can't have more than one connection per flow socket");
                        break;
                    case FunctionNames.For:
                    case FunctionNames.While:
                        throw new NotImplementedException();
                }
            }
        }
    }
}