using System.Windows;

namespace comp_netwrks_course_work
{
    public partial class OptimalPathWindow : Window
    {
        public Node? SelectedSource { get; private set; }
        public Node? SelectedSink { get; private set; }

        public OptimalPathWindow(List<Node> nodes)
        {
            InitializeComponent();
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
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
                    MessageBox.Show("Source and sink have to be different", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SelectedSource = sourceNode;
                SelectedSink = sinkNode;

                DialogResult = true;
                Close();
            }
            else
                MessageBox.Show("Select both, sink and source", "Info", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
