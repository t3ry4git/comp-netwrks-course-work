using Microsoft.Msagl.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string current = "Themes/LightTheme.xaml";
        public MainWindow()
        {
            InitializeComponent();
            ApplyTheme(current);
            this.Background = (Brush)Application.Current.Resources["BackgroundColor"];
            this.Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void OnNetworkSimulationClick(object sender, RoutedEventArgs e)
        {
            string mode = HalfDuplexRadio.IsChecked == true ? "Напівдуплекс" : "Дуплекс";

            // Создание окна визуализации графа
            var graphWindow = new GraphWindow();

            graphWindow.UpdateTheme(current);

            // Скрыть основное окно
            this.Visibility = Visibility.Hidden;

            // Показать окно графа
            graphWindow.Show();

            // Восстановить видимость основного окна, когда графовое окно закрыто
            graphWindow.Closed += (s, args) => this.Visibility = Visibility.Visible;
            OutputTextBox.Text = $"Режим: {mode}\nАнализ топологии выполнен.";
        }



        private void OnTrafficAnalysisClick(object sender, RoutedEventArgs e)
        {
            // TODO: Добавить логику анализа трафика
            OutputTextBox.Text = "Анализ трафика выполнен.";
        }

        private void OnDarkThemeClick(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Themes/DarkTheme.xaml");
            current = "Themes/DarkTheme.xaml";
            this.Background = (Brush)Application.Current.Resources["BackgroundColor"];
            this.Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void OnLightThemeClick(object sender, RoutedEventArgs e)
        {
            ApplyTheme("Themes/LightTheme.xaml");
            current = "Themes/LightTheme.xaml";
            this.Background = (Brush)Application.Current.Resources["BackgroundColor"];
            this.Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }

        private void ApplyTheme(string themePath)
        {
            var theme = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(theme);
        }


    }
}