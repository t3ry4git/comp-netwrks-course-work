using System.Diagnostics;
using System.Windows;


namespace comp_netwrks_course_work
{
    public partial class TimedFordFulkersonWindow : Window
    {
        public Node? SelectedSource { get; private set; }
        public Node? SelectedSink { get; private set; }
        public int Iter { get; private set; }
        private NetworkAnalyzer Network;
        private readonly List<NetworkAnalyzer> NetworkBackup;
        private List<int> summies = [];

        public TimedFordFulkersonWindow(NetworkAnalyzer network)
        {
            InitializeComponent();
            Network = network;
            NetworkBackup = [];
            UpdateSinkSource();
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }


        private void UpdateSinkSource()
        {
            SelectedSink = Network.Nodes.LastOrDefault();
            SelectedSource = Network.Nodes.FirstOrDefault();
            SourceComboBox.ItemsSource = Network.Nodes;
            SourceComboBox.SelectedItem = SelectedSource;
            SinkComboBox.ItemsSource = Network.Nodes;
            SinkComboBox.SelectedItem = SelectedSink;
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (SourceComboBox.SelectedItem is not Node
                || SinkComboBox.SelectedItem is not Node)
            {
                MessageBox.Show("Select source and sink", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (Iter > 0)
            {
                Iter--;
                Step.Content = Iter.ToString();
                if (NetworkBackup.Count > 0)
                {
                        summies.Remove(summies.Last());
                        int s = 0;
                        foreach (var sum in summies)
                            s += sum;

                        Summy.Content = s.ToString();
                        Network = NetworkBackup.Last();
                        NetworkBackup.Remove(NetworkBackup.Last());
                    Network.Redraw();
                    UpdateSinkSource();
                }
            }

        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {

            if (SourceComboBox.SelectedItem is not Node sourceNode
                || SinkComboBox.SelectedItem is not Node sinkNode)
            {
                MessageBox.Show("Select source and sink", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NetworkAnalyzer nc = Network.Clone();

            var (found, connections, flow) = FordFulkerson.GetData(sourceNode, sinkNode, 0);

            if (found == true)
            {
                NetworkBackup.Add(nc);
                Iter++;
                Step.Content = Iter.ToString();
                MessageBox.Show($"Current max flow:{flow}", "Info", MessageBoxButton.OK);
                foreach (var con in connections)
                {
                    con.Highlighted = true;
                    con.WeightUsed += flow;
                    con.ResetFlow();
                    Debug.WriteLine(con);
                }
                
                summies.Add(flow);
                int s = 0;
                foreach(var sum in summies)
                    s += sum;

                Summy.Content = s.ToString();   
                Network.Redraw();
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
            Iter = 0;
            Step.Content =Iter.ToString();
            summies = [];
            Summy.Content = "0";
            NetworkBackup.Clear();
            foreach (var cons in Network.Connections)
            {
                cons.ResetFlow();
                cons.WeightUsed = 0;
                cons.Highlighted = false;
            }
            Network.Redraw();
            UpdateSinkSource();
        }

        public static void Update(TimedFordFulkersonWindow window, NetworkAnalyzer network)
        {
            window.Iter = 0;
            window.Step.Content = window.Step.ToString();
            window.summies = [];
            window.Summy.Content = "0";
            window.NetworkBackup.Clear();
            window.Network = network;
            window.Network.Redraw();
            window.UpdateSinkSource();
        }
    }
}
