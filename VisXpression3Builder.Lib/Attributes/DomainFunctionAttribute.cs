using System;

namespace VisXpression3Builder.Lib.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DomainFunctionAttribute : FunctionAttribute
    {
        public string[] InputSetTables { get; set; }
        public string[] InputSetColumns { get; set; }
    }
}