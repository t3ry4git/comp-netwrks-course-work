﻿using System.Windows;
using System.Windows.Media;


namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for ConnectionManipulator.xaml
    /// </summary>
    public partial class ConnectionManipulator : Window
    {
        List<Node> Nodes;
        List<Connection> Connections;
        NetworkAnalyzer Network;
        public ConnectionManipulator(List<Node> nodes, List<Connection> connections, NetworkAnalyzer nc)
        {
            InitializeComponent();
            Nodes = nodes;
            Connections = connections;
            Network = nc;
            // Привязка данных к DataGrid
            NodesDataGrid.ItemsSource = Nodes;
            ConnectionsDataGrid.ItemsSource = Connections;

        }


        private void AddNodeButton_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            var editor = new NodeEditorWindow(Nodes);
            editor.UpdateTheme(Properties.Settings.Default.currentTheme);

            if (editor.ShowDialog() == true) // Окно вернуло успешный результат
            {
                if (editor.ResultNode != null)
                {
                    Nodes.Add(editor.ResultNode); // Добавляем новую Node
                }
            }
            this.Visibility = Visibility.Visible;
            Network.RefreshNetwork(Nodes, Connections, false);
            Connections = Network.Connections;
            NodesDataGrid.Items.Refresh();
            ConnectionsDataGrid.Items.Refresh();
        }

        private void EditNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodesDataGrid.SelectedItem is Node selectedNode)
            {
                this.Visibility = Visibility.Hidden;
                var nodesWithoutCurrent = Nodes.Where(node => node != selectedNode).ToList();
                var editor = new NodeEditorWindow(nodesWithoutCurrent, selectedNode);
                editor.UpdateTheme(Properties.Settings.Default.currentTheme);

                if (editor.ShowDialog() == true) // Окно вернуло успешный результат
                {
                    if (editor.ResultNode != null)
                    {
                        var oldNode = selectedNode;
                        selectedNode.Number = editor.ResultNode.Number;
                        selectedNode.Type = editor.ResultNode.Type;
                        UpdateConnections(oldNode, selectedNode);
                    }
                }
                this.Visibility = Visibility.Visible;
                Network.RefreshNetwork(Nodes, Connections, false);
                Connections = Network.Connections;
                NodesDataGrid.Items.Refresh();
                ConnectionsDataGrid.Items.Refresh();
            }
        }
        private void UpdateConnections(Node oldNode, Node newNode)
        {
            foreach (var connection in Connections)
            {
                if ((connection.Node1 == oldNode || connection.Node2 == oldNode) && newNode.Type == NodeType.Disabled)
                {
                    connection.Type = ConnectionType.Disabled;
                }
                // Если Node1 совпадает, заменяем
                if (connection.Node1 == oldNode)
                {
                    connection.Node1 = newNode;

                }

                // Если Node2 совпадает, заменяем
                if (connection.Node2 == oldNode)
                {
                    connection.Node2 = newNode;
                }
            }
        }

        private void DeleteNodeAndConnections(Node nodeToRemove)
        {
            // Удаляем все связи, которые включают данный узел
            Connections.RemoveAll(connection => connection.Node1 == nodeToRemove || connection.Node2 == nodeToRemove);

            // Удаляем сам узел из списка
            Nodes.Remove(nodeToRemove);
        }


        private void DeleteNodeButton_Click(object sender, RoutedEventArgs e)
        {
            if (NodesDataGrid.SelectedItem is Node selectedNode)
            {
                DeleteNodeAndConnections(selectedNode);
                Network.RefreshNetwork(Nodes, Connections, false);
                Connections = Network.Connections;
                NodesDataGrid.Items.Refresh();
                ConnectionsDataGrid.Items.Refresh();
            }
        }

        private void AddConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Nodes.Count < 2) return;

            this.Visibility = Visibility.Hidden;

            var editor = new ConnectionEditorWindow(Nodes.ToList()); // Передаём список узлов
            editor.UpdateTheme(Properties.Settings.Default.currentTheme);
            if (editor.ShowDialog() == true) // Окно вернуло успешный результат
            {
                if (editor.ResultConnection != null)
                {
                    Connections.Add(editor.ResultConnection); // Добавляем новую связь
                }

            }
            this.Visibility = Visibility.Visible;
            Network.RefreshNetwork(Nodes, Connections, false);
            Connections = Network.Connections;
            NodesDataGrid.Items.Refresh();
            ConnectionsDataGrid.Items.Refresh();

        }

        private void EditConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (ConnectionsDataGrid.SelectedItem is Connection selectedConnection)
            {
                this.Visibility = Visibility.Hidden;
                var editor = new ConnectionEditorWindow(Nodes.ToList(), selectedConnection); // Передаём список узлов и текущую связь
                editor.UpdateTheme(Properties.Settings.Default.currentTheme);
                if (editor.ShowDialog() == true) // Окно вернуло успешный результат
                {
                    if (editor.ResultConnection != null)
                    {
                        // Обновляем данные существующей связи
                        selectedConnection.Node1 = editor.ResultConnection.Node1;
                        selectedConnection.Node2 = editor.ResultConnection.Node2;
                        selectedConnection.Weight = editor.ResultConnection.Weight;
                        selectedConnection.Type = editor.ResultConnection.Type;
                    }
                }
                this.Visibility = Visibility.Visible;
                Network.RefreshNetwork(Nodes, Connections, false);
                Connections = Network.Connections;
                NodesDataGrid.Items.Refresh();
                ConnectionsDataGrid.Items.Refresh();
            }
        }


        private void DeleteConnectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConnectionsDataGrid.SelectedItem is Connection selectedConnection)
            {
                Connections.Remove(selectedConnection);
                Network.RefreshNetwork(Nodes, Connections, false);
                Connections = Network.Connections;
                NodesDataGrid.Items.Refresh();
                ConnectionsDataGrid.Items.Refresh();
            }
        }

        public void UpdateTheme(string themePath)
        {
            var theme = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(theme);
            this.Background = (Brush)Application.Current.Resources["BackgroundColor"];
            this.Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }
    }
}
