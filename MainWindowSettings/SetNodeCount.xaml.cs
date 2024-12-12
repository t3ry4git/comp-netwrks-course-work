using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace comp_netwrks_course_work
{
    public partial class SetNodeCount : Window
    {
        private readonly bool _isInitializing = true;
        public SetNodeCount()
        {
            InitializeComponent();
            Textik.Text = Properties.Settings.Default.NodeCount;
            _isInitializing = false;
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");

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
