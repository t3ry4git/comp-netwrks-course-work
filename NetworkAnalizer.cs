using Microsoft.Msagl.Drawing;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows;

namespace comp_netwrks_course_work
{
    public enum NodeType
    {
        Red,
        Green,
        Blue,
        Disabled
    }

    public enum ConnectionType
    {
        Satellite,
        Duplex,
        HalfDuplex,
        Custom,
        Random,
        Disabled
    }
    public enum Direction
    {
        Bidirectional,
        DirectionalNode1Node2,
        DirectionalNode2Node1,
        DirectionalUndefined
    }

    public class Connection
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public int Weight { get; set; }

        public List<int> FlowsNode1Node2 { get; set; }
        public List<int> FlowsNode2Node1 { get; set; }
        public int WeightUsed { get; set; }

        public ConnectionType Type { get; set; }
        private Direction BackupDirection;
        public Direction Direction { get; set; }
        public bool Highlighted { get; set; } = false;
        public Connection(Node node1, Node node2, int weight, ConnectionType connectionType)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Type = connectionType;
            FlowsNode1Node2 = new List<int>();
            FlowsNode2Node1 = new List<int>();
            var values = (ConnectionType[])Enum.GetValues(typeof(ConnectionType));
            var random = new Random();
            if (connectionType == ConnectionType.Random)
            {
                Type = values[random.Next(0, 3)];
            }
            if (Type == ConnectionType.Duplex || Type == ConnectionType.Satellite)
            {
                Direction = Direction.Bidirectional;
            }
            else
            {
                Direction = Direction.DirectionalUndefined;
            }
        }

        public (Direction wantedto, int max) getMaxFlow(Node sender, int weight_wanted, Direction wanted = Direction.DirectionalUndefined)
        {
            if (sender.Equals(Node1))
                wanted = Direction.DirectionalNode1Node2;
            else
                wanted = Direction.DirectionalNode2Node1;
            if (Direction == Direction.Bidirectional)
            {
                if (wanted == Direction.DirectionalNode1Node2)
                    return (wanted,Weight - WeightUsed - weight_wanted*FlowsNode1Node2.Count);
                else
                    return (wanted, Weight - WeightUsed - weight_wanted*FlowsNode2Node1.Count);
            }
            if (wanted == Direction.DirectionalNode1Node2 && (Direction == Direction.DirectionalUndefined || Direction == Direction.DirectionalNode1Node2))
                return (wanted, Weight - WeightUsed - weight_wanted * FlowsNode1Node2.Count);
            if (wanted == Direction.DirectionalNode2Node1 && (Direction == Direction.DirectionalUndefined || Direction == Direction.DirectionalNode2Node1))
                return (wanted, Weight - WeightUsed - weight_wanted * FlowsNode2Node1.Count);

            return (wanted, 0);
        }

        public void SendFlow(int weight, Direction wanted)
        {
            BackupDirection = Direction;

            if(Direction != Direction.Bidirectional)
                Direction = wanted;
            if (wanted == Direction.DirectionalNode1Node2)
                FlowsNode1Node2.Add(weight);
            if (wanted == Direction.DirectionalNode2Node1)
                FlowsNode2Node1.Add(weight);
        }
        public void RevertFlow()
        {
            Direction = BackupDirection;
            if (FlowsNode1Node2.Count >0)
                FlowsNode1Node2.Remove(FlowsNode1Node2.Last());
            if(FlowsNode2Node1.Count >0)
                FlowsNode2Node1.Remove(FlowsNode2Node1.Last());
        }

        public void ResetFlow()
        {
            FlowsNode1Node2 = new List<int>();
            FlowsNode2Node1 = new List<int>();
            if (Type == ConnectionType.Duplex || Type == ConnectionType.Satellite)
            {
                Direction = Direction.Bidirectional;
            }
            else
            {
                Direction = Direction.DirectionalUndefined;
            }
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
            int for_balance_n1n2 = FlowsNode1Node2.Count > 0 ? FlowsNode1Node2.Last() * FlowsNode1Node2.Count: 0;
            int for_balance_n2n1 = FlowsNode2Node1.Count > 0 ? FlowsNode2Node1.Last() * FlowsNode2Node1.Count: 0;
            if (Direction == Direction.Bidirectional)
                return (Weight - WeightUsed - (int.Max(for_balance_n1n2, for_balance_n2n1) -int.Min(for_balance_n1n2, for_balance_n2n1))).ToString();    
            if(Direction == Direction.DirectionalNode1Node2)
                return (Weight - WeightUsed - FlowsNode1Node2.Last()).ToString();
            if(Direction == Direction.DirectionalNode2Node1)
                return (Weight - WeightUsed - FlowsNode2Node1.Last()).ToString();

            return "0";
        }
        public override string ToString()
        {
            return $"Connection => type: {Type}, node1 : {Node1}, node2 : {Node2}, weight : {Weight}";
        }


            public Connection Clone()
            {
                return new Connection(Node1, Node2, Weight, Type)
                {
                    FlowsNode1Node2 = new List<int>(FlowsNode1Node2),
                    FlowsNode2Node1 = new List<int>(FlowsNode2Node1),
                    WeightUsed = WeightUsed,
                    Direction = Direction,
                    Highlighted = Highlighted
                };
            }
    }

    public class Node(int number, NodeType nodeType)
    {
        public int Number { get; set; } = number;
        public NodeType Type { get; set; } = nodeType;
        public List<Connection> Connections { get; set; } = [];

        public Color GetColor()
        {
            return Type switch
            {
                NodeType.Red => Color.IndianRed,
                NodeType.Green => Color.LightGreen,
                NodeType.Blue => Color.LightBlue,
                _ => Color.White,
            };
        }

        public List<Connection> GetConnections() => (Type != NodeType.Disabled) ? Connections : new List<Connection>();

        public Node Clone()
        {
            return new Node(Number, Type)
            {
                Connections = new List<Connection>() // Пустые, будут заполнены в NetworkAnalyzer
            };
        }
        public override string ToString()
        {
            return $"Node #{Number} : {Type}";
        }
    }

    public class NetworkAnalyzer
    {
        private int TotalNodes;
        private readonly int SatelliteChannels;
        private readonly int AverageDegree;
        private readonly List<int> Weights;
        private readonly List<ConnectionType> Cons;
        private readonly GraphWindow graph;
        public List<Node> Nodes { get; set; }
        public List<Connection> Connections { get; set; }

        public NetworkAnalyzer(List<int> weights, List<ConnectionType> cons, int satellite, int nodeCount, float avg, GraphWindow graphWindow)
        {
            Nodes = [];
            Connections = [];
            graph = graphWindow;
            Weights = weights;
            AverageDegree = (int)(avg / 2.0);
            Cons = cons;
            SatelliteChannels = satellite;
            TotalNodes = nodeCount;
            for (int i = 0; i < TotalNodes; i++)
            {
                Nodes.Add(new Node(i, NodeType.Red));
            }
            foreach (var con in Cons)
                Debug.Write(con.ToString() + " ");
            Debug.WriteLine("");
            foreach (var weight in Weights)
                Debug.Write(weight.ToString() + " ");
            Debug.WriteLine("");
            GenerateNetwork();
        }

        public NetworkAnalyzer Clone()
        {
            // Копируем узлы
            var clonedNodes = Nodes.Select(node => node.Clone()).ToList();

            // Копируем соединения
            var clonedConnections = Connections.Select(conn =>
            {
                var clonedConnection = conn.Clone();

                // Связываем узлы с клонированными соединениями
                clonedConnection.Node1 = clonedNodes.First(n => n.Number == conn.Node1.Number);
                clonedConnection.Node2 = clonedNodes.First(n => n.Number == conn.Node2.Number);

                return clonedConnection;
            }).ToList();

            // Обновляем Connections в каждом клонированном узле
            foreach (var node in clonedNodes)
            {
                node.Connections.Clear();
                node.Connections.AddRange(clonedConnections.Where(conn =>
                    conn.Node1.Number == node.Number || conn.Node2.Number == node.Number));
            }

            // Создаём новый экземпляр NetworkAnalyzer с клонированными данными
            return new NetworkAnalyzer(
                Weights,
                Cons,
                SatelliteChannels,
                TotalNodes,
                AverageDegree * 2, // avg хранится как половина значения
                graph)
            {
                Nodes = clonedNodes,
                Connections = clonedConnections
            };
        }


        public void RefreshNetwork(List<Node> nodes, List<Connection> connections, bool reacreate_connections)
        {
            TotalNodes = nodes.Count;
            Nodes = nodes;
            Connections = connections;
            if (reacreate_connections)
            {
                Connections = new List<Connection>();
                GenerateNetwork();
            }
            graph.DrawGraph(Nodes, Connections);
        }

        public void GenerateNetwork()
        {
            var random = new Random();
            int weight_counter = 0;
            int constype_counter = 0;
            // Создание связей для всех узлов
            foreach (var node in Nodes)
            {
                int edgesNeeded = AverageDegree;
                var potentialNeighbors = new List<Node>(Nodes);
                potentialNeighbors.Remove(node);

                for (int i = 0; i < edgesNeeded; i++)
                {
                    foreach (var conn in Connections)
                    {
                        if (conn.Node2 == node)
                            potentialNeighbors.Remove(conn.Node1);
                    }
                    if (potentialNeighbors.Count > 0)
                    {
                        var neighbor = potentialNeighbors[random.Next(potentialNeighbors.Count)];
                        potentialNeighbors.Remove(neighbor);

                        int weight;
                        if (Properties.Settings.Default.randomWeights)
                            weight = Weights[random.Next(Weights.Count)];
                        else
                        {
                            weight = Weights[weight_counter];
                            weight_counter++;
                            if (weight_counter == Weights.Count)
                                weight_counter = 0;
                        }
                        ConnectionType constype = Cons[constype_counter];
                        constype_counter++;
                        if (constype_counter == Cons.Count)
                            constype_counter = 0;
                        Connection connection;
                        connection = new Connection(node, neighbor, weight, constype);

                        Connections.Add(connection);

                        node.Connections.Add(connection);

                        neighbor.Connections.Add(connection);
                    }
                    else
                    {
                        MessageBox.Show($"Failed create channel #{i}", "Warning", MessageBoxButton.OK);
                    }
                }
            }

            // Добавление спутниковых каналов
            for (int i = 0; i < SatelliteChannels; i++)
            {
                var source = Nodes[random.Next(Nodes.Count)];
                var potentialTargets = new List<Node>(Nodes);
                potentialTargets.Remove(source);
                foreach (var conn in Connections)
                {
                    if (conn.Node2 == source)
                        potentialTargets.Remove(conn.Node1);
                    if (conn.Node1 == source)
                        potentialTargets.Remove(conn.Node2);
                }
                if (potentialTargets.Count > 0)
                {
                    var target = potentialTargets[random.Next(potentialTargets.Count)];
                
                int weight;
                if (Properties.Settings.Default.randomWeights)
                    weight = Weights[random.Next(Weights.Count)];
                else
                {
                    weight = Weights[weight_counter];
                    weight_counter++;
                    if (weight_counter == Weights.Count - 1)
                        weight_counter = 0;
                }
                var satelliteConnection = new Connection(source, target, weight, ConnectionType.Satellite);
                Connections.Add(satelliteConnection);

                source.Connections.Add(satelliteConnection);
                target.Connections.Add(satelliteConnection);
                }
                else
                {
                    MessageBox.Show($"Failed create sattelite channel #{i}", "Warning", MessageBoxButton.OK);
                }
            }
        }

        public void PrintNetwork()
        {
            foreach (var connection in Connections)
            {
                Debug.WriteLine(connection.ToString());
            }
        }
    }
}
