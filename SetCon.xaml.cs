using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace comp_netwrks_course_work
{
    /// <summary>
    /// Interaction logic for SetCon.xaml
    /// </summary>
    public partial class SetCon : Window
    {
        private readonly bool _isInitializing = true;
        public SetCon()
        {
            InitializeComponent();
            // Подписываемся на событие PreviewTextInput для RichTextBox
            richyNet.PreviewTextInput += RichTextBox_PreviewTextInput;
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
            // Убираем возможность вставки неподходящего текста
            DataObject.AddPastingHandler(richyNet, OnPaste);
            SetTextFromList(richyNet);
            _isInitializing = false;
        }

        private void RichTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Проверяем, соответствует ли вводимый текст разрешённым символам: 'd', 'h', 's' и пробел
            e.Handled = !Regex.IsMatch(e.Text, @"^[dhs\s]+$", RegexOptions.IgnoreCase);
        }
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string? pastedText = e.DataObject.GetData(DataFormats.Text) as string;
                pastedText ??= string.Empty;
                // Если вставленный текст содержит не только цифры и пробелы, отменяем вставку
                if (!Regex.IsMatch(pastedText, @"^[dhs\s]+$", RegexOptions.IgnoreCase))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
        private void SetTextFromList(RichTextBox richTextBox)
        {
            // Устанавливаем текст в RichTextBox
            _ = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd)
            {
                Text = Properties.Settings.Default.Connections
            };
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

        private void RichyNet_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_isInitializing) { }
            else
            {

                // Извлекаем текст из RichTextBox
                string text = new TextRange(richyNet.Document.ContentStart, richyNet.Document.ContentEnd).Text;
                Properties.Settings.Default.Connections = text;
                Properties.Settings.Default.Save();

            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.HalfDuplex);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Duplex);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Checked_2(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Custom);
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Checked_3(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ConnectionType = ConnectionTypeConverter.ToString(ConnectionType.Random);
            Properties.Settings.Default.Save();
        }

        private void NumberInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, @"^\d+$"); // Разрешает только цифры
        }

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
