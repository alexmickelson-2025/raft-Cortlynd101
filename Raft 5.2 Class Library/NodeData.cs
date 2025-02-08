using System.Text.Json.Serialization;
using Raft_5._2_Class_Library;

public class NodeData
{
    [JsonInclude]
    public int id;
    [JsonInclude]
    public string status;
    [JsonInclude]
    public int electionTimeout;
    [JsonInclude]
    public int term;
    [JsonInclude]
    public int currentTermLeader;
    [JsonInclude]
    public int committedEntryIndex;
    [JsonInclude]
    public Dictionary<int, string> log;
    [JsonInclude]
    public string state;
    [JsonInclude]
    public bool responsive;
    public NodeData()
    {
        id = 0;
        status = "unresponsive";
        electionTimeout = 0;
        term = 0;
        currentTermLeader = 0;
        committedEntryIndex = 0;
        log = new Dictionary<int, string>();
        state = "uninitalized";
        responsive = false;
    }

    public NodeData(int Id, string Status, int ElectionTimeout, int Term, int CurrentTermLeader, int CommittedEntryIndex, Dictionary<int, string> Log, string State, bool Responsive)
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
    public void Pause(List<NodeData> nodes, int id)
    {
        nodes[id].responsive = false;
    }
    public void UnPause(List<NodeData> nodes, int id)
    {
        nodes[id].responsive = true;
    }
    public bool Set(string value)
    {
        int key = committedEntryIndex;
        committedEntryIndex++;
        if (state != "leader" || value is null)
        {
            return false; 
        }

        ReceiveCommand(key, value);
        return true;
    }
    public void ReceiveCommand(int key, string value)
    {
        log.Add(key, value);
    }
}