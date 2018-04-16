namespace VisXpression3Builder.Lib.Models
{
    public class D3NEGraph
    {
        public string Description { get; set; }

        public Node[] Nodes { get; set; }
        public Parameter[] Inputs { get; set; }
        public Parameter Output { get; set; }
        public Parameter[] LocalVariables { get; set; }

        public class Node
        {
            public int Id { get; set; }
            public Data Data { get; set; }
            public object Group { get; set; }
            public InputSocket[] Inputs { get; set; }
            public OutputSocket[] Outputs { get; set; }
            public string Title { get; set; }
            public double[] Position { get; set; }

            internal string Type
            {
                get
                {
                    return Data?._Id;
                }
            }
            internal string DisplayName
            {
                get
                {
                    return Title ?? Type ?? Id.ToString();
                }
            }
        }

        public class Data
        {
            public string _Id { get; set; }
            public string Value { get; set; }
        }

        public class InputSocket
        {
            public InputConnection[] Connections { get; set; }

            internal InputConnection this[int index]
            {
                get
                {
                    if (index >= Connections.Length) return null;
                    return Connections[index];
                }
            }
        }

        public class OutputSocket
        {
            public OutputConnection[] Connections { get; set; }

            internal OutputConnection this[int index]
            {
                get
                {
                    if (index >= Connections.Length) return null;
                    return Connections[index];
                }
            }
        }

        public class InputConnection
        {
            public int Node { get; set; }
            public int Output { get; set; }
        }

        public class OutputConnection
        {
            public int Node { get; set; }
            public int Input { get; set; }
        }
    }
}