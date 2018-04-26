using System.Collections.Generic;
using VisXpression3Builder.Lib.Models;

namespace VisXpression3Builder.Lib.Repositories
{
    /// <summary>
    /// Functions that can be stored, manipulated, executed. They are parsed from D3NE graphs that use only static/user-defined/basic/flow-control functions as node types.
    /// Have to be implemented by library-user.
    /// </summary>
    public interface IUserDefinedFunctionsRepository : IFunctionsRepository
    {
        D3NEGraph CreateFunction(string name, D3NEGraph graph, string createdBy);
        D3NEGraph UpdateFunction(string name, D3NEGraph graph, string updatedBy);
        bool DeleteFunction(string name);
    }
}
