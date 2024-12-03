using System.Windows;
using System.Windows.Media;
using Microsoft.Msagl.Drawing;

namespace comp_netwrks_course_work
{


    public static class ColorConverter
    {
        public static Microsoft.Msagl.Drawing.Color ConvertToMsaglColor(string resourceKey)
        {
            // Получаем Brush из ресурсов приложения
            var brush = Application.Current.Resources[resourceKey] as SolidColorBrush;

            if (brush == null)
            {
                throw new ArgumentException($"Resource with key '{resourceKey}' is not a SolidColorBrush.");
            }

            // Получаем цвет из SolidColorBrush
            var color = brush.Color;

            // Конвертируем в MSAGL Color
            return new Microsoft.Msagl.Drawing.Color(color.A, color.R, color.G, color.B);
        }
    }
}
