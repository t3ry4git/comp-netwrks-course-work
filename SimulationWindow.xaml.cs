using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace comp_netwrks_course_work
{
    public partial class SimulationWindow : Window
    {
        private Simulator simulator;
        private NetworkAnalyzer networkAnalyzer;
        private ObservableCollection<ResultSimulator> simulationResults = new ObservableCollection<ResultSimulator>();

        public SimulationWindow(NetworkAnalyzer analyzer)
        {
            InitializeComponent();

            networkAnalyzer = analyzer;
            simulator = new Simulator(networkAnalyzer);

            PopulateNodeDropdowns();
            PacketHistoryDataGrid.ItemsSource = simulationResults;

            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void PopulateNodeDropdowns()
        {
            var nodes = networkAnalyzer.Nodes;
            StartNodeComboBox.ItemsSource = nodes;
            EndNodeComboBox.ItemsSource = nodes;
            StartNodeComboBox.DisplayMemberPath = "Number";
            EndNodeComboBox.DisplayMemberPath = "Number";
        }

        private void SimulateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int messageCount = ValidateNumericInput(MessageCountTextBox.Text, 1, 25, "Message Count");
                for (int i = 0; i < messageCount; i++)
                {
                    Node startNode = RandomStartNodeCheckBox.IsChecked == true ? GetRandomNode() : ValidateNodeSelection(StartNodeComboBox.SelectedItem, "Start Node");
                    Node endNode = RandomEndNodeCheckBox.IsChecked == true ? GetRandomNode() : ValidateNodeSelection(EndNodeComboBox.SelectedItem, "End Node");

                    int infoPacketVolume = ValidateNumericInput(InfoPacketVolumeTextBox.Text, 1, 1000, "Info Packet Volume");
                    int servicePacketVolume = ValidateNumericInput(ServicePacketVolumeTextBox.Text, 1, 1000, "Service Packet Volume");
                    int infoPacketCount = ValidateNumericInput(InfoVolumeTextBox.Text, 1, 100, "Info Packet Count");
                    double errorProbability = ValidateDoubleInput(ErrorProbabilityTextBox.Text, 0.0, 0.3, "Error Probability");

                    MessageConnectionType connectionType = RandomConnectionTypeCheckBox.IsChecked == true ? GetRandomConnectionType() : ValidateConnectionType();

                    var result = simulator.Simulate(
                        startNode,
                        endNode,
                        connectionType,
                        servicePacketVolume,
                        infoPacketVolume,
                        infoPacketCount,
                        errorProbability
                    );

                    // Добавление нового результата
                    simulationResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Логика сохранения данных из DataGrid
            MessageBox.Show("Saving to table is not yet implemented.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private Node GetRandomNode()
        {
            var random = new Random();
            return networkAnalyzer.Nodes[random.Next(networkAnalyzer.Nodes.Count)];
        }

        private MessageConnectionType GetRandomConnectionType()
        {
            var random = new Random();
            return random.Next(2) == 0 ? MessageConnectionType.TCP : MessageConnectionType.UDP;
        }

        private Node ValidateNodeSelection(object selectedItem, string fieldName)
        {
            if (selectedItem is Node node)
                return node;

            throw new ArgumentException($"{fieldName} must be selected.");
        }

        private MessageConnectionType ValidateConnectionType()
        {
            return ConnectionTypeComboBox.SelectedIndex switch
            {
                0 => MessageConnectionType.TCP,
                1 => MessageConnectionType.UDP,
                _ => throw new InvalidOperationException("Invalid Connection Type")
            };
        }

        private int ValidateNumericInput(string input, int min, int max, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"{fieldName} cannot be empty.");

            if (input.Trim().ToLower() == "r")
            {
                var random = new Random();
                return random.Next(min, max + 1);
            }

            if (int.TryParse(input, out int value))
            {
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException($"{fieldName} must be between {min} and {max}.");

                return value;
            }

            throw new ArgumentException($"{fieldName} must be a valid number.");
        }

        private double ValidateDoubleInput(string input, double min, double max, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"{fieldName} cannot be empty.");

            if (input.Trim().ToLower() == "r")
            {
                var random = new Random();
                return (random.NextDouble() * (max - min)) + min;
            }

            if (double.TryParse(input, out double value))
            {
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException($"{fieldName} must be between {min} and {max}.");

                return value;
            }

            throw new ArgumentException($"{fieldName} must be a valid decimal number.");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            simulationResults.Clear();
        }
    }
}