using System.Diagnostics;
using System.Windows;
using System.Windows.Media;


namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme(Properties.Settings.Default.currentTheme);
            Background = (Brush)Application.Current.Resources["BackgroundColor"];
            Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void OnNetworkSimulationClick(object sender, RoutedEventArgs e)
        {

            // Создание окна визуализации графа
            var graphWindow = new GraphWindow();
            var satCount = GetMinSatteliteCount();
            var nodeCount = GetNodeCount();
            var avg = GetAVG();
            NetworkAnalyzer network;
                network = new NetworkAnalyzer(GetWeights(), GetCons(), satCount, nodeCount, avg);
            graphWindow.DrawGraph(network.Nodes, network.Connections);
            graphWindow.UpdateTheme(Properties.Settings.Default.currentTheme);

            // Скрыть основное окно
            Visibility = Visibility.Hidden;

            // Показать окно графа
            graphWindow.Show();

            // Восстановить видимость основного окна, когда графовое окно закрыто
            graphWindow.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }

        public List<int> GetWeights()
        {
            try
            {
                List<int> numbers = Properties.Settings.Default.Weights
        .Split(new[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries) // Убираем пустые строки
        .Select(item =>
        {
            if (int.TryParse(item, out int result))
            {
                return result; // Преобразуем в число
            }
            else
            {
                return (int?)null; // Возвращаем null для нечисловых значений
            }
        })
        .Where(x => x.HasValue) // Убираем null значения
        .Select(x => x != null ? x.Value: 0) // Преобразуем обратно в int
        .ToList();
                return numbers;
            }
            catch (Exception ex)
            {
                // Логируем ошибку или уведомляем пользователя
                MessageBox.Show($"Ошибка при обработке текста: {ex.Message}");
                Properties.Settings.Default.Weights = "2 4 6 8 10 12 14 16 18 20 22 24";
                Properties.Settings.Default.Save();
                return [2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24];
            }
        }

        public static List<ConnectionType> GetCons()
        {
            if(ConnectionTypeConverter.FromString(Properties.Settings.Default.ConnectionType) == ConnectionType.Custom)
                return ConnectionTypeParser.Parse(Properties.Settings.Default.Connections);
            else if(ConnectionTypeConverter.FromString (Properties.Settings.Default.ConnectionType) == ConnectionType.Duplex)
                return [ConnectionType.Duplex, ConnectionType.Duplex];
            else if (ConnectionTypeConverter.FromString(Properties.Settings.Default.ConnectionType) == ConnectionType.HalfDuplex)
                return [ConnectionType.HalfDuplex, ConnectionType.HalfDuplex];
            else
                return [ConnectionType.Random, ConnectionType.Random];
        }

        public int GetMinSatteliteCount()
        {
            Debug.WriteLine(Properties.Settings.Default.MinSatelliteCount);
            try
            {
                if (int.TryParse(Properties.Settings.Default.MinSatelliteCount, out int result))
                {
                    return result; // Преобразуем в число
                }
                else
                {
                    return 2; // Возвращаем null для нечисловых значений
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку или уведомляем пользователя
                MessageBox.Show($"Ошибка при обработке текста: {ex.Message}");
                Properties.Settings.Default.MinSatelliteCount = "2";
                Properties.Settings.Default.Save();
                return 2;
            }
        }
        public int GetNodeCount()
        {
            Debug.WriteLine(Properties.Settings.Default.NodeCount);
            try
            {
                if (int.TryParse(Properties.Settings.Default.NodeCount, out int result))
                {
                    return result; // Преобразуем в число
                }
                else
                {
                    return 20; // Возвращаем null для нечисловых значений
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку или уведомляем пользователя
                MessageBox.Show($"Ошибка при обработке текста: {ex.Message}");
                Properties.Settings.Default.NodeCount = "20";
                Properties.Settings.Default.Save();
                return 20;
            }
        }

        public float GetAVG()
        {
            Debug.WriteLine(Properties.Settings.Default.AVGCon);
            try
            {
                if (float.TryParse(Properties.Settings.Default.AVGCon, out float result))
                {
                    return result; // Преобразуем в число с плавающей точкой
                }
                else
                {
                    return 3.0f; // Возвращаем значение по умолчанию для некорректных данных
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку или уведомляем пользователя
                MessageBox.Show($"Ошибка при обработке текста: {ex.Message}");
                Properties.Settings.Default.AVGCon = "3.0";
                Properties.Settings.Default.Save();
                return 3.0f;
            }
        }


        private void OnTrafficAnalysisClick(object sender, RoutedEventArgs e)
        {
            // TODO: Добавить логику анализа трафика
            OutputTextBox.Text = "Анализ трафика выполнен.";
        }

        private void OnDarkThemeClick(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Themes/DarkTheme.xaml");
            Background = (Brush)Application.Current.Resources["BackgroundColor"];
            Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void OnLightThemeClick(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Themes/LightTheme.xaml");
            Background = (Brush)Application.Current.Resources["BackgroundColor"];
            Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void OnWeightSettings(object sender, RoutedEventArgs e)
        {
            var weightSet = new SetWeight();
            Visibility = Visibility.Hidden;
            weightSet.UpdateTheme(Properties.Settings.Default.currentTheme);
            weightSet.Show();
            weightSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }

        private void OnConnectSettings(object sender, RoutedEventArgs e)
        {
            var conSet = new SetCon();
            Visibility = Visibility.Hidden;
            conSet.UpdateTheme(Properties.Settings.Default.currentTheme);
            conSet.Show();
            conSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }
        private void OnNodeCount(object sender, RoutedEventArgs e)
        {
            var nodeSet = new NodeCount();
            Visibility = Visibility.Hidden;
            nodeSet.UpdateTheme(Properties.Settings.Default.currentTheme);
            nodeSet.Show();
            nodeSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }
        private void OnNodeConCount(object sender, RoutedEventArgs e)
        {
            var avgSet = new SetAVGCon();
            Visibility = Visibility.Hidden;
            avgSet.UpdateTheme(Properties.Settings.Default.currentTheme);
            avgSet.Show();
            avgSet.Closed += (s, args) => this.Visibility = Visibility.Visible;
        }


        private static void ApplyTheme(string themePath)
        {
            Properties.Settings.Default.currentTheme = themePath;
            Properties.Settings.Default.Save();
            
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            });
        }


    }
}