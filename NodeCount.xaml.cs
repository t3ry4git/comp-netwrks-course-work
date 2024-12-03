using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for NodeCount.xaml
    /// </summary>
    public partial class NodeCount : Window
    {
        private readonly bool _isInitializing = true;
        public NodeCount()
        {
            InitializeComponent();
            Textik.Text = Properties.Settings.Default.NodeCount;
            _isInitializing = false;
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$"); // Разрешает только цифры
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                Properties.Settings.Default.NodeCount = Textik.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
