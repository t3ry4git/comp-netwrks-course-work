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
            var brush = Application.Current.Resources[resourceKey] as SolidColorBrush ?? throw new ArgumentException($"Resource with key '{resourceKey}' is not a SolidColorBrush.");

            // Получаем цвет из SolidColorBrush
            var color = brush.Color;

            // Конвертируем в MSAGL Color
            return new Microsoft.Msagl.Drawing.Color(color.A, color.R, color.G, color.B);
        }
    }
    public static class ConnectionTypeConverter
    {
        // Конвертация Enum -> String
        public static string ToString(ConnectionType connectionType)
        {
            return connectionType.ToString();
        }

        // Конвертация String -> Enum
        public static ConnectionType FromString(string connectionTypeString)
        {
            if (Enum.TryParse(connectionTypeString, true, out ConnectionType result))
            {
                return result;
            }
            else
            {
                throw new ArgumentException($"Invalid connection type: {connectionTypeString}");
            }
        }
    }

    public static class ConnectionTypeParser
    {
        // Словарь для маппинга символов на ConnectionType
        private static readonly Dictionary<char, ConnectionType> TypeMap = new()
        {
        { 'd', ConnectionType.Duplex },
        { 'h', ConnectionType.HalfDuplex },
        { 's', ConnectionType.Satellite },
        { 'r', ConnectionType.Random }
    };

        /// <summary>
        /// Преобразует строку в список ConnectionType.
        /// </summary>
        /// <param name="input">Строка вида "d h d h s s r d h".</param>
        /// <returns>Список ConnectionType.</returns>
        public static List<ConnectionType> Parse(string input)
        {
            var result = new List<ConnectionType>();

            // Убираем символы \r и \n
            input = input.Replace("\r", "").Replace("\n", "");

            // Убираем лишние пробелы и превращаем строку в массив символов
            var tokens = input.Replace(" ", "").ToCharArray();

            // Преобразуем каждый символ в ConnectionType
            foreach (var token in tokens)
            {
                if (TypeMap.TryGetValue(token, out var connectionType))
                {
                    result.Add(connectionType);
                }
                else
                {
                    throw new ArgumentException($"Invalid character '{token}' in input string.");
                }
            }

            return result;
        }

    }
}
