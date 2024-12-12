using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace comp_netwrks_course_work
{
    public partial class SetAVG : Window
    {
        private readonly bool _isInitializing = true;
        public SetAVG()
        {
            InitializeComponent();
            Textik.Text = Properties.Settings.Default.AVGCon;
            _isInitializing = false;
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool isInputValid = Regex.IsMatch(e.Text, @"^[\d\.]+$");
            bool isDotAllowed = !(e.Text == "." && Textik.Text.Contains('.'));
            e.Handled = !(isInputValid && isDotAllowed);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                Properties.Settings.Default.AVGCon = Textik.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
