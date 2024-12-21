namespace Sardine.Core.Graph
{
    public sealed class DependencyNode
    {
        private readonly List<NodeConnection> connections = [];
        private int nodeLevel;


        public Vessel Vessel { get; }

        public int NodeLevel
        {
            get => nodeLevel;

            private set
            {
                nodeLevel = value;
                UpdateUpstreamNodeLevels();
            }
        }


        internal DependencyNode(Vessel vessel)
        {
            Vessel = vessel;
        }


        public IList<(DependencyNode, NodeConnection)> GetUpstreamNodes() => connections.Where((x) => x.DownstreamNode == this).Select((x) => (x.UpstreamNode,x)).ToList();
        public IList<(DependencyNode, NodeConnection)> GetDownstreamNodes() => connections.Where((x) => x.UpstreamNode == this).Select((x) => (x.DownstreamNode,x)).ToList();

        internal void AddConnection(NodeConnection connection)
        {
            connections.Add(connection);

            if (connection.UpstreamNode == this)
                NodeLevel = Math.Max(NodeLevel, connection.DownstreamNode.NodeLevel + 1);
        }

        private void UpdateUpstreamNodeLevels()
        {
            foreach (NodeConnection connection in connections)
            {
                if (connection.DownstreamNode == this)
                {
                    if (connection.UpstreamNode.NodeLevel <= NodeLevel)
                        connection.UpstreamNode.NodeLevel = NodeLevel + 1;
                }
            }
        }
    }
}