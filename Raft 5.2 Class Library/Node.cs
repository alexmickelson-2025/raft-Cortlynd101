using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raft_5._2_Class_Library;

public class Node : INode
{
    public int delay { get; set; }
    public int intervalScaler { get; set; } = 0;
    public int Id { get; set; } = 0;
    public bool _vote { get; set; } = false;
    public bool hasVoted { get; set; } = false;
    public bool hasActed { get; set; } = false;
    public string serverType { get; set; } = "follower";
    public bool receivedHeartBeat { get; set; } = false;
    public List<string> entries { get; set; } = [];
    public int leaderId { get; set; } = -1;
    public int electionTimeout { get; set; } = 0;
    public int voteCount { get; set; } = 0;
    public bool responsive { get; set; } = true;
    public int directedVote { get; set; } = 0;
    public int term { get; set; } = 0;
    public bool receivedResponse { get; set; } = false;
    public bool recievedRPC { get; set; } = false;
    public int votingFor { get; set; } = -1;
    public int termVotedFor { get; set; } = 0;
    public bool goFirst { get; set; } = false;
    public bool sentRPCs { get; set; } = false;
    public bool forcedOutcome { get; set; } = false;
    public Dictionary<int, string> log { get; set; } = new Dictionary<int, string>();
    public List<int> nextIndex { get; set; } = [];
    public int committedIndex { get; set; } = 0;
    public int nodeCount { get; set; } = 0;
    public int committedLogCount { get; set; } = 0;
    public int receivedCommittedLogIndex { get; set; } = 0;
    public int receivedCommittedTermIndex { get; set; } = 0;
    public int previousIndex { get; set; } = 0;
    public int previousTerm { get; set; } = 0;
    public bool thereIsACandidate { get; set; } = false;

    public Node()
    {
        responsive = true;
    }
    public Node(bool responsiveOrNot)
    {
        responsive = responsiveOrNot;
    }

    public void Vote(List<INode> nodes, int id)
    {
        if (responsive == false)
        {
            return;
        }

        hasVoted = false;
        if (serverType == "leader")
        {

        }
        else if (serverType == "follower")
        {
            Random random = new();
            int randomNum = random.Next(nodes.Count);

            if (votingFor != -1)
            {
                nodes[votingFor].voteCount++;
                return;
            }

            bool voting = true;
            while (voting)
            {
                if (nodes[randomNum].serverType == "candidate")
                {
                    nodes[randomNum].voteCount++;
                    voting = false;
                }
                else
                {
                    if (randomNum > 0)
                    {
                        randomNum--;
                        voting = false;
                    }
                    else
                    {
                        randomNum++;
                        voting = false;
                    }
                }
            }
            hasVoted = true;
        }
        else if (serverType == "directedFollower")
        {
            nodes[directedVote].voteCount++;
        }
        else
        {
            voteCount++;
            hasVoted = true;
        }
    }

    public void Act(List<INode> nodes, int id, Election election)
    {
        Console.WriteLine("This node is a: "+ nodes[id].serverType);
        Thread.Sleep(10);
        if (responsive == false)
        {
            return;
        }

        Id = id;
        if (serverType == "leader")
        {
            Random random = new();
            int randomNum = random.Next(0, 100);
            if (randomNum > 100 && forcedOutcome == false)
            {
                nodes[id].BecomeFollower();
            }
            else
            {
                nodeCount = nodes.Count();
                leaderId = id;
                SendHeartBeats(nodes);
                nodes[id].AppendEntries(nodes, id, committedIndex, term, previousIndex, previousTerm);
                AttemptLogCommit(nodes, id, committedIndex, term);
                SetNextIndex(nodes, id);
            }
        }
        else if (serverType == "follower")
        {
            ElectionTimeout(election, nodes, id);
        }
        else
        {
            SendRPCs(election, nodes);
            if (!election.electionOngoing)
            {
                StartElection(election, nodes);
            }
        }
        hasActed = true;
    }

    public void SendRPCs(Election election, List<INode> nodes)
    {
        sentRPCs = true;
        foreach (Node node in nodes)
        {
            node.receiveRPC(election, nodes, Id, term);
        }
    }

    public void receiveRPC(Election election, List<INode> nodes, int id, int sentTerm)
    {
        if (serverType == "follower")
        {
            if (sentTerm < term || termVotedFor >= sentTerm)
            {
                return;
            }

            termVotedFor = sentTerm;
            votingFor = id;
        }
    }

    public void ElectionTimeout(Election election, List<INode> nodes, int id)
    {
        if (intervalScaler != 0)
        {
            ThreadLocal<Random> random = new(() => new Random());
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int randomNum = random.Value.Next((150 * intervalScaler), (300 * intervalScaler));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            electionTimeout = randomNum;
        }
        else
        {
            ThreadLocal<Random> random = new(() => new Random());
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int randomNum = random.Value.Next(150, 300);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            electionTimeout = randomNum;
        }

        Thread.Sleep(electionTimeout);
        if (!receivedHeartBeat)
        {
            leaderId = -1;
            ThreadLocal<Random> random2 = new(() => new Random());
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            int randomNum2 = random2.Value.Next(0, 10);
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            if (randomNum2 < 11 && !thereIsACandidate)
            {
                nodes[id].BecomeCandidate();
                foreach (var node in nodes)
                {
                    node.thereIsACandidate = true;
                }
                StartElection(election, nodes);
            }
            else
            {
                if (forcedOutcome && !thereIsACandidate)
                {
                    nodes[id].BecomeCandidate();
                    StartElection(election, nodes);
                }
            }
        }
    }

    public void StartElection(Election election, List<INode> nodes)
    {
        term++;
        election.electionOngoing = true;
        election.term++;
        election.runElection(nodes);
    }

    public void AppendEntries(List<INode> nodes, int id, int highestCommittedIndex, int term, int prevIndex, int prevTerm)
    {
        sentRPCs = true;
        Thread.Sleep(10);
        foreach (var node in nodes)
        {
            int logTimeDifference = highestCommittedIndex - node.committedIndex;
            if (highestCommittedIndex <= log.Count() - 1 && node.responsive && logTimeDifference < 10 && highestCommittedIndex == term && prevIndex <= previousIndex && prevTerm <= previousTerm)
            {
                for (int i = node.committedIndex + 1; i <= highestCommittedIndex; i++)
                {
                    if (!node.log.ContainsKey(i))
                    {
                        node.log.Add(i, log[i]);
                    }
                }
                //Might need to do this multiple times?
                nodes[id].RecieveLogCommit();
            }
            else
            {
                if (node.responsive && logTimeDifference < 10 && highestCommittedIndex == term && prevIndex <= previousIndex && prevTerm <= previousTerm)
                {
                    node.log = log.ToDictionary(entry => entry.Key, entry => entry.Value);
                }
            }

            if (node.responsive && logTimeDifference < 10 && highestCommittedIndex == term && prevIndex <= previousIndex && prevTerm <= previousTerm)
            {
                previousIndex = node.committedIndex;
                previousTerm = term;
                node.receivedResponse = true;
                node.committedIndex = highestCommittedIndex;
                node.RecieveAppendEntries(entries, id, term, committedIndex, nodes);
            }
        }
    }

    public void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, int lastCommittedLogIndex, List<INode> nodes)
    {
        if (serverType == "leader")
        {
            receivedCommittedLogIndex = lastCommittedLogIndex;
            receivedCommittedTermIndex = receivedTerm;
            receivedResponse = true;
            return;
        }

        if (receivedTerm < term)
        {
            return;
        }

        if (serverType == "candidate")
        {
            entries = newEntries;
            leaderId = id;
            serverType = "follower";
            return;
        }

        if (serverType == "follower")
        {
            entries = newEntries;
            leaderId = id;
            nodes[leaderId].RecieveAppendEntries(newEntries, leaderId, term, committedIndex, nodes);
        }
    }

    public void SendHeartBeats(List<INode> nodes)
    {
        Thread.Sleep(40);
        foreach (var node in nodes)
        {
            node.ReceiveHeartBeat(committedIndex, term);
        }
    }

    public void SendHeartBeatsImmediately(List<INode> nodes)
    {
        foreach (var node in nodes)
        {
            node.ReceiveHeartBeat(committedIndex, term);
        }
    }

    public void ReceiveHeartBeat(int newIndex, int newTerm)
    {
        if (newTerm < term)
        {
            return;
        }

        if (serverType != "leader")
        {
            if (newIndex >= previousIndex && newTerm >= previousTerm)
            {
                committedIndex = newIndex;
                term = newTerm;
            }
            receivedHeartBeat = true;
        }
    }

    public bool IsElectionWinner()
    {
        return _vote;
    }

    public bool Request()
    {
        return _vote;
    }

    public void BecomeFollower()
    {
        sentRPCs = false;
        serverType = "follower";
        receivedHeartBeat = false;
    }

    public void BecomeLeader()
    {
        serverType = "leader";
        leaderId = leaderId;
        receivedHeartBeat = false;
    }
    public void BecomeCandidate()
    {
        sentRPCs = false;
        serverType = "candidate";
        leaderId = Id;
        receivedHeartBeat = false;
    }
    public void BecomeDirectedFollower()
    {
        sentRPCs = false;
        serverType = "directedFollower";
    }

    public void ReceiveCommand(int key, string value)
    {
        log.Add(key, value);
    }

    public void SetNextIndex(List<INode> nodes, int index)
    {
        foreach (var node in nodes)
        {
            nextIndex.Add(node.log.Count());
        }
    }

    public void AttemptLogCommit(List<INode> nodes, int id, int highestCommittedIndex, int term)
    {
        if (committedLogCount > (nodeCount / 2))
        {
            term++;
            committedIndex++;
            committedLogCount = 0;
            nodes[id].AppendEntries(nodes, id, committedIndex, term, previousIndex, previousTerm);
        }
    }
    public void RecieveLogCommit()
    {
        committedLogCount++;
    }
    public void Pause(List<INode> nodes, int id)
    {
        nodes[id].responsive = false;
    }
    public void UnPause(Cluster cluster, List<INode> nodes, int id)
    {
        nodes[id].responsive = true;
        cluster.runCluster(nodes);
    }
    public bool Set(string value)
    {
        int key = committedLogCount;
        committedLogCount++;
        if (serverType != "leader" || value is null)
        {
            return false; 
        }

        ReceiveCommand(key, value);
        return true;
    }
}
