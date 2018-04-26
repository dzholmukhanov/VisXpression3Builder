using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VisXpression3Builder.Lib.Models;
using VisXpression3Builder.Lib.Repositories;

namespace VisXpression3Builder.Api.VXB
{
    public class DomainFunctionsRepository : ADomainFunctionsRepository
    {
        public override D3NEGraph GetFunctionGraph(string name)
        {
            throw new NotImplementedException();
        }

        public override D3NEGraph UpdateFunction(string name, D3NEGraph graph, string updatedBy)
        {
            throw new NotImplementedException();
        }
    }
}