using ClosedXML.Excel;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;

namespace comp_netwrks_course_work
{
    public partial class SimulationWindow : Window
    {
        private Simulator simulator;
        private Random random = new Random();
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

        public MessageConnectionType MessageConnectionType
        {
            get => default;
            set
            {
            }
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
                    int max_error = ValidateNumericInput(ErrorTextBox.Text, 5, 100, "Error maximum count");

                    MessageConnectionType connectionType = RandomConnectionTypeCheckBox.IsChecked == true ? GetRandomConnectionType() : ValidateConnectionType();

                    var result = simulator.Simulate(
                        startNode,
                        endNode,
                        connectionType,
                        servicePacketVolume,
                        infoPacketVolume,
                        infoPacketCount,
                        max_error
                    );

                    if (result.Time != 0)
                        simulationResults.Add(result);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExportSimulationResultsToExcel(ObservableCollection<ResultSimulator> results, string filePath)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Simulation Results");

            var properties = typeof(ResultSimulator).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                worksheet.Cell(1, i + 1).Value = properties[i].Name;
            }

            for (int rowIndex = 0; rowIndex < results.Count; rowIndex++)
            {
                var result = results[rowIndex];
                for (int colIndex = 0; colIndex < properties.Length; colIndex++)
                {
                    var res = properties[colIndex].GetValue(result);
                    worksheet.Cell(rowIndex + 2, colIndex + 1).Value = res == null ? "0" : res.ToString();
                }
            }
            try
            {
                workbook.SaveAs(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Cannot save file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Debug.WriteLine(ex.Message);
                return;
            }
            MessageBox.Show("Save done", "Info", MessageBoxButton.OK);
        }



        private void SaveButton_Click(object sender, RoutedEventArgs e) 
            => ExportSimulationResultsToExcel(simulationResults, "savedhistory.xlsx");

        private Node GetRandomNode()
            => networkAnalyzer.Nodes[random.Next(networkAnalyzer.Nodes.Count)];

        private MessageConnectionType GetRandomConnectionType() 
            => random.Next(2) == 0 ? MessageConnectionType.TCP : MessageConnectionType.UDP;

        private Node ValidateNodeSelection(object selectedItem, string fieldName)
        {
            if (selectedItem is Node node)
                return node;

            throw new ArgumentException($"{fieldName} must be selected.");
        }

        private MessageConnectionType ValidateConnectionType() 
            => ConnectionTypeComboBox.SelectedIndex switch
        {
            0 => MessageConnectionType.TCP,
            1 => MessageConnectionType.UDP,
            _ => throw new InvalidOperationException("Invalid Connection Type")
        };

        private int ValidateNumericInput(string input, int min, int max, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException($"{fieldName} cannot be empty.");

            if (input.Trim().ToLower() == "r")
                return random.Next(min, max + 1);

            if (int.TryParse(input, out int value))
            {
                if (value < min || value > max)
                    throw new ArgumentOutOfRangeException($"{fieldName} must be between {min} and {max}.");

                return value;
            }

            throw new ArgumentException($"{fieldName} must be a valid number.");
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
            => simulationResults.Clear();
    }
}