using Microsoft.Msagl.Drawing;

namespace comp_netwrks_course_work
{
    public class Node(int number, NodeType nodeType)
    {
        public int Number { get; set; } = number;
        public NodeType Type { get; set; } = nodeType;
        public List<Connection> Connections { get; set; } = [];


        public Color GetColor() => Type switch
        {
            NodeType.Red => Color.IndianRed,
            NodeType.Green => Color.LightGreen,
            NodeType.Blue => Color.LightBlue,
            _ => Color.White,
        };

        public List<Connection> GetConnections() => Type != NodeType.Disabled ? Connections : [];

        public Node Clone() => new(Number, Type)
        {
            Connections = []
        };

        public override string ToString() => $"Node #{Number}";
        public string OnlyNumberToString() => $"{Number}";
        public bool IsEqual(Node other)
        {
            if(Number !=  other.Number) return false;
            if(Type != other.Type) return false;
            return true;
        }
    }
}
