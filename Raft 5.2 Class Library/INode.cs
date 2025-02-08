
namespace Raft_5._2_Class_Library;

public interface INode
{
    int delay { get; set; }
    int intervalScaler { get; set; }
    bool _vote { get; set; }
    int directedVote { get; set; }
    int electionTimeout { get; set; }
    List<string> entries { get; set; }
    bool goFirst { get; set; }
    bool hasActed { get; set; }
    bool hasVoted { get; set; }
    int Id { get; set; }
    int leaderId { get; set; }
    bool receivedHeartBeat { get; set; }
    bool receivedResponse { get; set; }
    bool recievedRPC { get; set; }
    bool responsive { get; set; }
    string serverType { get; set; }
    int term { get; set; }
    int termVotedFor { get; set; }
    int voteCount { get; set; }
    int votingFor { get; set; }
    Dictionary<int, string> log { get; set; }
    bool sentRPCs { get; set; }
    bool forcedOutcome { get; set; }
    List<int> nextIndex { get; set; }
    int committedIndex { get; set; }
    int nodeCount { get; set; }
    int committedLogCount { get; set; }
    int receivedCommittedLogIndex { get; set; }
    int receivedCommittedTermIndex { get; set; }
    int previousIndex { get; set; }
    int previousTerm { get; set; }
    bool thereIsACandidate { get; set; }

    bool Set(string value);
    void Pause(List<INode> nodes, int id);
    void UnPause(Cluster cluster, List<INode> nodes, int id);
    void ReceiveCommand(int key, string value);
    void AttemptLogCommit(List<INode> nodes, int id, int highestCommittedIndex, int term);
    void Act(ref List<INode> nodes, int id, Election election);
    void AppendEntries(List<INode> nodes, int id, int highestCommittedIndex, int term, int prevIndex, int prevTerm);
    void BecomeCandidate();
    void BecomeDirectedFollower();
    void BecomeFollower();
    void BecomeLeader();
    void ElectionTimeout(Election election, List<INode> nodes, int id);
    bool IsElectionWinner();
    void ReceiveHeartBeat(int newIndex, int newTerm);
    void receiveRPC(Election election, List<INode> nodes, int id, int sentTerm);
    void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, int lastCommittedLogIndex, List<INode> nodes);
    bool Request();
    void SendHeartBeats(List<INode> nodes);
    void SendHeartBeatsImmediately(List<INode> nodes);
    void SendRPCs(Election election, List<INode> nodes);
    void StartElection(Election election, List<INode> nodes);
    void Vote(List<INode> nodes, int id);
    void SetNextIndex(List<INode> nodes, int index);
    void RecieveLogCommit();
}