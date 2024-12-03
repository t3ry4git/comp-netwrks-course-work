using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using System.Windows.Forms.Integration;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Layout.MDS; // Для WindowsFormsHost


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


        public void DrawGraph(List<Node> nodes, List<Connection> edges)
        {
            // Создаём новый граф
            var graph = new Graph("Graph");
            graph.Attr.BackgroundColor = ColorConverter.ConvertToMsaglColor("BackgroundColor");
            var mdsSettings = new MdsLayoutSettings();
            graph.LayoutAlgorithmSettings = mdsSettings;
            // Добавляем узлы
            foreach (var lnode in nodes)
            {
                var node = graph.AddNode(lnode.Number.ToString());
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                node.Attr.XRadius = 15; // Радиус по оси X
                node.Attr.YRadius = 15; // Радиус по оси Y

                // Увеличиваем размер шрифта
                node.Label.FontSize = 32;
                node.Attr.FillColor = lnode.GetColor();
                node.LabelText = $"{lnode.Number}"; // Можно убрать, если позиции не нужны
            }

            // Добавляем рёбра
            foreach (var edge in edges)
            {
                var msaglEdge = graph.AddEdge(edge.Node1.Number.ToString(), edge.Weight.ToString(), edge.Node2.Number.ToString());
                msaglEdge.Attr.Color = edge.GetColor();
                msaglEdge.Label.FontColor = ColorConverter.ConvertToMsaglColor("ForegroundColor");
                msaglEdge.Attr.LineWidth = 3;
                msaglEdge.Attr.ArrowheadAtSource = ArrowStyle.Normal;
                msaglEdge.Attr.ArrowheadAtTarget = ArrowStyle.Normal;
                if (edge.Type == ConnectionType.HalfDuplex)
                {
                    msaglEdge.Attr.LineWidth = 1;
                    msaglEdge.Attr.Color = edge.GetColor();
                }
                msaglEdge.Label.FontSize = 32;
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
