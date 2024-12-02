using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Windows.Forms.Integration; // Для WindowsFormsHost


namespace comp_netwrks_course_work
{

    /// <summary>
    /// Interaction logic for GraphWindow.xaml
    /// </summary>
    public partial class GraphWindow : Window
    {
        private GViewer _gViewer;

        public GraphWindow()
        {
            InitializeComponent();

            // Привязываем GViewer к WindowsFormsHost
            _gViewer = new GViewer();
            GViewerHost.Child = _gViewer;
        }


        public void DrawGraph((double X, double Y)[] nodes, (int From, int To, int Weight)[] edges)
        {
            // Создаём новый граф
            var graph = new Graph("Graph");

            // Добавляем узлы
            for (int i = 0; i < nodes.Length; i++)
            {
                var node = graph.AddNode(i.ToString());
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                node.Attr.FillColor = Microsoft.Msagl.Drawing.Color.LightBlue;
                node.LabelText = $"({nodes[i].X}, {nodes[i].Y})"; // Можно убрать, если позиции не нужны
            }

            // Добавляем рёбра
            foreach (var edge in edges)
            {
                var msaglEdge = graph.AddEdge(edge.From.ToString(), edge.Weight.ToString(), edge.To.ToString());
                msaglEdge.Attr.Color = Microsoft.Msagl.Drawing.Color.Gray;
                msaglEdge.Attr.LineWidth = 2;
                msaglEdge.LabelText = edge.Weight.ToString();
            }

            // Устанавливаем граф для GViewer
            _gViewer.Graph = graph;
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
