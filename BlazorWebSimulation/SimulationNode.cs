using Raft_5._2_Class_Library;

namespace BlazorWebSimulation
{
    public class SimulationNode : INode 
    {
        public readonly Node InnerNode;
        public SimulationNode(Node node)
        {
            InnerNode = node;
        }

        public int leaderId { get => ((INode)InnerNode).leaderId; set => ((INode)InnerNode).leaderId = value; }

        public void Act(List<Node> nodes, int id, Election election)
        {
            throw new NotImplementedException();
        }

        public void BecomeCandidate()
        {
            throw new NotImplementedException();
        }

        public void BecomeFollower()
        {
            throw new NotImplementedException();
        }

        public void BecomeLeader()
        {
            throw new NotImplementedException();
        }

        public bool IsElectionWinner()
        {
            throw new NotImplementedException();
        }

        public bool Request()
        {
            throw new NotImplementedException();
        }

        public void SendHeartBeats(List<Node> nodes)
        {
            throw new NotImplementedException();
        }

        public void StartElection(Election election, List<Node> nodes)
        {
            throw new NotImplementedException();
        }

        public void Vote(List<Node> nodes, int id)
        {
            throw new NotImplementedException();
        }
    }
}
