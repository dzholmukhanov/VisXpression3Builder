using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace VisXpression3Builder.Lib.Models
{
    public class FunctionSignature
    {
        public string OutputName { get; set; }
        public string Title { get; set; }
        public string[] InputSetTables { get; set; }
        public string[] InputSetColumns { get; set; }
        public Type[] InputSetTypes { get; set; }
        public ParameterInfo[] Parameters { get; set; }
    }
}
