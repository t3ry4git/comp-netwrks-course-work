using System;
using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public partial class NodeEditorWindow : Window
    {
        public Node? ResultNode { get; private set; }

        private readonly List<Node> _existingNodes;

        public NodeEditorWindow(List<Node> existingNodes, Node? existingNode = null)
        {
            InitializeComponent();
            _existingNodes = existingNodes;

            // Заполнение ComboBox значениями NodeType
            NodeTypeComboBox.ItemsSource = Enum.GetValues(typeof(NodeType));

            // Если редактируем существующую Node
            if (existingNode != null)
            {
                NumberTextBox.Text = existingNode.Number.ToString();
                NodeTypeComboBox.SelectedItem = existingNode.Type;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Проверка корректности ввода
            if (!int.TryParse(NumberTextBox.Text, out int number))
            {
                MessageBox.Show("Number must be a valid integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NodeTypeComboBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a Node Type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Проверка уникальности Number
            bool isDuplicate = _existingNodes.Any(node => node.Number == number);
            if (isDuplicate)
            {
                MessageBox.Show("Number must be unique. A node with this number already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            // Создание или обновление Node
            var nodeType = (NodeType)NodeTypeComboBox.SelectedItem;
            ResultNode = new Node(number, nodeType);

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
