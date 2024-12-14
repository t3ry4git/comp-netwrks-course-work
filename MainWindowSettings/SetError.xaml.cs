using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace comp_netwrks_course_work
{
    public partial class SetError : Window
    {
        public SetError()
        {
            InitializeComponent();
            switch (Properties.Settings.Default.ErrorType)
            {
                case "Custom":
                    Cbutton.IsChecked = true;
                    break;
                case "Random":
                    Rbutton.IsChecked = true;
                    break;
            }
            SetTextFromList(richyNet);

            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
        }

        private static void SetTextFromList(RichTextBox richTextBox) =>
            _ = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd)
            {
                Text = Properties.Settings.Default.Error
            };

        private void RadioButton_Custom(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ErrorType = "Custom";
            Properties.Settings.Default.Save();
        }

        private void RadioButton_Random(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.ErrorType = "Random";
            Properties.Settings.Default.Save();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputText = new TextRange(richyNet.Document.ContentStart, richyNet.Document.ContentEnd).Text;

                List<double> doubleList = inputText
                    .Split(new[] { ' ', ',', ';', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries) // Разбиваем по разделителям
                    .Where(x => double.TryParse(x, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out _)) // Фильтруем валидные числа
                    .Select(x => double.Parse(x, System.Globalization.CultureInfo.InvariantCulture)) // Преобразуем в double
                    .ToList();

                if (doubleList.Count == 0)
                {
                    MessageBox.Show("Input is empty or invalid. Please enter valid numbers.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string saveString = string.Join(" ", doubleList.Select(x => x.ToString(System.Globalization.CultureInfo.InvariantCulture)));

                Properties.Settings.Default.Error = saveString;
                Properties.Settings.Default.Save();

                MessageBox.Show("List successfully saved.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
