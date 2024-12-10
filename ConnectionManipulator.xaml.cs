using System.Windows;
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

        List<Node> GetOptimalPath(Node source, Node sink)
        {
            var distances = new Dictionary<Node, int>();
            var previousNodes = new Dictionary<Node, Node?>();
            var unvisitedNodes = new HashSet<Node>();

            // Инициализация
            foreach (var node in Nodes)
            {
                distances[node] = int.MaxValue; // Устанавливаем "бесконечность" для всех узлов
                previousNodes[node] = null;    // Нет предыдущих узлов
                unvisitedNodes.Add(node);     // Все узлы ещё не посещены
            }
            distances[source] = 0; // Начальная точка: расстояние 0

            while (unvisitedNodes.Count > 0)
            {
                // Выбираем узел с минимальным расстоянием
                var current = unvisitedNodes.OrderBy(n => distances[n]).First();
                unvisitedNodes.Remove(current);

                // Если текущий узел — сток, прекращаем выполнение
                if (current == sink)
                    break;

                // Обновляем расстояния для соседей
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

            // Восстанавливаем путь
            var path = new List<Node>();
            var currentPathNode = sink;
            while (currentPathNode != null)
            {
                path.Insert(0, currentPathNode);
                currentPathNode = previousNodes[currentPathNode];
            }

            // Если путь не начинается с источника, значит пути нет
            if (path.FirstOrDefault() != source)
                return new List<Node>(); // Путь отсутствует

            return path;
        }

        private void FindOptimalPathButton_Click(object sender, RoutedEventArgs e)
        {
            ClearOptimalPathButton_Click();
            var optimalPathWindow = new OptimalPathWindow(Nodes);
            optimalPathWindow.UpdateTheme(Properties.Settings.Default.currentTheme);
            optimalPathWindow.Owner = this;

            if (optimalPathWindow.ShowDialog() == true)
            {
                var sourceNode = optimalPathWindow.SelectedSource;
                var sinkNode = optimalPathWindow.SelectedSink;

                var optimalPath = GetOptimalPath(sourceNode, sinkNode);

                if (optimalPath.Count > 0)
                {
                    // Выделяем путь на графе
                    HighlightPathOnGraph(optimalPath);
                    MessageBox.Show("Оптимальный маршрут найден и выделен на графе.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("Маршрут не найден.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void HighlightPathOnGraph(List<Node> optimalPath)
        {
            // Сбрасываем выделение всех соединений
            foreach (var connection in Connections)
            {
                connection.Highlighted = false;
            }

            // Проходим по пути из узлов и находим соответствующие соединения
            for (int i = 0; i < optimalPath.Count - 1; i++)
            {
                var current = optimalPath[i];
                var next = optimalPath[i + 1];

                // Находим соединение между текущим и следующим узлом
                var connection = Connections.FirstOrDefault(c =>
                    (c.Node1 == current && c.Node2 == next) ||
                    (c.Node1 == next && c.Node2 == current));

                if (connection != null)
                {
                    connection.Highlighted = true;
                }
            }
            Network.RefreshNetwork(Nodes, Connections, false);
            Connections = Network.Connections;
            Nodes = Network.Nodes;
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

        private void ClearOptimalPathButton_Click(object? sender = null, RoutedEventArgs? e = null)
        {
            foreach (var connection in Connections)
            {
                connection.Highlighted = false;
            }
            Network.RefreshNetwork(Nodes, Connections, false);
            Connections = Network.Connections;
            Nodes = Network.Nodes;
        }
    }
}
