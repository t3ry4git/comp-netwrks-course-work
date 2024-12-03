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

            var network = new NetworkAnalyzer(GetWeights());
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
        .Select(x => x.Value) // Преобразуем обратно в int
        .ToList();
                return numbers;
            }
            catch (Exception ex)
            {
                // Логируем ошибку или уведомляем пользователя
                MessageBox.Show($"Ошибка при обработке текста: {ex.Message}");
                Properties.Settings.Default.Weights = "2 4 6 8 10 12 14 16 18 20 22 24";
                Properties.Settings.Default.Save();
                return new List<int>() { 2, 4, 6, 8, 10, 12, 14, 16, 18, 20, 22, 24 };
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

        }
        private void OnNodeCount(object sender, RoutedEventArgs e)
        {

        }
        private void OnNodeConCount(object sender, RoutedEventArgs e)
        { 
        
        }


        private void ApplyTheme(string themePath)
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