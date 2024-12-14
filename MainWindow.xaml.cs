using System.Diagnostics;
using System.Windows;

namespace comp_netwrks_course_work
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void OnNetworkSimulationClick(object sender, RoutedEventArgs e)
        {
            bool dontclose = false;
            var graphWindow = new GraphWindow(dontclose);
            NetworkAnalyzer network = new(Weights,
                                          Cons,
                                          MinSatteliteCount,
                                          NodeCount,
                                          AVG,
                                          ERROR,
                                          graphWindow);
            var contolWindow = new ConnectionManipulator(network,graphWindow);

            graphWindow.DrawGraph(network.Nodes, network.Connections);

            Visibility = Visibility.Hidden;

            graphWindow.Show();
            contolWindow.Show();

            graphWindow.Closed += (s, args) =>
            {
                Visibility = Visibility.Visible;
                contolWindow.Close();
            };
            contolWindow.Closed += (s, args) =>
            {
                Visibility = Visibility.Visible;
                graphWindow.Close();
            };
        }

        private static readonly char[] separator = [' ', '\n', '\r', '\t'];

        public static List<int> Weights
        {
            get
            {
                try
                {
                    return Properties.Settings.Default.Weights
            .Split(separator,
                   StringSplitOptions.RemoveEmptyEntries)
            .Select(item =>
            {
                if (int.TryParse(item, out int result))
                    return result;
                else
                    return (int?)null;
            })
            .Where(x => x.HasValue)
            .Select(x => x != null ? x.Value : 0)
            .ToList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when reading text: {ex.Message}");
                    Properties.Settings.Default.Weights = "2 4 6 8 10 12 14 16 18 20 22 24";
                    Properties.Settings.Default.Save();
                    return [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24];
                }
            }
        }

        public static List<ConnectionType> Cons 
            => ConnectionTypeConverter.FromString(Properties.Settings.Default.ConnectionType) switch
        {
            ConnectionType.Custom => ConnectionTypeParser.Parse(Properties.Settings.Default.Connections),
            ConnectionType.Duplex => [ConnectionType.Duplex, ConnectionType.Duplex],
            ConnectionType.HalfDuplex => [ConnectionType.HalfDuplex, ConnectionType.HalfDuplex],
            _ => [ConnectionType.Random, ConnectionType.Random]
        };

        public static int MinSatteliteCount
        {
            get
            {
                Debug.WriteLine(Properties.Settings.Default.MinSatelliteCount);
                try
                {
                    if (int.TryParse(Properties.Settings.Default.MinSatelliteCount, out int result))
                        return result; 
                    else
                        return 2;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when reading text: {ex.Message}");
                    Properties.Settings.Default.MinSatelliteCount = "2";
                    Properties.Settings.Default.Save();
                    return 2;
                }
            }
        }

        public static int NodeCount
        {
            get
            {
                try
                {
                    if (int.TryParse(Properties.Settings.Default.NodeCount, out int result))
                        return result; 
                    else
                        return 20;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when reading text: {ex.Message}");
                    Properties.Settings.Default.NodeCount = "20";
                    Properties.Settings.Default.Save();
                    return 20;
                }
            }
        }

        public static List<double> ERROR
        {
            get
            {
                try
                {
                    string? errorString = Properties.Settings.Default.Error;
                    if (string.IsNullOrWhiteSpace(errorString))
                    {
                        errorString = "0.1 0.2 0.3";
                        Properties.Settings.Default.Error = errorString;
                        Properties.Settings.Default.Save();
                    }

                    List<double> doubleList = errorString
                        .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => double.TryParse(x, System.Globalization.NumberStyles.Any,
                            System.Globalization.CultureInfo.InvariantCulture, out _))
                        .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture))
                        .ToList();
                    return doubleList.Count > 0 ? doubleList : new List<double> { 0.1, 0.2, 0.3 };
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when reading text: {ex.Message}");
                    Properties.Settings.Default.Error = "0.1 0.2 0.3";
                    Properties.Settings.Default.Save();
                    return new List<double> { 0.1, 0.2, 0.3 };
                }
            }
        }


        public static float AVG
        {
            get
            {
                try
                {
                    if (float.TryParse(Properties.Settings.Default.AVGCon, out float result))
                        return result; 
                    else
                        return 3.0f; 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when reading text: {ex.Message}");
                    Properties.Settings.Default.AVGCon = "3.0";
                    Properties.Settings.Default.Save();
                    return 3.0f;
                }
            }
        }



        private void OnDarkThemeClick(object sender, RoutedEventArgs e) 
            => ThemeSupport.ApplyTheme("Themes/DarkTheme.xaml", this);

        private void OnLightThemeClick(object sender, RoutedEventArgs e) 
            => ThemeSupport.ApplyTheme("Themes/LightTheme.xaml", this);

        private void OnWeightSettings(object sender, RoutedEventArgs e)
        {
            var weightSet = new SetWeight();
            Visibility = Visibility.Hidden;
            weightSet.Show();
            weightSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }

        private void OnConnectSettings(object sender, RoutedEventArgs e)
        {
            var conSet = new SetConnection();
            Visibility = Visibility.Hidden;
            conSet.Show();
            conSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }
        private void OnNodeCount(object sender, RoutedEventArgs e)
        {
            var nodeSet = new SetNodeCount();
            Visibility = Visibility.Hidden;
            nodeSet.Show();
            nodeSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }
        private void OnNodeConCount(object sender, RoutedEventArgs e)
        {
            var avgSet = new SetAVG();
            Visibility = Visibility.Hidden;
            avgSet.Show();
            avgSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }
        private void OnErrorChance(object sender, RoutedEventArgs e)
        {
            var errorchance = new SetError();
            Visibility = Visibility.Hidden;
            errorchance.Show();
            errorchance.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }

    }
}