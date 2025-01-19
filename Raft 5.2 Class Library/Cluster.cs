namespace Raft_5._2_Class_Library
{
    public class Cluster : ICluster
    {
        public bool clusterRunning { get; set; } = false;
        public Election election { get; set; } = new();

        public void runCluster(List<INode> nodes)
        {
            clusterRunning = true;

            while (clusterRunning)
            {
                Thread.Sleep(10);
                for (int i = 0; i < nodes.Count(); i++)
                {
                    nodes[i].Act(nodes, i, election);
                }
                clusterRunning = false;
            }
        }
    }
}
