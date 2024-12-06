using Microsoft.Msagl.Drawing;
using System;
using System.Diagnostics;

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

    public class Connection
    {
        public Node Node1 { get; set; }
        public Node Node2 { get; set; }
        public int Weight { get; set; }

        public ConnectionType Type { get; set; }

        public Connection(Node node1, Node node2, int weight, ConnectionType connectionType)
        {
            Node1 = node1;
            Node2 = node2;
            Weight = weight;
            Type = connectionType;
            var values = (ConnectionType[])Enum.GetValues(typeof(ConnectionType));
            var random = new Random();
            if (connectionType == ConnectionType.Random)
            {
                Type = values[random.Next(0, 3)];
            }
        }

        public Color GetColor()
        {
            return Type switch
            {
                ConnectionType.Duplex => Color.DarkRed,
                ConnectionType.HalfDuplex => Color.DarkOrange,
                ConnectionType.Satellite => Color.DarkCyan,
                _ => Color.Gray,
            };
        }

        public override string ToString()
        {
            return $"Connection => type: {Type}, node1 : {Node1}, node2 : {Node2}, weight : {Weight}";
        }
    }

    public class Node(int number, NodeType nodeType)
    {
        public int Number { get; set; } = number;
        public NodeType Type { get; set; } = nodeType;
        public List<Connection> Connections { get; } = [];

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

        public override string ToString() {
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
            AverageDegree = (int)(avg/2.0);
            Cons = cons;
            SatelliteChannels = satellite;
            TotalNodes = nodeCount;
            for (int i = 0; i < TotalNodes; i++)
            {
                Nodes.Add(new Node(i, NodeType.Red));
            }
            foreach(var con in Cons)
                Debug.Write(con.ToString()+ " ");
            Debug.WriteLine("");
            foreach(var weight in Weights)
                Debug.Write(weight.ToString()+ " ");
            Debug.WriteLine("");
            GenerateNetwork();
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
