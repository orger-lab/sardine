using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using Sardine.Core.Graph;
using System.Windows;

namespace Sardine.Core.Views.Graph.WPF
{
    /// <summary>
    /// Interaction logic for SardineGraphVisualizer.xaml
    /// </summary>
    public partial class SardineGraphVisualizer : Window
    {
        private readonly GraphViewer graphViewer = new();
        private readonly Microsoft.Msagl.Drawing.Graph viewModel = new();

        public static void SaveImageAs(DependencyGraph dependencyGraph)
        {
            SardineGraphVisualizer visualizer = new(dependencyGraph);
            visualizer.Show();
            visualizer.Button_SaveImage_Click(null, null);
            visualizer.Close();
        }

        public SardineGraphVisualizer(DependencyGraph dependencyGraph)
        {
            InitializeComponent();

            if (dependencyGraph is not null)
                viewModel = GenerateGraphViewModel(dependencyGraph);

            graphViewer.BindToPanel(DockPanel_GraphViewer);
            graphViewer.Graph = viewModel;
        }

        private static Microsoft.Msagl.Drawing.Graph GenerateGraphViewModel(DependencyGraph dependencyGraph)
        {
            Microsoft.Msagl.Drawing.Graph graph = new();

            foreach (DependencyNode vesselNode in dependencyGraph.Nodes)
            {
                Node node = new(vesselNode.Vessel.DisplayName);
                node.Attr.Shape = Shape.Ellipse;
                node.Attr.FillColor = vesselNode.Vessel.IsAvailable ? Color.LightGreen : Color.LightPink;
                graph.AddNode(node);
            }

            foreach(DependencyNode vesselNode in dependencyGraph.Nodes)
            {
                foreach ((DependencyNode connectedNode, NodeConnection connection) in vesselNode.GetUpstreamNodes())
                {
                    Edge edge = new(graph.FindNode(vesselNode.Vessel.DisplayName), graph.FindNode(connectedNode.Vessel.DisplayName), ConnectionToGraph.Disconnected);
                    edge.Attr.Color = connection.ConnectionType == NodeConnectionType.Link ? Color.Aquamarine : Color.BlueViolet;

                    if (!graph.Edges.Any(x => x.ToString() == edge.ToString() && x.Attr.Color == edge.Attr.Color))
                        graph.AddPrecalculatedEdge(edge);
                }
            }

            graph.Attr.LayerDirection = LayerDirection.TB;
            graph.Attr.OptimizeLabelPositions = true;
            graph.CreateGeometryGraph();
            return graph;
        }

        private void Button_SaveImage_Click(object? sender, RoutedEventArgs? e)
        {
            SaveFileDialog saveFileDialog = new()
            {
                FileName = "graph.png",
                AddExtension = true,
                Filter = "PNG files (*.png)|*.png|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true,
                DefaultExt = "png",
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                graphViewer.DrawImage(saveFileDialog.FileName);
            }
        }
    }
}
