using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public partial class TimedFordFulkersonWindow : Window
    {
        public Node? SelectedSource { get; private set; }
        public Node? SelectedSink { get; private set; }
        private FordFulkerson ford;

        public int iter { get; private set; }

        private NetworkAnalyzer Network;
        private List<NetworkAnalyzer> NetworkBackup;
        private List<int> summies = new List<int>();

        public TimedFordFulkersonWindow(NetworkAnalyzer network)
        {
            InitializeComponent();
            ford = new FordFulkerson();
            Network = network;
            SelectedSink = Network.Nodes.LastOrDefault();
            SelectedSource = Network.Nodes.FirstOrDefault();
            NetworkBackup = new List<NetworkAnalyzer>();

            // Инициализация ComboBox
            SourceComboBox.ItemsSource = Network.Nodes;
            SourceComboBox.SelectedItem = SelectedSource;
            SinkComboBox.ItemsSource = Network.Nodes;
            SinkComboBox.SelectedItem = SelectedSink;
            
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


        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (SourceComboBox.SelectedItem is not Node sourceNode ||
    SinkComboBox.SelectedItem is not Node sinkNode)
            {
                MessageBox.Show("Select source and sink", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (iter > 0)
            {
                iter--;
                Step.Content = iter.ToString();
                if (NetworkBackup.Count > 0)
                {
                    summies.Remove(summies.Last());
                    int s = 0;
                    foreach (var sum in summies)
                        s += sum;

                    Summy.Content = s.ToString();
                    Network = NetworkBackup.Last();
                    NetworkBackup.Remove(NetworkBackup.Last());
                    Network.RefreshNetwork(Network.Nodes, Network.Connections, false);
                    SelectedSink = Network.Nodes.LastOrDefault();
                    SelectedSource = Network.Nodes.FirstOrDefault();
                    SourceComboBox.ItemsSource = Network.Nodes;
                    SourceComboBox.SelectedItem = SelectedSource;
                    SinkComboBox.ItemsSource = Network.Nodes;
                    SinkComboBox.SelectedItem = SelectedSink;
                }
            }

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

            if (SourceComboBox.SelectedItem is not Node sourceNode ||
SinkComboBox.SelectedItem is not Node sinkNode)
            {
                MessageBox.Show("Select source and sink", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            NetworkAnalyzer nc = Network.Clone();
            var data = FordFulkerson.GetData(sourceNode, sinkNode, 0);
            if (data.found == true)
            {
                NetworkBackup.Add(nc);
                iter++;
                Step.Content = iter.ToString();
                MessageBox.Show($"Current max flow:{data.flow}", "Info", MessageBoxButton.OK);
                foreach (var con in data.connections)
                {
                    con.Highlighted = true;
                    con.WeightUsed += data.flow;
                    con.ResetFlow();
                    Debug.WriteLine(con);
                }
                
                summies.Add(data.flow);
                int s = 0;
                foreach(var sum in summies)
                    s += sum;

                Summy.Content = s.ToString();   
                Network.RefreshNetwork(Network.Nodes, Network.Connections, false);

                return;
            }
            else
            {
                MessageBox.Show("Cannot found new route", "Info", MessageBoxButton.OK);
                return;
            }

        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkBackup.Count > 0)
                Network = NetworkBackup[0];
            iter = 0;
            Step.Content = iter.ToString();
            summies = new List<int>();
            Summy.Content = "0";
            NetworkBackup.Clear();
            Network.RefreshNetwork(Network.Nodes, Network.Connections, false);
            SelectedSink = Network.Nodes.LastOrDefault();
            SelectedSource = Network.Nodes.FirstOrDefault();
            SourceComboBox.ItemsSource = Network.Nodes;
            SourceComboBox.SelectedItem = SelectedSource;
            SinkComboBox.ItemsSource = Network.Nodes;
            SinkComboBox.SelectedItem = SelectedSink;
        }
    }
}
