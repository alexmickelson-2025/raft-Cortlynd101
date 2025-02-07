using Raft_5._2_Class_Library;

public class NodeData
{
    public int id;
    public string status;
    public int electionTimeout;
    public int term;
    public int currentTermLeader;
    public int committedEntryIndex;
    public Dictionary<int, string> log;
    public object state;
    public bool responsive;

    public NodeData(int Id, string Status, int ElectionTimeout, int Term, int CurrentTermLeader, int CommittedEntryIndex, Dictionary<int, string> Log, object State, bool Responsive)
    {
        id = Id;
        status = Status;
        electionTimeout = ElectionTimeout;
        term = Term;
        currentTermLeader = CurrentTermLeader;
        committedEntryIndex = CommittedEntryIndex;
        log = Log;
        state = State;
        responsive = Responsive;
    }
}