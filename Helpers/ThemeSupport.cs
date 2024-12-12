using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public static class ThemeSupport
    {
        public static void ApplyTheme(string themePath, Window window)
        {
            Properties.Settings.Default.currentTheme = themePath;
            Properties.Settings.Default.Save();
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri(themePath, UriKind.Relative)
            });
            window.Background = (Brush)Application.Current.Resources["BackgroundColor"];
            window.Foreground = (Brush)Application.Current.Resources["ForegroundColor"];
        }
    }
}
