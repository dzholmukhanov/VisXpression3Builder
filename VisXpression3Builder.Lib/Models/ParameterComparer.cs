using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisXpression3Builder.Lib.Models
{
    public class ParameterComparer : IEqualityComparer<Parameter>
    {
        public bool Equals(Parameter x, Parameter y)
        {
            return x.Name == y.Name && x.Type == y.Type;
        }

        public int GetHashCode(Parameter obj)
        {
            return obj.Name.GetHashCode() ^ obj.Type.GetHashCode();
        }
    }
}
