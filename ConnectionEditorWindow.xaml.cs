using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public partial class ConnectionEditorWindow : Window
    {
        private readonly List<Node> _availableNodes;

        public Connection? ResultConnection { get; private set; }

        public ConnectionEditorWindow(List<Node> availableNodes, Connection? existingConnection = null)
        {
            InitializeComponent();
            _availableNodes = availableNodes;

            // Заполнение ComboBox для выбора узлов
            Node1ComboBox.ItemsSource = _availableNodes;
            Node2ComboBox.ItemsSource = _availableNodes;
            Node1ComboBox.DisplayMemberPath = "Number"; // Отображаем номер узла
            Node2ComboBox.DisplayMemberPath = "Number";

            // Заполнение ComboBox для ConnectionType
            ConnectionTypeComboBox.ItemsSource = Enum.GetValues(typeof(ConnectionType));

            // Если редактируем существующую связь
            if (existingConnection != null)
            {
                Node1ComboBox.SelectedItem = existingConnection.Node1;
                Node2ComboBox.SelectedItem = existingConnection.Node2;
                WeightTextBox.Text = existingConnection.Weight.ToString();
                ConnectionTypeComboBox.SelectedItem = existingConnection.Type;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка выбора узлов
            if (Node1ComboBox.SelectedItem is not Node node1 || Node2ComboBox.SelectedItem is not Node node2)
            {
                MessageBox.Show("Please select both Node 1 and Node 2.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Узлы не могут быть одинаковыми
            if (node1 == node2)
            {
                MessageBox.Show("Node 1 and Node 2 must be different.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка корректности веса
            if (!int.TryParse(WeightTextBox.Text, out int weight) || weight <= 0)
            {
                MessageBox.Show("Weight must be a positive integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка выбора типа соединения
            if (ConnectionTypeComboBox.SelectedItem is not ConnectionType connectionType)
            {
                MessageBox.Show("Please select a Connection Type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Создание новой или изменённой связи
            ResultConnection = new Connection(node1, node2, weight, connectionType);

            DialogResult = true; // Указываем, что данные сохранены
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Закрываем окно без сохранения
            Close();
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
