using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using MessageBox = System.Windows.MessageBox;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for SetWeight.xaml
    /// </summary>
    public partial class SetWeight : Window
    {
        private bool _isInitializing = true;
        public SetWeight()
        {
            InitializeComponent();
            randomly.IsChecked = Properties.Settings.Default.randomWeights;
            // Подписываемся на событие PreviewTextInput для RichTextBox
            richyWeights.PreviewTextInput += RichTextBox_PreviewTextInput;

            // Убираем возможность вставки неподходящего текста
            DataObject.AddPastingHandler(richyWeights, OnPaste);
            SetTextFromList(richyWeights);
            _isInitializing = false;
        }


        private void SetTextFromList(RichTextBox richTextBox)
        {
            // Устанавливаем текст в RichTextBox
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            textRange.Text = Properties.Settings.Default.Weights;
        }
        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, соответствует ли вводимый текст числам или пробелу
            e.Handled = !Regex.IsMatch(e.Text, @"^[\d\s]+$");
        }

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string? pastedText = e.DataObject.GetData(DataFormats.Text) as string;
                if (pastedText == null)
                {
                    pastedText = string.Empty;
                }
                // Если вставленный текст содержит не только цифры и пробелы, отменяем вставку
                if (!Regex.IsMatch(pastedText, @"^[\d\s]+$"))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (randomly.IsChecked != null)
            {
                Properties.Settings.Default.randomWeights = (bool)randomly.IsChecked;
                Properties.Settings.Default.Save();
            }
            else
            {
                Properties.Settings.Default.randomWeights = true;
                Properties.Settings.Default.Save();
            }
        }

        private void RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInitializing) { }
            else
            {

                // Извлекаем текст из RichTextBox
                string text = new TextRange(richyWeights.Document.ContentStart, richyWeights.Document.ContentEnd).Text;
                Properties.Settings.Default.Weights = text;
                Properties.Settings.Default.Save();

            }
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
    }
}
