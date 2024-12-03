using Microsoft.Msagl.Drawing;
using System.Diagnostics;

namespace comp_netwrks_course_work
{
    public enum NodeType
    {
        Red,
        Green,
        Blue,
    }

    public enum ConnectionType
    {
        Satellite,
        Duplex,
        HalfDuplex
    }

    public class Connection
    {
        public Node Node1 { get; }
        public Node Node2 { get; }
        public int Weight { get; }

        public ConnectionType Type { get; set; }

        public Connection(Node node1, Node node2, int weight, ConnectionType connectionType)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Type = connectionType;
        }

        public Color GetColor()
        {
            switch (Type)
            {
                case ConnectionType.Duplex:
                case ConnectionType.HalfDuplex:
                    return Color.Gray;
                case ConnectionType.Satellite:
                    return Color.DarkCyan;
                default:
                    return Color.Gray;
            }
        }

        public override string ToString()
        {
            return $"Connection => type: {Type}, node1 : {Node1.ToString()}, node2 : {Node2.ToString()}, weight : {Weight}";
        }
    }

    public class Node
    {
        public int Number { get; }
        public NodeType Type { get; set; }
        public List<Connection> Connections { get; }

        public Node(int number, NodeType nodeType)
        {
            Number = number;
            Connections = new List<Connection>();
            Type = nodeType;
        }

        public Color GetColor()
        {
            switch (Type)
            {
                case NodeType.Red:
                    return Color.IndianRed;
                case NodeType.Green:
                    return Color.LightGreen;
                case NodeType.Blue:
                    return Color.LightBlue;
                default:
                    return Color.White;
            }
        }

        public override string ToString() {
            return $"Node #{Number} : {Type}";
        }
    }

    public class NetworkAnalyzer
    {
        private const int TotalNodes = 20;
        private const int SatelliteChannels = 2;
        private const int AverageDegree = 3;
        private List<int> Weights;
        public List<Node> Nodes { get; }
        public List<Connection> Connections { get; }

        public NetworkAnalyzer(List<int> weights)
        {
            Nodes = new List<Node>();
            Connections = new List<Connection>();
            Weights = weights;
            for (int i = 0; i < TotalNodes; i++)
            {
                Nodes.Add(new Node(i, i < 9 ? NodeType.Red : i < 18 ? NodeType.Green : NodeType.Blue));
            }
            GenerateNetwork();
        }

        public void GenerateNetwork(List<ConnectionType>? connectionTypes = null)
        {
            var random = new Random();
            int weight_counter = 0;
            // Создание связей для всех узлов
            foreach (var node in Nodes)
            {
                int edgesNeeded = AverageDegree / 2;
                var potentialNeighbors = new List<Node>(Nodes);
                potentialNeighbors.Remove(node);

                for (int i = 0; i < edgesNeeded; i++)
                {
                    foreach(var conn in Connections)
                    {
                        if (conn.Node2 == node)
                            potentialNeighbors.Remove(conn.Node1);
                    }
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
                    Connection connection;
                    if(connectionTypes != null)
                        connection = new Connection(node, neighbor, weight, connectionTypes[i]);
                    else
                        connection = new Connection(node,neighbor,weight, random.Next(0, 2) == 0 ? ConnectionType.Duplex : ConnectionType.HalfDuplex);

                    Connections.Add(connection);

                    node.Connections.Add(connection);
                    neighbor.Connections.Add(connection);
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
                var target = potentialTargets[random.Next(potentialTargets.Count)];
                int weight;
                if (Properties.Settings.Default.randomWeights)
                    weight = Weights[random.Next(Weights.Count)];
                else
                {
                    weight = Weights[weight_counter ];
                    weight_counter++;
                    if(weight_counter == Weights.Count -1)
                        weight_counter = 0;
                }
                var satelliteConnection = new Connection(source, target, weight,ConnectionType.Satellite);
                Connections.Add(satelliteConnection);
                
                source.Connections.Add(satelliteConnection);
                target.Connections.Add(satelliteConnection);
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
