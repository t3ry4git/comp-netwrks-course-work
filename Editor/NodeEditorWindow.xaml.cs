using System.Windows;

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
            NodeTypeComboBox.ItemsSource = Enum.GetValues(typeof(NodeType));
            if (existingNode != null)
            {
                NumberTextBox.Text = existingNode.Number.ToString();
                NodeTypeComboBox.SelectedItem = existingNode.Type;
            }
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
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
            if (_existingNodes.Any(node => node.Number == number))
            {
                MessageBox.Show("Number must be unique. A node with this number already exists.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var nodeType = (NodeType)NodeTypeComboBox.SelectedItem;
            ResultNode = new Node(number, nodeType);
            DialogResult = true;
            Close();
        }


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
