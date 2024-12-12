using System.Windows;


namespace comp_netwrks_course_work
{

    public partial class ConnectionManipulator : Window
    {
        private readonly NetworkAnalyzer Network;

        public ConnectionManipulator(NetworkAnalyzer nc)
        {
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


        List<Node> GetOptimalPath(Node source, Node sink)
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
                    var optimalPath = GetOptimalPath(sourceNode, sinkNode);
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
            var fulker = new TimedFordFulkersonWindow(Network);
            Visibility = Visibility.Hidden;
            fulker.Show();
            fulker.Closed += (s, args) =>
            {
                Visibility = Visibility.Visible;
            };
        }
    }
}
