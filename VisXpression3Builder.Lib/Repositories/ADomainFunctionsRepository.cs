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
    }
}
