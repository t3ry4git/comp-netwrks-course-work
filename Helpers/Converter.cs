using System.Windows;
using System.Windows.Media;

namespace comp_netwrks_course_work
{
    public static class ColorConverter
    {
        public static Microsoft.Msagl.Drawing.Color ConvertToMsaglColor(string resourceKey)
        {
            var brush = Application.Current.Resources[resourceKey] as SolidColorBrush
                ?? throw new ArgumentException($"Resource with key '{resourceKey}' is not a SolidColorBrush.");
            var color = brush.Color;
            return new Microsoft.Msagl.Drawing.Color(color.A, color.R, color.G, color.B);
        }
    }

    public static class ConnectionTypeConverter
    {
        public static string ToString(ConnectionType connectionType) => connectionType.ToString();
        public static ConnectionType FromString(string connectionTypeString)
        {
            if (Enum.TryParse(connectionTypeString, true, out ConnectionType result))
                return result;
            else
                throw new ArgumentException($"Invalid connection type: {connectionTypeString}");
        }
    }

    public static class ConnectionTypeParser
    {
        private static readonly Dictionary<char, ConnectionType> TypeMap = new()
        {
        { 'd', ConnectionType.Duplex },
        { 'h', ConnectionType.HalfDuplex },
        { 's', ConnectionType.Satellite },
        { 'r', ConnectionType.Random }
        };

        public static List<ConnectionType> Parse(string input)
        {
            var result = new List<ConnectionType>();
            input = input.Replace("\r", "").Replace("\n", "");

            var tokens = input.Replace(" ", "").ToCharArray();
            foreach (var token in tokens)
            {
                if (TypeMap.TryGetValue(token, out var connectionType))
                    result.Add(connectionType);
                else
                    throw new ArgumentException($"Invalid character '{token}' in input string.");
            }

            return result;
        }
    }
}
