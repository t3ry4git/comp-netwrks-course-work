using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using DataFormats = System.Windows.DataFormats;
using DataObject = System.Windows.DataObject;
using RichTextBox = System.Windows.Controls.RichTextBox;

namespace comp_netwrks_course_work
{

    public partial class SetWeight : Window
    {
        private readonly bool _isInitializing = true;
        public SetWeight()
        {
            InitializeComponent();
            randomly.IsChecked = Properties.Settings.Default.randomWeights;
            richyWeights.PreviewTextInput += RichTextBox_PreviewTextInput;
            DataObject.AddPastingHandler(richyWeights, OnPaste);
            SetTextFromList(richyWeights);
            _isInitializing = false;
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }


        private static void SetTextFromList(RichTextBox richTextBox) =>
            _ = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd)
            {
                Text = Properties.Settings.Default.Weights
            };

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^[\d\s]+$");

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string? pastedText = e.DataObject.GetData(DataFormats.Text) as string;
                pastedText ??= string.Empty;
                if (!Regex.IsMatch(pastedText, @"^[\d\s]+$"))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
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
                string text =
                    new TextRange(richyWeights.Document.ContentStart,
                                  richyWeights.Document.ContentEnd).Text;
                Properties.Settings.Default.Weights = text;
                Properties.Settings.Default.Save();

            }
        }
    }
}
