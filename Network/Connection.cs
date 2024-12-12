using Microsoft.Msagl.Drawing;

namespace comp_netwrks_course_work
{
    public class Connection
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public int Weight { get; set; }
        public List<int> FlowsNode1Node2 { get; set; }
        public List<int> FlowsNode2Node1 { get; set; }
        public int WeightUsed { get; set; }
        public double ChanceOfError { get; set; }
        public ConnectionType Type { get; set; }
        private Direction BackupDirection;
        public Direction LastDirection { get; set; }
        private Direction BackupLastDirection { get; set; }
        public Direction Direction { get; set; }
        public bool Highlighted { get; set; } = false;
        public Connection(Node node1, Node node2, int weight, ConnectionType connectionType)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Type = connectionType;
            FlowsNode1Node2 = [];
            FlowsNode2Node1 = [];
            var values = (ConnectionType[])Enum.GetValues(typeof(ConnectionType));
            var random = new Random();
            if (connectionType == ConnectionType.Random)
                Type = values[random.Next(0, 3)];
            Direction = Type is ConnectionType.Duplex or ConnectionType.Satellite ?
                Direction.Bidirectional : Direction.DirectionalUndefined;

            LastDirection = Direction.DirectionalUndefined;
        }

        public (Direction wantedto, int max) GetMaxFlow(Node sender, int weightWanted)
        {
            Direction wanted = sender.Equals(Node1) ?
                Direction.DirectionalNode1Node2 : Direction.DirectionalNode2Node1;
            if (LastDirection == wanted)
                return (wanted, 0);
            return Direction switch
            {
                Direction.Bidirectional => wanted == Direction.DirectionalNode1Node2 ?
                    (wanted, Weight - WeightUsed - weightWanted * FlowsNode1Node2.Count) :
                    (wanted, Weight - WeightUsed - weightWanted * FlowsNode2Node1.Count),
                _ => wanted switch
                {
                    Direction.DirectionalNode1Node2 when
                    Direction == Direction.DirectionalUndefined || Direction == Direction.DirectionalNode1Node2
                    => (wanted, Weight - WeightUsed - weightWanted * FlowsNode1Node2.Count),
                    Direction.DirectionalNode2Node1 when
                    Direction == Direction.DirectionalUndefined || Direction == Direction.DirectionalNode2Node1
                    => (wanted, Weight - WeightUsed - weightWanted * FlowsNode2Node1.Count),
                    _ => (wanted, 0)
                }
            };
        }

        public void SendFlow(int weight, Direction wanted)
        {
            BackupDirection = Direction;
            BackupLastDirection = LastDirection;
            LastDirection = wanted;

            if (Direction != Direction.Bidirectional)
                Direction = wanted;
            if (wanted == Direction.DirectionalNode1Node2)
                FlowsNode1Node2.Add(weight);
            if (wanted == Direction.DirectionalNode2Node1)
                FlowsNode2Node1.Add(weight);
        }

        public void RevertFlow()
        {
            Direction = BackupDirection;
            LastDirection = BackupLastDirection;
            if (FlowsNode1Node2.Count > 0)
                FlowsNode1Node2.Remove(FlowsNode1Node2.Last());
            if (FlowsNode2Node1.Count > 0)
                FlowsNode2Node1.Remove(FlowsNode2Node1.Last());
        }


        public void ResetFlow()
        {
            FlowsNode1Node2 = [];
            FlowsNode2Node1 = [];
            Direction = Type is ConnectionType.Duplex or ConnectionType.Satellite ?
                Direction.Bidirectional : Direction.DirectionalUndefined;
            LastDirection = Direction;
        }

        public Color GetColor()
        {
            if (Highlighted) return Color.Magenta;
            else return Type switch
            {
                ConnectionType.Duplex => Color.DarkRed,
                ConnectionType.HalfDuplex => Color.DarkOrange,
                ConnectionType.Satellite => Color.DarkCyan,
                _ => Color.Gray,
            };
        }

        public string GetBalancedFlow()
        {
            int for_balance_n1n2 = FlowsNode1Node2.Count > 0 ?
                FlowsNode1Node2.Last() * FlowsNode1Node2.Count : 0;
            int for_balance_n2n1 = FlowsNode2Node1.Count > 0 ?
                FlowsNode2Node1.Last() * FlowsNode2Node1.Count : 0;
            return Direction switch
            {
                Direction.Bidirectional => (Weight - WeightUsed -
                (int.Max(for_balance_n1n2, for_balance_n2n1) -
                int.Min(for_balance_n1n2, for_balance_n2n1))).ToString(),
                Direction.DirectionalNode1Node2 => (Weight - WeightUsed - FlowsNode1Node2.Last()).ToString(),
                Direction.DirectionalNode2Node1 => (Weight - WeightUsed - FlowsNode2Node1.Last()).ToString(),
                _ => "0"
            };
        }

        public static void SetChance(List<Connection> connections, double chance)
        {
            foreach (var con in connections)
                con.ChanceOfError = chance;
        }

        public Connection Clone() => new(Node1, Node2, Weight, Type)
        {
            FlowsNode1Node2 = new List<int>(FlowsNode1Node2),
            FlowsNode2Node1 = new List<int>(FlowsNode2Node1),
            WeightUsed = WeightUsed,
            Direction = Direction,
            Highlighted = Highlighted
        };

        public bool IsEqual(Connection other)
        {
            if(Node1 != other.Node1) return false;
            if(Node2 != other.Node2) return false;
            if(Type != other.Type) return false;
            return true;
        }
    }
}
