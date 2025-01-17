namespace Raft_5._2_Class_Library
{
    public class Cluster
    {
        bool clusterRunning = false;
        public Election election = new();

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
