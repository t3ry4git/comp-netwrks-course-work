using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;


namespace comp_netwrks_course_work
{

    public partial class ConnectionManipulator : Window
    {
        private readonly NetworkAnalyzer Network;
        private readonly GraphWindow window;

        public ConnectionManipulator(NetworkAnalyzer nc,GraphWindow graphWindow)
        {
            window = graphWindow;
            InitializeComponent();
            Network = nc;
            NodesDataGrid.ItemsSource = Network.Nodes;
            ConnectionsDataGrid.ItemsSource = Network.Connections;
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void Refresh()
        {
            NodesDataGrid.Items.Refresh();
            ConnectionsDataGrid.Items.Refresh();
        }

        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Hidden;
            var editor = new NodeEditorWindow(Network.Nodes);
            if (editor.ShowDialog() == true && editor.ResultNode != null) 
                Network.Nodes.Add(editor.ResultNode);
            Visibility = Visibility.Visible;
            Network.Redraw();
        }

        private void EditNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodesDataGrid.SelectedItem is Node selectedNode)
            {
                Visibility = Visibility.Hidden;
                var nodesWithoutCurrent = Network.Nodes.Where(node => node != selectedNode).ToList();
                var editor = new NodeEditorWindow(nodesWithoutCurrent, selectedNode);
                if (editor.ShowDialog() == true && editor.ResultNode != null) 
                {
                        var oldNode = selectedNode;
                        selectedNode.Number = editor.ResultNode.Number;
                        selectedNode.Type = editor.ResultNode.Type;
                        UpdateConnections(oldNode, selectedNode);
                }
                Visibility = Visibility.Visible;
                Network.Redraw();
                Refresh(); 
            }

        }
        private void UpdateConnections(Node oldNode, Node newNode)
        {
            foreach (var connection in Network.Connections)
            {
                if ((connection.Node1 == oldNode || connection.Node2 == oldNode) 
                    && newNode.Type == NodeType.Disabled)
                    connection.Type = ConnectionType.Disabled;
                if (connection.Node1 == oldNode)
                    connection.Node1 = newNode;
                if (connection.Node2 == oldNode)
                    connection.Node2 = newNode;
            }
        }
        private void UpdateNodes(Connection newConnection, Connection? oldConnection = null)
        {
            foreach(var node in Network.Nodes)
            {
                if (oldConnection == null)
                {
                    if (node.Number == newConnection.Node1.Number || node.Number == newConnection.Node2.Number)
                        node.Connections.Add(newConnection);
                }
                else {
                    if (node.Connections.Contains(oldConnection))
                    {
                        node.Connections.Remove(oldConnection);
                    }
                }
            }
            if (oldConnection != null) { 
                foreach (var node in Network.Nodes)
                {
                    if (node.Number == newConnection.Node1.Number || node.Number == newConnection.Node2.Number)
                        node.Connections.Add(newConnection);
                }
            }
        }
        private void DeleteConFromNode(Connection connection)
        {
            foreach (var node in Network.Nodes)
            {
                node.Connections.Remove(connection);
            }
        }

        private void DeleteNodeAndConnections(Node nodeToRemove)
        {
            Network.Connections.RemoveAll(connection => 
            connection.Node1 == nodeToRemove || connection.Node2 == nodeToRemove);
            Network.Nodes.Remove(nodeToRemove);
        }


        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodesDataGrid.SelectedItem is Node selectedNode)
            {
                DeleteNodeAndConnections(selectedNode);
                Network.Redraw();
                Refresh(); 
            }

        }

        private void AddConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Network.Nodes.Count < 2) return;
            Visibility = Visibility.Hidden;
            var editor = new ConnectionEditorWindow([.. Network.Nodes]);
            if (editor.ShowDialog() == true && editor.ResultConnection != null)
            {
                Network.Connections.Add(editor.ResultConnection);
                UpdateNodes(editor.ResultConnection);
            }
            Visibility = Visibility.Visible;
            Network.Redraw();
            Refresh();

        }

        private void EditConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionsDataGrid.SelectedItem is Connection selectedConnection)
            {
                Visibility = Visibility.Hidden;
                var editor = new ConnectionEditorWindow([.. Network.Nodes], selectedConnection);
                
                if (editor.ShowDialog() == true && editor.ResultConnection != null) 
                {
                    var saveSelected = selectedConnection;
                    selectedConnection.Node1 = editor.ResultConnection.Node1;
                        selectedConnection.Node2 = editor.ResultConnection.Node2;
                        selectedConnection.Weight = editor.ResultConnection.Weight;
                        selectedConnection.Type = editor.ResultConnection.Type;
                        selectedConnection.ChanceOfError = editor.ResultConnection.ChanceOfError;
                    UpdateNodes(editor.ResultConnection,saveSelected);
                        
                }
                Visibility = Visibility.Visible;
                Network.Redraw();
                Refresh();
            }

        }


        private void DeleteConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionsDataGrid.SelectedItem is Connection selectedConnection)
            {
                DeleteConFromNode(selectedConnection);
                Network.Connections.Remove(selectedConnection);
                Network.Redraw();
                Refresh();
            }

        }


        public static List<Node> GetOptimalPath(Node source, Node sink, NetworkAnalyzer Network)
        {
            var distances = new Dictionary<Node, int>();
            var previousNodes = new Dictionary<Node, Node?>();
            var unvisitedNodes = new HashSet<Node>();

            foreach (var node in Network.Nodes)
            {
                distances[node] = int.MaxValue; 
                previousNodes[node] = null;   
                unvisitedNodes.Add(node);    
            }
            distances[source] = 0; 

            while (unvisitedNodes.Count > 0)
            {
                var current = unvisitedNodes.OrderBy(n => distances[n]).First();
                unvisitedNodes.Remove(current);

                if (current == sink)
                    break;

                foreach (var connection in current.GetConnections())
                {
                    var neighbor = connection.Node1 == current ? connection.Node2 : connection.Node1;

                    if (!unvisitedNodes.Contains(neighbor))
                        continue;

                    var newDist = distances[current] + connection.Weight;
                    if (newDist < distances[neighbor])
                    {
                        distances[neighbor] = newDist;
                        previousNodes[neighbor] = current;
                    }
                }
            }

            var path = new List<Node>();
            var currentPathNode = sink;
            while (currentPathNode != null)
            {
                path.Insert(0, currentPathNode);
                currentPathNode = previousNodes[currentPathNode];
            }

            if (path.FirstOrDefault() != source)
                return []; 

            return path;
        }

        private void FindOptimalPathButton_Click(object sender, RoutedEventArgs e)
        {
            ClearOptimalPathButton_Click();
            Refresh();
            var optimalPathWindow = new OptimalPathWindow(Network.Nodes);

            if (optimalPathWindow.ShowDialog() == true)
            {
                var sourceNode = optimalPathWindow.SelectedSource;
                var sinkNode = optimalPathWindow.SelectedSink;
                if (sourceNode != null && sinkNode != null)
                {
                    var optimalPath = GetOptimalPath(sourceNode, sinkNode, Network);
                    if (optimalPath.Count > 0)
                    {
                        HighlightPathOnGraph(optimalPath);
                        MessageBox.Show("Optimal path on screen.", "Success", MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Path is not exist",
                                        "Info",
                                        MessageBoxButton.OK,
                                        MessageBoxImage.Information);
                    }
                }
            }
        }

        private void HighlightPathOnGraph(List<Node> optimalPath)
        {
            ClearOptimalPathButton_Click();

            for (int i = 0; i < optimalPath.Count - 1; i++)
            {
                var current = optimalPath[i];
                var next = optimalPath[i + 1];
                var connection = Network.Connections.FirstOrDefault(c =>
                    (c.Node1 == current && c.Node2 == next) ||
                    (c.Node1 == next && c.Node2 == current));

                if (connection != null)
                    connection.Highlighted = true;
            }
            Network.Redraw();
        }

        private void ClearOptimalPathButton_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            foreach (var connection in Network.Connections)
                connection.Highlighted = false;
            Network.Redraw();
        }

        private void Fulker_Click(object sender, RoutedEventArgs e)
        {
            window.DontCLOSE = true;
            var fulker = new TimedFordFulkersonWindow(Network);
            Visibility = Visibility.Hidden;
            fulker.Show();
            fulker.Closed += (s, args) =>
            {
                Visibility = Visibility.Visible;
                window.DontCLOSE = false;
            };
        }

        private void PacketSender_Click(object sender, RoutedEventArgs e)
        {
            window.DontCLOSE = true;
            var packet = new SimulationWindow(Network);
            Visibility = Visibility.Hidden;
            packet.Show();
            packet.Closed += (s, args) =>
            {
                Visibility = Visibility.Visible;
                window.DontCLOSE = false;
            };
        }

        private void SaveRoute_Click(object sender, RoutedEventArgs e)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Route Table");
            worksheet.Cell(1, 1).Value = "Start";
            worksheet.Cell(1, 2).Value = "End";
            int i = 2;
            foreach (Node node1 in Network.Nodes)
            {
                foreach (Node node2 in Network.Nodes)
                {
                    if (node1 != node2)
                    {
                        var route = FordFulkerson.GetData(node1, node2, 0, new List<Node>(), null, -1);
                        int max_flow = route.flow;
                        List<Connection> cons = route.connections;
                        bool found = route.found;
                        do
                        {
                            route = FordFulkerson.GetData(node1, node2, 0, new List<Node>(), null, -1);
                            if (route.flow > max_flow && route.found == true)
                            {
                                cons = route.connections;
                                max_flow = route.flow;
                                found = route.found;
                            }
                        } while (route.found == true);
                        foreach (var conny in Network.Connections)
                        {
                            conny.ResetFlow();
                            conny.WeightUsed = 0;
                            conny.Highlighted = false;
                        }
                        if(found == true)
                        {
                            worksheet.Cell(i,1).Value = node1.ToString();
                            worksheet.Cell(i,2).Value = node2.ToString();
                            cons.Reverse();
                            int prev = node1.Number;
                            worksheet.Cell(i, 3).Value = node1.OnlyNumberToString();
                            worksheet.Cell(1, 3).Value = "STEP 1";
                            for (int k = 0; k < cons.Count; k++) {
                                if (cons[k].Node1.Number == prev)
                                {
                                    worksheet.Cell(i, k + 4).Value = cons[k].Node2.OnlyNumberToString();
                                    prev = cons[k].Node2.Number;
                                }
                                else {
                                    worksheet.Cell(i, k + 4).Value = cons[k].Node1.OnlyNumberToString();
                                    prev = cons[k].Node1.Number;
                                }
                                worksheet.Cell(1, k + 4).Value = "STEP " + (k+2).ToString();
                            }
                            worksheet.Cell(i, cons.Count + 4).Value = $"Flow: {max_flow}";
                            i++;
                        }
                    }
                }
            }
            try
            {
                workbook.SaveAs("savedroute.xlsx");
            }
            catch(Exception ex)
            {
                MessageBox.Show($"Cannot save file", "Error",MessageBoxButton.OK,MessageBoxImage.Error);
                Debug.WriteLine(ex.Message);
                return;
            }
            MessageBox.Show("Saved route table", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
