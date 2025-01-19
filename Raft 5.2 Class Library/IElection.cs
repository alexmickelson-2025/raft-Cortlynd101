
namespace Raft_5._2_Class_Library
{
    public interface IElection
    {
        bool electionOngoing { get; set; }
        int term { get; set; }
        int votesCast { get; set; }

        void runElection(List<INode> nodes);
    }
}