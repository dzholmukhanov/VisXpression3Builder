namespace VisXpression3Builder.Lib.Models
{
    public class FunctionDeclaration
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Parameter[] Inputs { get; set; }
        public Parameter Output { get; set; }
    }
}