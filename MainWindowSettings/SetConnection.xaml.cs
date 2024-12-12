using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace comp_netwrks_course_work
{
    public partial class SetConnection : Window
    {
        private readonly bool _isInitializing = true;
        public SetConnection()
        {
            InitializeComponent();
            richyNet.PreviewTextInput += RichTextBox_Connections;
            switch (ConnectionTypeConverter.FromString(Properties.Settings.Default.ConnectionType))
            {
                case ConnectionType.HalfDuplex:
                    Hbutton.IsChecked = true;
                    break;
                case ConnectionType.Duplex:
                    Dbutton.IsChecked = true;
                    break;
                case ConnectionType.Custom:
                    Cbutton.IsChecked = true;
                    break;
                case ConnectionType.Random:
                    Rbutton.IsChecked = true;
                    break;
            }

            MinCount.Text = Properties.Settings.Default.MinSatelliteCount;
            DataObject.AddPastingHandler(richyNet, OnPaste);
            SetTextFromList(richyNet);
            _isInitializing = false;
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private void RichTextBox_Connections(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^[dhs\s]+$", RegexOptions.IgnoreCase);

        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string? pastedText = e.DataObject.GetData(DataFormats.Text) as string;
                pastedText ??= string.Empty;
                if (!Regex.IsMatch(pastedText, @"^[dhs\s]+$", RegexOptions.IgnoreCase))
                    e.CancelCommand();
            }
            else
                e.CancelCommand();
        }
        private static void SetTextFromList(RichTextBox richTextBox) =>
            _ = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd)
            {
                Text = Properties.Settings.Default.Connections
            };

        private void RichTextBox_ConnectionsChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                Properties.Settings.Default.Connections =
                    new TextRange(richyNet.Document.ContentStart, richyNet.Document.ContentEnd).Text;
                Properties.Settings.Default.Save();
            }
        }

        private void RadioButton_HalfDuplex(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.HalfDuplex);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Duplex(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Duplex);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Custom(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Custom);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Random(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Random);
            Properties.Settings.Default.Save();
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
            => e.Handled = !Regex.IsMatch(e.Text, @"^\d+$");

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isInitializing)
            {
                Properties.Settings.Default.MinSatelliteCount = MinCount.Text;
                Properties.Settings.Default.Save();
            }
        }
    }
}
