using System.Windows;

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
            Node1ComboBox.ItemsSource = _availableNodes;
            Node2ComboBox.ItemsSource = _availableNodes;
            Node1ComboBox.DisplayMemberPath = "Number";
            Node2ComboBox.DisplayMemberPath = "Number";
            ConnectionTypeComboBox.ItemsSource = Enum.GetValues(typeof(ConnectionType));

            if (existingConnection != null)
            {
                Node1ComboBox.SelectedItem = existingConnection.Node1;
                Node2ComboBox.SelectedItem = existingConnection.Node2;
                WeightTextBox.Text = existingConnection.Weight.ToString();
                ConnectionTypeComboBox.SelectedItem = existingConnection.Type;
                ErrorTextBox.Text = existingConnection.ChanceOfError.ToString();
            }
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Node1ComboBox.SelectedItem is not Node node1 || Node2ComboBox.SelectedItem is not Node node2)
            {
                MessageBox.Show("Please select both Node 1 and Node 2.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (node1 == node2)
            {
                MessageBox.Show("Node 1 and Node 2 must be different.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!int.TryParse(WeightTextBox.Text, out int weight) || weight <= 0)
            {
                MessageBox.Show("Weight must be a positive integer.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (ConnectionTypeComboBox.SelectedItem is not ConnectionType connectionType)
            {
                MessageBox.Show("Please select a Connection Type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if(!double.TryParse(ErrorTextBox.Text, out double error) || error <= 0)
            {
                MessageBox.Show("Error must be a positive double.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ResultConnection = new Connection(node1, node2, weight, connectionType,error);
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
