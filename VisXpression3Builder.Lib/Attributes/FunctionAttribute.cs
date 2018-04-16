using System;

namespace VisXpression3Builder.Lib.Attributes
{
    public class FunctionAttribute : Attribute
    {
        public string OutputName { get; set; }
        public string Title { get; set; }

        public FunctionAttribute()
        {
            OutputName = "Result";
            Title = "";
        }
    }
}