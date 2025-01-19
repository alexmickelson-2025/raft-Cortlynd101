using Raft_5._2_Class_Library;

namespace WebSimulation
{
    public class SimulationCluster : ICluster
    {
        public readonly Cluster InnerCluster;
        public SimulationCluster(Cluster cluster)
        {
            InnerCluster = cluster;
        }

        public Election election { get => ((ICluster)InnerCluster).election; set => ((ICluster)InnerCluster).election = value; }

        public void runCluster(List<INode> nodes)
        {
            ((ICluster)InnerCluster).runCluster(nodes);
        }
    }
}
