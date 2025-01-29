using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NSubstitute.Core;
using Raft_5._2_Class_Library;
using System.Threading;
using System.Xml.Linq;
using System;
using Newtonsoft.Json.Linq;
using NSubstitute;
using static System.Collections.Specialized.BitVector32;
using static System.Formats.Asn1.AsnWriter;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Collections.Generic;
using System.Timers;
using Xunit;
using System.Diagnostics;
using Meziantou.Xunit;
using Castle.Components.DictionaryAdapter.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics.Metrics;
using System.Reflection.PortableExecutable;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Raft_5._2_Test_Scenarios;

public class LogReplicationTests
{
    [Fact]
    public void LeaderReceivesCommandSendsLogEntryInAppendEntriesRPCTest()
    {
        //TODO: Test 1, how are we sure that the leader is sending the logs in the test?
        //1. When a leader receives a client command the leader sends the log entry in the next AppendEntriesRPC to all nodes
        // Testing Logs #1
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        cluster.runCluster(nodes);

        //Then the follower should've received an AppendEntriesRPC
        Thread.Sleep(50);
        Assert.True(leader.sentRPCs);
    }
    [Fact]
    public void LeaderReceivesCommandAppendsItsLogTest()
    {
        //2. When a leader receives a command from the client, it is appended to its log
        // Testing Logs #2
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        cluster.runCluster(nodes);

        //Then the leader should have 1 item in its dictionary
        Thread.Sleep(50);
        Assert.Equal(1, leader.log?.Count);
    }
    [Fact]
    public void NewNodeHasEmptyLog()
    {
        //3. When a node is new, its log is empty
        // Testing Logs #3
        INode follower = new Node();
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower should have a log count of 0
        Thread.Sleep(50);
        Assert.Equal(0, follower.log?.Count);
    }
    [Fact]
    public void LeaderWinsElectionInitializesNextIndexOnFollowersTest()
    {
        //4. When a leader wins an election, it initializes the nextIndex for each follower to the index just after the last one it its log
        // Testing Logs #4
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the leader sets nextIndex and has 2 in nextIndex, one for itself, one for the follower
        Thread.Sleep(300);
        Assert.Equal(2, leader.nextIndex?.Count());
    }
    [Fact]
    public void LeaderMaintainsNextIndexForEachFollowersNextLogEntryTest()
    {
        //5. Leaders maintain a "nextIndex" for each follower that is the index of the next log entry the leader will send to that follower
        // Testing Logs #5
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex++;
        //We also have to add to the term so they stay in sync. This will be the case for all other times committedIndex is changed. 
        leader.term++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the first follower should have a log count of 1
        Thread.Sleep(300);
        Assert.Equal(1, leader.nextIndex[1]);
    }
    [Fact]
    public void HighestCommittedIndexIsInAppendRPCsTest()
    {
        //6. Highest committed index from the leader is included in AppendEntriesRPC's
        // Testing Logs #6
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex = 2;
        leader.term = 2;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the first follower should have a committedIndex of 2 (because that's what we set the leaders to)
        Thread.Sleep(300);
        Assert.Equal(2, follower.committedIndex);
    }
    [Fact]
    public void FollowerLearnsEntryIsCommittedAppliesItToItselfTest()
    {
        //7. When a follower learns that a log entry is committed, it applies the entry to its local state machine
        // Testing Logs #7
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //The leader recieves a command
        leader.ReceiveCommand(1, "number two");
        //The leader recieves a command
        leader.ReceiveCommand(2, "number three");
        //We add this to make it so the leader has one more committed entry then the follower (so the follower is behind).
        leader.committedIndex++;
        leader.term++; 


        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the first follower should have a committedIndex of 2
        //Then the leader should have three committed log entries
        //Then the follower should only have two committed log entries
        Thread.Sleep(300);
        Assert.Equal(2, follower.committedIndex);
        Assert.Equal(3, leader.log.Count());
        Assert.Equal(2, follower.log?.Count());
    }
    [Fact]
    public void LeaderReceivesMajorityLogConfirmationCommitsLogTest()
    {
        //8. When the leader has received a majority confirmation of a log, it commits it
        // Testing Logs #8
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower2 = Substitute.For<INode>();
        follower2.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower2.serverType = "follower";
            follower2.committedIndex = 0;
            follower2.log = new Dictionary<int, string>();
            follower2.responsive = true;
        });
        follower2.BecomeFollower();
        follower2.forcedOutcome = true;
        Assert.Equal("follower", follower2.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        nodes.Add(follower2);
        cluster.runCluster(nodes);

        //Then the leader should have a nodeCount of 3 and a committedLogCount of 0 (since it gets reset after a commit)
        //The leader should also have a commitIndex of 1
        //Then the followers should both have a committeedIndex of 1 and their first log should be "number one"
        Thread.Sleep(300);
        Assert.Equal(3, leader.nodeCount);
        Assert.Equal(0, leader.committedLogCount);
        Assert.Equal(1, leader.committedIndex);
        Assert.Equal(1, follower.committedIndex);
        Assert.Equal(1, follower2.committedIndex);
        Assert.Equal("number one", follower.log[0]);
        Assert.Equal("number one", follower2.log[0]);
    }
    [Fact]
    public void LeaderCommitsLogByIncrementingItsCommitedIndexTest()
    {
        //9. The leader commits logs by incrementing its committed log index
        // Testing Logs #9
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower2 = Substitute.For<INode>();
        follower2.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower2.serverType = "follower";
            follower2.committedIndex = 0;
            follower2.log = new Dictionary<int, string>();
            follower2.responsive = true;
        });
        follower2.BecomeFollower();
        follower2.forcedOutcome = true;
        Assert.Equal("follower", follower2.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        nodes.Add(follower2);
        cluster.runCluster(nodes);

        //Then the leader should have a nodeCount of 3 and a committedLogCount of 0 (since it gets reset after a commit)
        //The leader should also have a commitIndex of 1
        Thread.Sleep(300);
        Assert.Equal(3, leader.nodeCount);
        Assert.Equal(0, leader.committedLogCount);
        Assert.Equal(1, leader.committedIndex);
    }
    [Fact]
    public void FollowerRecievesAppendEntriesRPCAddsLogsToItsEntriesTest()
    {
        //10. Given a follower receives an AppendEntriesRPC with log(s) it will add those entries to its personal log
        // Testing Logs #10
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex++;
        leader.term++;
        //The leader recieves a command
        leader.ReceiveCommand(1, "number two");
        leader.committedIndex++;
        leader.term++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower should have two logs
        Thread.Sleep(300);
        Assert.Equal(2, follower.log?.Count());
        Assert.Equal("number one", follower.log?[0]);
        Assert.Equal("number two", follower.log?[1]);
    }
    [Fact]
    public void FollowerRespondsWithTermNumberAndLogEntryIndexTest()
    {
        //11. A followers response to an AppendEntriesRPC includes the followers term number and log entry index
        // Testing Logs #11
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex++;
        leader.term++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the leader has received the commit index and the term index
        Thread.Sleep(300);
        Assert.Equal(1, leader.receivedCommittedLogIndex);
        Assert.Equal(1, leader.receivedCommittedTermIndex);
    }
    [Fact]
    public void LeaderSendsConfirmationResponseAfterLogReplicationTest()
    {
        //12. When a leader receives a majority responses from the clients after a log replication heartbeat, the leader sends a confirmation response to the client
        // NOTE: I assumed he meant node right here instead of client?
        // Testing Logs #12
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex++;
        leader.term++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the node should have received a response
        Thread.Sleep(300);
        Assert.True(follower.receivedResponse);
    }
    [Fact]
    public void LeaderCommitsToInternalStateMachineTest()
    {
        //13. Given a leader node, when a log is committed, it applies it to its internal state machine
        // Testing Logs #13
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        leader.committedIndex++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the leader shoud've committed the log to its own machine
        Thread.Sleep(300);
        Assert.Equal(1, leader.log?.Count());
        Assert.Equal("number one", leader.log?[0]);
    }
    [Fact]
    public void FollowerMatchesCommitIndexIfItMatchesTest()
    {
        //14. When a follower receives a valid heartbeat, it increases its commitIndex to match the commit index of the heartbeat
        //Reject the heartbeat if the previous log index / term number does not match your log
        // Testing Logs #14
        //This one tests for a correct match
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower should have updated its committedIndex to 1.
        Thread.Sleep(300);
        Assert.Equal(1, follower.committedIndex);
    }
    [Fact]
    public void FollowerDoesntMatcheCommitIndexIfItDoesntMatchTest()
    {
        //14. When a follower receives a valid heartbeat, it increases its commitIndex to match the commit index of the heartbeat
        //Reject the heartbeat if the previous log index / term number does not match your log
        // Testing Logs #14
        //This one tests for an incorrect match
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //Offset the previousIndex and committedIndex by one so that they won't match correctly. 
        leader.previousIndex--;
        leader.committedIndex--;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt have updated its committedIndex to 1, so it should still be 0.
        Thread.Sleep(300);
        Assert.Equal(0, follower.committedIndex);
    }
    [Fact]
    public void LeaderSendsIndexAndTermFollowerRefusesIfItCantFindEntryTermLesserTest()
    {
        //15. When sending an AppendEntries RPC,the leader includes the index and term of the entry in its log that immediately precedes the new entries
        //    If the follower does not find an entry in its log with the same index and term, then it refuses the new entries
        //        Term must be same or newer
        //        If index is greater, it will be decreased by leader
        //        If index is less, we delete what we have
        //    If a follower rejects the AppendEntriesRPC, the leader decrements nextIndex and retries the AppendEntries RPC
        // Testing Logs #15
        //This one tests if the term is lesser
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //The term is one behind so it rejects it
        leader.term--;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt have updated its log or committedIndex because it rejects the AppendEntriesRPC.
        Thread.Sleep(300);
        Assert.Equal(0, follower.committedIndex);
        Assert.Equal(0, follower.log?.Count());
    }
    [Fact]
    public void LeaderSendsIndexAndTermFollowerRefusesIfItCantFindEntryIndexGreaterTest()
    {
        //15. When sending an AppendEntries RPC,the leader includes the index and term of the entry in its log that immediately precedes the new entries
        //    If the follower does not find an entry in its log with the same index and term, then it refuses the new entries
        //        Term must be same or newer
        //        If index is greater, it will be decreased by leader
        //        If index is less, we delete what we have
        //    If a follower rejects the AppendEntriesRPC, the leader decrements nextIndex and retries the AppendEntries RPC
        // Testing Logs #15
        //This one tests if the index is greater
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //The index is one ahead 
        leader.previousIndex++;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt have updated its log or committedIndex because it rejects the AppendEntriesRPC.
        Thread.Sleep(300);
        Assert.Equal(0, follower.committedIndex);
        Assert.Equal(0, follower.log?.Count());
    }
    [Fact]
    public void LeaderSendsIndexAndTermFollowerRefusesIfItCantFindEntryIndexLesserTest()
    {
        //15. When sending an AppendEntries RPC,the leader includes the index and term of the entry in its log that immediately precedes the new entries
        //    If the follower does not find an entry in its log with the same index and term, then it refuses the new entries
        //        Term must be same or newer
        //        If index is greater, it will be decreased by leader
        //        If index is less, we delete what we have
        //    If a follower rejects the AppendEntriesRPC, the leader decrements nextIndex and retries the AppendEntries RPC
        // Testing Logs #15
        //This one tests if the index is lesser
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //The index is one behind 
        leader.previousIndex--;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower should have updated its log or committedIndex because it works with the AppendEntriesRPC.
        Thread.Sleep(300);
        Assert.Equal(1, follower.committedIndex);
        Assert.Equal(1, follower.log?.Count());
    }
    [Fact]
    public void LeaderSendsIndexAndTermFollowerRefusesIfItCantFindEntryNothingWrongTest()
    {
        //15. When sending an AppendEntries RPC,the leader includes the index and term of the entry in its log that immediately precedes the new entries
        //    If the follower does not find an entry in its log with the same index and term, then it refuses the new entries
        //        Term must be same or newer
        //        If index is greater, it will be decreased by leader
        //        If index is less, we delete what we have
        //    If a follower rejects the AppendEntriesRPC, the leader decrements nextIndex and retries the AppendEntries RPC
        // Testing Logs #15
        //This one tests if nothing is wrong
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower should have updated its log because nothing is wrong with this one
        Thread.Sleep(300);
        Assert.Equal(1, follower.committedIndex);
        Assert.Equal(1, follower.log?.Count());
    }
    [Fact]
    public void EntryIsntCommittedWithoutMajorityTest()
    {
        //16. When a leader sends a heartbeat with a log, but does not receive responses from a majority of nodes, the entry is uncommitted
        // Testing Logs #16
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = false;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //The follower is unresponsive, so the log shouldn't get committed. Thus the committedIndex should be 0.
        Thread.Sleep(300);
        Assert.Equal(0, leader.committedIndex);
    }
    [Fact]
    public void LeaderStillSendsHeartbeatsEvenWithoutResponseTest()
    {
        //17. If a leader does not get a response from a follower, the leader continues to send the log entries in subsequent heartbeats  
        // Testing Logs #17
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = false;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        INode follower2 = Substitute.For<INode>();
        follower2.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower2.serverType = "follower";
            follower2.committedIndex = 0;
            follower2.log = new Dictionary<int, string>();
            follower2.responsive = true;
        });
        follower2.BecomeFollower();
        follower2.forcedOutcome = true;
        Assert.Equal("follower", follower2.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        nodes.Add(follower2);
        cluster.runCluster(nodes);

        //The first follower is unresponsive, but the second one should still receive the log (even though the first isn't responsive).
        Thread.Sleep(300);
        Assert.Equal(1, follower2.log?.Count());
        Assert.Equal("number one", follower2.log?[0]);

        //The first follower shouldn't have received any log updates
        Assert.Equal(0, follower.log?.Count());
    }
    [Fact]
    public void LeaderDoesntRespondToClientWithoutACommitTest()
    {
        //18. If a leader cannot commit an entry, it does not send a response to the client
        // Testing Logs #18
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = false;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt of reveived a response because it is unresponsive and won't commit the log.
        Thread.Sleep(300);
        Assert.False(follower.receivedResponse);
    }
    [Fact]
    public void NodeRejectsEntriesIfTheyreTooFarAheadTest()
    {
        //19. If a node receives an AppendEntriesRPC with logs that are too far in the future from your local state, you should reject the appendentries
        // Testing Logs #19
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //The leaders committedIndex is too far ahead (10 ahead or more)
        leader.committedIndex = 10;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt of committed the log so its commitedIndex should be 0 and the size of its log should be 0.
        Thread.Sleep(300);
        Assert.Equal(0, follower.committedIndex);
        Assert.Equal(0, follower.log?.Count());
    }
    [Fact]
    public void NodeRejectsIfTermAndIndexDontMatchTest()
    {
        //TODO: Test 20, how are you sure that the leader eventually finds a matching request?
        //20. If a node receives an AppendEntriesRPC with a term and index that do not match, you will reject the appendentry until you find a matching log
        // Testing Logs #20
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        //Create all the followers as mocks in order to have only one real node per test (except the test that is just a follower)
        INode follower = Substitute.For<INode>();
        follower.When(x => x.BecomeFollower()).Do(_ =>
        {
            follower.serverType = "follower";
            follower.committedIndex = 0;
            follower.log = new Dictionary<int, string>();
            follower.responsive = true;
        });
        follower.BecomeFollower();
        follower.forcedOutcome = true;
        Assert.Equal("follower", follower.serverType);

        //The leader recieves a command
        leader.ReceiveCommand(0, "number one");
        //Offset the leaders committedIndex by 1 to make it so the term and index aren't the same. 
        leader.committedIndex = 1;

        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        cluster.runCluster(nodes);

        //Then the follower shouldnt have updated its log or committedIndex because it rejects the AppendEntriesRPC.
        Thread.Sleep(300);
        Assert.Equal(0, follower.committedIndex);
        Assert.Equal(0, follower.log?.Count());
    }
}