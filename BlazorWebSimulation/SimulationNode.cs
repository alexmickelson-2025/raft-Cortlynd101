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
        public bool _vote { get => ((INode)InnerNode)._vote; set => ((INode)InnerNode)._vote = value; }
        public int directedVote { get => ((INode)InnerNode).directedVote; set => ((INode)InnerNode).directedVote = value; }
        public int electionTimeout { get => ((INode)InnerNode).electionTimeout; set => ((INode)InnerNode).electionTimeout = value; }
        public List<string> entries { get => ((INode)InnerNode).entries; set => ((INode)InnerNode).entries = value; }
        public bool goFirst { get => ((INode)InnerNode).goFirst; set => ((INode)InnerNode).goFirst = value; }
        public bool hasActed { get => ((INode)InnerNode).hasActed; set => ((INode)InnerNode).hasActed = value; }
        public bool hasVoted { get => ((INode)InnerNode).hasVoted; set => ((INode)InnerNode).hasVoted = value; }
        public int Id { get => ((INode)InnerNode).Id; set => ((INode)InnerNode).Id = value; }
        public int leaderId { get => ((INode)InnerNode).leaderId; set => ((INode)InnerNode).leaderId = value; }
        public bool receivedHeartBeat { get => ((INode)InnerNode).receivedHeartBeat; set => ((INode)InnerNode).receivedHeartBeat = value; }
        public bool receivedResponse { get => ((INode)InnerNode).receivedResponse; set => ((INode)InnerNode).receivedResponse = value; }
        public bool recievedRPC { get => ((INode)InnerNode).recievedRPC; set => ((INode)InnerNode).recievedRPC = value; }
        public bool responsive { get => ((INode)InnerNode).responsive; set => ((INode)InnerNode).responsive = value; }
        public string serverType { get => ((INode)InnerNode).serverType; set => ((INode)InnerNode).serverType = value; }
        public int term { get => ((INode)InnerNode).term; set => ((INode)InnerNode).term = value; }
        public int termVotedFor { get => ((INode)InnerNode).termVotedFor; set => ((INode)InnerNode).termVotedFor = value; }
        public int voteCount { get => ((INode)InnerNode).voteCount; set => ((INode)InnerNode).voteCount = value; }
        public int votingFor { get => ((INode)InnerNode).votingFor; set => ((INode)InnerNode).votingFor = value; }

        public void Act(List<INode> nodes, int id, Election election)
        {
            ((INode)InnerNode).Act(nodes, id, election);
        }

        public void AppendEntries(List<INode> nodes, int id)
        {
            ((INode)InnerNode).AppendEntries(nodes, id);
        }

        public void BecomeCandidate()
        {
            ((INode)InnerNode).BecomeCandidate();
        }

        public void BecomeDirectedFollower()
        {
            ((INode)InnerNode).BecomeDirectedFollower();
        }

        public void BecomeFollower()
        {
            ((INode)InnerNode).BecomeFollower();
        }

        public void BecomeLeader()
        {
            ((INode)InnerNode).BecomeLeader();
        }

        public void ElectionTimeout(Election election, List<INode> nodes)
        {
            ((INode)InnerNode).ElectionTimeout(election, nodes);
        }

        public bool IsElectionWinner()
        {
            return ((INode)InnerNode).IsElectionWinner();
        }

        public void ReceiveHeartBeat()
        {
            ((INode)InnerNode).ReceiveHeartBeat();
        }

        public void receiveRPC(Election election, List<INode> nodes, int id, int sentTerm)
        {
            ((INode)InnerNode).receiveRPC(election, nodes, id, sentTerm);
        }

        public void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, List<INode> nodes)
        {
            ((INode)InnerNode).RecieveAppendEntries(newEntries, id, receivedTerm, nodes);
        }

        public bool Request()
        {
            return ((INode)InnerNode).Request();
        }

        public void SendHeartBeats(List<INode> nodes)
        {
            ((INode)InnerNode).SendHeartBeats(nodes);
        }

        public void SendHeartBeatsImmediately(List<INode> nodes)
        {
            ((INode)InnerNode).SendHeartBeatsImmediately(nodes);
        }

        public void SendRPCs(Election election, List<INode> nodes)
        {
            ((INode)InnerNode).SendRPCs(election, nodes);
        }

        public void StartElection(Election election, List<INode> nodes)
        {
            ((INode)InnerNode).StartElection(election, nodes);
        }

        public void Vote(List<INode> nodes, int id)
        {
            ((INode)InnerNode).Vote(nodes, id);
        }
    }
}
