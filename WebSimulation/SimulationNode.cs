using Raft_5._2_Class_Library;

namespace WebSimulation;

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
    public int delay { get => ((INode)InnerNode).delay; set => ((INode)InnerNode).delay = value; }
    public int intervalScaler { get => ((INode)InnerNode).intervalScaler; set => ((INode)InnerNode).intervalScaler = value; }
    public bool sentRPCs { get => ((INode)InnerNode).sentRPCs; set => ((INode)InnerNode).sentRPCs = value; }
    public bool forcedOutcome { get => ((INode)InnerNode).forcedOutcome; set => ((INode)InnerNode).forcedOutcome = value; }
    public List<int> nextIndex { get => ((INode)InnerNode).nextIndex; set => ((INode)InnerNode).nextIndex = value; }
    public int committedIndex { get => ((INode)InnerNode).committedIndex; set => ((INode)InnerNode).committedIndex = value; }
    public Dictionary<int, string> log { get => ((INode)InnerNode).log; set => ((INode)InnerNode).log = value; }
    public int nodeCount { get => ((INode)InnerNode).nodeCount; set => ((INode)InnerNode).nodeCount = value; }
    public int committedLogCount { get => ((INode)InnerNode).committedLogCount; set => ((INode)InnerNode).committedLogCount = value; }
    public int receivedCommittedLogIndex { get => ((INode)InnerNode).receivedCommittedLogIndex; set => ((INode)InnerNode).receivedCommittedLogIndex = value; }
    public int receivedCommittedTermIndex { get => ((INode)InnerNode).receivedCommittedTermIndex; set => ((INode)InnerNode).receivedCommittedTermIndex = value; }
    public int previousIndex { get => ((INode)InnerNode).previousIndex; set => ((INode)InnerNode).previousIndex = value; }
    public int previousTerm { get => ((INode)InnerNode).previousTerm; set => ((INode)InnerNode).previousTerm = value; }
    public bool thereIsACandidate { get => ((INode)InnerNode).thereIsACandidate; set => ((INode)InnerNode).thereIsACandidate = value; }

    public void Act(List<INode> nodes, int id, Election election)
    {
        ((INode)InnerNode).Act(nodes, id, election);
    }

    public void AppendEntries(List<INode> nodes, int id, int highestCommittedIndex, int term, int prevIndex, int prevTerm)
    {
        ((INode)InnerNode).AppendEntries(nodes, id, highestCommittedIndex, term, prevIndex, prevTerm);
    }

    public void AttemptLogCommit(List<INode> nodes, int id, int highestCommittedIndex, int term)
    {
        ((INode)InnerNode).AttemptLogCommit(nodes, id, highestCommittedIndex, term);
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

    public void ElectionTimeout(Election election, List<INode> nodes, int id)
    {
        ((INode)InnerNode).ElectionTimeout(election, nodes, id);
    }

    public bool IsElectionWinner()
    {
        return ((INode)InnerNode).IsElectionWinner();
    }

    public void Pause(List<INode> nodes, int id)
    {
        ((INode)InnerNode).Pause(nodes, id);
    }

    public void ReceiveCommand(int key, string value)
    {
        ((INode)InnerNode).ReceiveCommand(key, value);
    }

    public void ReceiveHeartBeat(int newIndex, int newTerm)
    {
        ((INode)InnerNode).ReceiveHeartBeat(newIndex, newTerm);
    }

    public void receiveRPC(Election election, List<INode> nodes, int id, int sentTerm)
    {
        ((INode)InnerNode).receiveRPC(election, nodes, id, sentTerm);
    }

    public void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, int commitIndex, List<INode> nodes)
    {
        ((INode)InnerNode).RecieveAppendEntries(newEntries, id, receivedTerm, commitIndex, nodes);
    }

    public void RecieveLogCommit()
    {
        ((INode)InnerNode).RecieveLogCommit();
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

    public bool Set(string value)
    {
        return ((INode)InnerNode).Set(value);
    }

    public void SetNextIndex(List<INode> nodes, int index)
    {
        ((INode)InnerNode).SetNextIndex(nodes, index);
    }

    public void StartElection(Election election, List<INode> nodes)
    {
        ((INode)InnerNode).StartElection(election, nodes);
    }

    public void UnPause(Cluster cluster, List<INode> nodes, int id)
    {
        ((INode)InnerNode).UnPause(cluster, nodes, id);
    }

    public void Vote(List<INode> nodes, int id)
    {
        ((INode)InnerNode).Vote(nodes, id);
    }
}
