namespace VisXpression3Builder.Lib.Models
{
    public class FunctionExecutionRequest
    {
        public Argument[] Arguments { get; set; }

        public class Argument
        {
            public string Type { get; set; }
            public string Value { get; set; }
        }
    }
}