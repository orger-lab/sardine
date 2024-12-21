namespace Sardine.Core.Graph
{
    public sealed class NodeConnection
    {
        public DependencyNode DownstreamNode { get; }
        public DependencyNode UpstreamNode { get; }
        public NodeConnectionType ConnectionType { get; }
        public Type? DependencyType { get; }

        internal NodeConnection(DependencyNode downstreamNode, DependencyNode upstreamNode, NodeConnectionType connectionType, Type? dataLinkType = null)
        {
            DownstreamNode = downstreamNode;
            UpstreamNode = upstreamNode;
            ConnectionType = connectionType;
            DependencyType = dataLinkType;

            if (connectionType == NodeConnectionType.Link)
                DependencyType = typeof(Vessel<>).MakeGenericType(downstreamNode.Vessel.HandleType);
        }
    }
}