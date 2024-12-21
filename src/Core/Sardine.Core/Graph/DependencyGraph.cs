namespace Sardine.Core.Graph
{
    public sealed class DependencyGraph
    {
        public IReadOnlyCollection<DependencyNode> Nodes { get; }


        internal DependencyGraph(List<Vessel>? vesselCollection, bool linkOnly = false)
        {
            DependencyNode[] nodeInfo = [];

            if (vesselCollection is not null)
                nodeInfo = DependencyGraphHelpers.GetNodesFromVesselList(vesselCollection, linkOnly);

            Nodes = nodeInfo;
        }
    }
}