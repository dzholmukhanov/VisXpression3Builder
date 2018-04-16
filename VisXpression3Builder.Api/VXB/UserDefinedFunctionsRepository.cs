using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VisXpression3Builder.Lib.Constants;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Api.VXB
{
    internal class UserDefinedFunctionsRepository : IUserDefinedFunctionsRepository
    {
        public D3NEGraph GetFunctionGraph(string name)
        {
            using (var db = new ApiDbContext())
            {
                var function = db.UserDefinedFunctions.Find(name);
                if (function == null) return null;
                return JsonConvert.DeserializeObject<D3NEGraph>(function.GraphJson);
            }
        }

        public IEnumerable<FunctionDeclaration> GetFunctions()
        {
            using (var db = new ApiDbContext())
            {
                var dbFuncs = db.UserDefinedFunctions.ToArray();
                var result = new List<FunctionDeclaration>(dbFuncs.Length);
                foreach (var func in dbFuncs)
                {
                    var graph = JsonConvert.DeserializeObject<D3NEGraph>(func.GraphJson);
                    result.Add(new FunctionDeclaration
                    {
                        Description = graph.Description,
                        Name = func.Name,
                        Title = func.Name,
                        Inputs = graph.Inputs,
                        Output = graph.Output
                    });
                }

                return result;
            }
        }

        public D3NEGraph CreateFunction(string name, D3NEGraph graph, string createdBy)
        {
            using (var db = new ApiDbContext())
            {
                db.UserDefinedFunctions.Add(new UserDefinedFunction
                {
                    Name = name,
                    GraphJson = JsonConvert.SerializeObject(graph),
                    CreatedOn = DateTime.Now,
                    CreatedBy = createdBy,
                    LastUpdatedOn = DateTime.Now,
                    LastUpdatedBy = createdBy
                });
                db.SaveChanges();

                return graph;
            }
        }

        public D3NEGraph UpdateFunction(string name, D3NEGraph graph, string updatedBy)
        {
            using (var db = new ApiDbContext())
            {
                var function = db.UserDefinedFunctions.Find(name);
                if (function == null) return null;

                function.GraphJson = JsonConvert.SerializeObject(graph);
                function.LastUpdatedBy = updatedBy;
                function.LastUpdatedOn = DateTime.Now;
                db.SaveChanges();

                return graph;
            }
        }

        public bool DeleteFunction(string name)
        {
            using (var db = new ApiDbContext())
            {
                var function = db.UserDefinedFunctions.Find(name);
                if (function == null) return false;

                db.UserDefinedFunctions.Remove(function);
                db.SaveChanges();
                return true;
            }
        }

        public bool Exists(string functionName)
        {
            using (var db = new ApiDbContext())
            {
                var function = db.UserDefinedFunctions.Find(functionName);
                return function != null;
            }
        }

        public Expression GetExpressionTree(string functionName, params Expression[] arguments)
        {
            throw new NeedToConstructExpressionTreeException();
        }

        public Type GetInputParameterType(string functionName, int parameterIndex)
        {
            var graph = GetFunctionGraph(functionName);
            if (graph == null) throw new ArgumentException($"Function {functionName} does not exist in {GetType().ToString()}");
            if (parameterIndex >= graph.Inputs.Length) throw new ArgumentException($"Function {functionName} doesn't have such a parameter");
            return DataTypes.GetType(graph.Inputs[parameterIndex].Type);
        }
    }
}