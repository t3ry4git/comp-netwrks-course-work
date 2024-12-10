using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public partial class OptimalPathWindow : Window
    {
        public Node SelectedSource { get; private set; }
        public Node SelectedSink { get; private set; }
        public bool IsConfirmed { get; private set; } = false;

        public OptimalPathWindow(List<Node> nodes)
        {
            InitializeComponent();

            // Инициализируем ComboBox списком узлов
            SourceComboBox.ItemsSource = nodes;
            SinkComboBox.ItemsSource = nodes;
        }

        private void FindPathButton_Click(object sender, RoutedEventArgs e)
        {
            if (SourceComboBox.SelectedItem is Node sourceNode &&
                SinkComboBox.SelectedItem is Node sinkNode)
            {
                if (sourceNode == sinkNode)
                {
                    MessageBox.Show("Исходный и конечный узлы должны быть разными.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SelectedSource = sourceNode;
                SelectedSink = sinkNode;

                DialogResult = true; // Установить успешный результат
                Close();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите оба узла.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Установить отрицательный результат
            Close();
        }


        public void UpdateTheme(string themePath)
        {
            var theme = new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Add(theme);
            Background = (Brush)Application.Current.Resources["BackgroundColor"];
            Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }
    }
}
