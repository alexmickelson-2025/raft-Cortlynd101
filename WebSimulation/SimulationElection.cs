using Raft_5._2_Class_Library;

namespace WebSimulation
{
    public class SimulationElection : IElection
    {
        public readonly Election InnerElection;
        public SimulationElection(Election election)
        {
            InnerElection = election;
        }

        public bool electionOngoing { get => ((IElection)InnerElection).electionOngoing; set => ((IElection)InnerElection).electionOngoing = value; }
        public int term { get => ((IElection)InnerElection).term; set => ((IElection)InnerElection).term = value; }
        public int votesCast { get => ((IElection)InnerElection).votesCast; set => ((IElection)InnerElection).votesCast = value; }

        public void runElection(List<INode> nodes)
        {
            ((IElection)InnerElection).runElection(nodes);
        }
    }
}
