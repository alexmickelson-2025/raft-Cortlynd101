
namespace Raft_5._2_Class_Library
{
    public interface INode
    {
        int leaderId { get; set; }
        void Act(List<Node> nodes, int id, Election election);
        void BecomeCandidate();
        void BecomeFollower();
        void BecomeLeader();
        bool IsElectionWinner();
        bool Request();
        void SendHeartBeats(List<Node> nodes);
        void StartElection(Election election, List<Node> nodes);
        void Vote(List<Node> nodes, int id);
    }
}