using Raft_5._2_Class_Library;

public class NodeData
{
    private int id;
    private object status;
    //private Action<Election, List<INode>, int> electionTimeout;
    private object term;
    private object currentTermLeader;
    private object committedEntryIndex;
    private object log;
    private object state;

    public NodeData(int Id, object Status, /*Action<Election, List<INode>, int> ElectionTimeout,*/ object Term, object CurrentTermLeader, object CommittedEntryIndex, object Log, object State)
    {
        id = Id;
        status = Status;
        //electionTimeout = ElectionTimeout;
        term = Term;
        currentTermLeader = CurrentTermLeader;
        committedEntryIndex = CommittedEntryIndex;
        log = Log;
        state = State;
    }
}