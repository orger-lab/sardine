using Sardine.Core.DataModel.Abstractions;

namespace Sardine.Core.Graph
{
    internal static class DependencyGraphHelpers
    {
        internal static void ConnectDataDependencies(Dictionary<Vessel, DependencyNode> nodeInfo, Vessel vessel)
        {
            foreach (KeyValuePair<IDataConsumer, List<Vessel>?> kvp in vessel.ReceiverFiltersDictionary)
            {
                foreach (Vessel weakDependency in kvp.Value!)
                {
                    if (weakDependency.IsLinked)
                    {
                        nodeInfo[vessel].AddConnection(new NodeConnection(nodeInfo[weakDependency], nodeInfo[vessel], NodeConnectionType.Data, kvp.Key.InputDataType));
                        nodeInfo[weakDependency].AddConnection(new NodeConnection(nodeInfo[weakDependency], nodeInfo[vessel], NodeConnectionType.Data, kvp.Key.InputDataType));
                    }
                }
            }
        }

        internal static DependencyNode[] GetNodesFromVesselList(List<Vessel> vessels, bool linkOnly)
        {
            Dictionary<Vessel, DependencyNode> nodeInfo = [];

            foreach (Vessel vessel in vessels)
                nodeInfo.Add(vessel, new DependencyNode(vessel));

            foreach (Vessel vessel in vessels)
            {
                foreach (Vessel dependency in vessel.DependencyList)
                {
                    nodeInfo[vessel].AddConnection(new NodeConnection(nodeInfo[dependency], nodeInfo[vessel], NodeConnectionType.Link));
                    nodeInfo[dependency].AddConnection(new NodeConnection(nodeInfo[dependency], nodeInfo[vessel], NodeConnectionType.Link));
                }
            }

            if (!linkOnly)
            {
                foreach (Vessel vessel in vessels)
                {
                    if (vessel.IsLinked)
                        ConnectDataDependencies(nodeInfo, vessel);
                }
            }

            return [.. nodeInfo.Values];
        }
    }
}