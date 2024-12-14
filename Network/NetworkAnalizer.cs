using System.Windows;

namespace comp_netwrks_course_work
{
    public class NetworkAnalyzer
    {
        private int TotalNodes;
        private readonly int SatelliteChannels;
        private readonly int AverageDegree;
        private readonly List<int> Weights;
        private readonly List<ConnectionType> Cons;
        private readonly GraphWindow graph;
        private readonly List<double> errorChance;
        public List<Node> Nodes { get; set; }
        public List<Connection> Connections { get; set; }

        public GraphWindow GraphWindow
        {
            get => default;
            set
            {
            }
        }

        public ConnectionManipulator ConnectionManipulator
        {
            get => default;
            set
            {
            }
        }

        public NetworkAnalyzer(List<int> weights,
                               List<ConnectionType> cons,
                               int satellite,
                               int nodeCount,
                               float avg,
                               List<double> errorchance,
                               GraphWindow graphWindow)
        {
            Nodes = [];
            Connections = [];
            graph = graphWindow;
            Weights = weights;
            errorChance = errorchance;
            AverageDegree = (int)(avg / 2.0);
            Cons = cons;
            SatelliteChannels = satellite;
            TotalNodes = nodeCount;
            for (int i = 0; i < TotalNodes; i++)
                Nodes.Add(new Node(i, NodeType.Red));
            GenerateNetwork();
        }

        public NetworkAnalyzer Clone()
        {
            var clonedNodes = Nodes.Select(node => node.Clone()).ToList();
            var clonedConnections = Connections.Select(conn =>
            {
                var clonedConnection = conn.Clone();
                clonedConnection.Node1 = clonedNodes.First(n => n.Number == conn.Node1.Number);
                clonedConnection.Node2 = clonedNodes.First(n => n.Number == conn.Node2.Number);
                return clonedConnection;
            }).ToList();

            foreach (var node in clonedNodes)
            {
                node.Connections.Clear();
                node.Connections.AddRange(clonedConnections.Where(conn =>
                    conn.Node1.Number == node.Number || conn.Node2.Number == node.Number));
            }

            return new NetworkAnalyzer(
                Weights,
                Cons,
                SatelliteChannels,
                TotalNodes,
                AverageDegree * 2,
                errorChance,
                graph)
            {
                Nodes = clonedNodes,
                Connections = clonedConnections,
            };
        }

        public void RefreshNetwork(List<Node> nodes,
                                   List<Connection> connections,
                                   bool reacreate_connections = false)
        {
            TotalNodes = nodes.Count;
            Nodes = nodes;
            Connections = connections;
            if (reacreate_connections)
            {
                Connections = [];
                GenerateNetwork();
            }
            graph.DrawGraph(Nodes, Connections);
        }

        public void Redraw() => graph.DrawGraph(Nodes, Connections);

        public void GenerateNetwork()
        {
            var random = new Random();
            int weight_counter = 0;
            int error_counter = 0;
            int constype_counter = 0;
            foreach (var node in Nodes)
            {
                int edgesNeeded = AverageDegree;
                var potentialNeighbors = new List<Node>(Nodes);
                potentialNeighbors.Remove(node);

                for (int i = 0; i < edgesNeeded; i++)
                {
                    foreach (var conn in Connections)
                        if (conn.Node2 == node)
                            potentialNeighbors.Remove(conn.Node1);
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
                        double error = 0;
                        if (Properties.Settings.Default.ErrorType == "Custom")
                        {
                            error = errorChance[error_counter];
                            error_counter++;
                            if (error_counter == errorChance.Count)
                                error_counter = 0;
                        }
                        else
                        {
                            error = errorChance[random.Next(errorChance.Count)];
                        }

                        ConnectionType constype = Cons[constype_counter];
                        constype_counter++;
                        if (constype_counter == Cons.Count)
                            constype_counter = 0;
                        Connection connection;
                        connection = new Connection(node, neighbor, weight, constype,error);
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
                    double error = 0;
                    if (Properties.Settings.Default.ErrorType == "Custom")
                    {
                        error = errorChance[error_counter];
                        error_counter++;
                        if (error_counter == errorChance.Count)
                            error_counter = 0;
                    }
                    else
                    {
                        error = errorChance[random.Next(errorChance.Count)];
                    }
                    var satelliteConnection = new Connection(source, target, weight, ConnectionType.Satellite, error);
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

        public bool IsEqual(NetworkAnalyzer other)
        {
            if (other.TotalNodes != TotalNodes) return false;
            if (other.SatelliteChannels != SatelliteChannels) return false;
            if (other.AverageDegree != AverageDegree) return false;
            if (other.Weights != Weights) return false;
            if (other.graph != graph) return false;
            if (other.Cons != Cons) return false;
            for(int i = 0; i < Nodes.Count; i++)
            {
                if (!Nodes[i].IsEqual(other.Nodes[i])) return false;
            }

            for (int i = 0; i < Connections.Count; i++)
            {
                if (!Connections[i].IsEqual(other.Connections[i])) return false;
            }

            return true;
        }
    }
}
