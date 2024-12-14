using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Layout.MDS;
using System.Windows;


namespace comp_netwrks_course_work
{
    public partial class GraphWindow : Window
    {
        private readonly GViewer _gViewer;
        public bool DontCLOSE;

        public GraphWindow(bool dontclose)
        {
            DontCLOSE = dontclose;
            InitializeComponent();
            ThemeSupport.ApplyTheme(Properties.Settings.Default.currentTheme, this);
            _gViewer = new GViewer();
            GViewerHost.Child = _gViewer;
        }

        public void DrawGraph(List<Node> nodes, List<Connection> edges)
        {
            var graph = new Graph("Graph");

            graph.Attr.BackgroundColor = ColorConverter.ConvertToMsaglColor("BackgroundColor");
            graph.LayoutAlgorithmSettings = new MdsLayoutSettings();

            foreach (var lnode in nodes)
            {
                var node = graph.AddNode(lnode.Number.ToString());
                node.Attr.Shape = Microsoft.Msagl.Drawing.Shape.Circle;
                node.Attr.XRadius = 15;
                node.Attr.YRadius = 15;
                node.Label.FontSize = 32;
                node.Attr.FillColor = lnode.GetColor();
                node.LabelText = $"{lnode.Number}";
            }

            foreach (var edge in edges)
            {
                var msaglEdge = graph.AddEdge(edge.Node1.Number.ToString(),
                                              edge.Weight.ToString(),
                                              edge.Node2.Number.ToString());
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
                msaglEdge.LabelText = edge.GetBalancedFlow() + "/" + edge.Weight.ToString();
            }

            _gViewer.Graph = graph;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (DontCLOSE)
            {
                e.Cancel = true;

                MessageBox.Show("Window close forbidden.");
            }
        }
    }
}
