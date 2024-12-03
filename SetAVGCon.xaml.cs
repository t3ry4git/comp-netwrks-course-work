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
    /// Interaction logic for SetAVGCon.xaml
    /// </summary>
    public partial class SetAVGCon : Window
    {
        private readonly bool _isInitializing = true;
        public SetAVGCon()
        {
            InitializeComponent();
            Textik.Text = Properties.Settings.Default.AVGCon;
            _isInitializing = false;
        }


        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var textBox = sender as TextBox;

            // Проверяем: цифры или точка
            bool isInputValid = Regex.IsMatch(e.Text, @"^[\d\.]+$");

            // Убедиться, что точка не повторяется
            bool isDotAllowed = !(e.Text == "." && Textik.Text.Contains("."));

            e.Handled = !(isInputValid && isDotAllowed);
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
                Properties.Settings.Default.AVGCon = Textik.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
