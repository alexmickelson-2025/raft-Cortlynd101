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

namespace Raft_5._2_Test_Scenarios
{
    //[DisableParallelization]
    public class RPCTests
    {
        [Fact]
        public void LeaderSendsHeartBeatsEvery50msTest()
        {
            //1. When a leader is active it sends a heart beat within 50ms.
            // Testing #1
            Node leader = new();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            Node follower = new();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Then the follower should've received a heartbeat after 50ms
            Thread.Sleep(50);
            Assert.True(follower.receivedHeartBeat);
        }

        [Fact]
        public void NodeRecognizesLeaderOnAppendEntriesTest()
        {
            //2. When a node receives an AppendEntries from another node, then first node remembers that other node is the current leader.
            // Testing #2
            Node leader = new();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            Node follower = new();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Then the follower appended its entries and found who the new leader is
            Thread.Sleep(50);
            Assert.True(follower.leaderId == 0);
        }

        [Fact]
        public void NodeStartsAsFollowerTest()
        {
            //3. When a new node is initialized, it should be in follower state.
            // Testing #3
            Node follower = new();
            Assert.Equal("follower", follower.serverType);
        }

        [Fact]
        public void NodeStartsAnElectionAfter300msTest()
        {
            //4. When a follower doesn't get a message for 300ms then it starts an election.
            // Testing #4
            Node follower = new();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(follower);
            cluster.runCluster(nodes);

            Thread.Sleep(300);
            //Check if at least one election has happened, raising the term amount above 0
            Assert.True(cluster.election.term > 0);
        }

        [Fact]
        public void ElectionResetTimerBetween150And300msTest()
        {
            //5. When the election time is reset, it is a random value between 150 and 300ms.
            //      1. between
            //      2. random: call n times and make sure that there are some that are different (other properties of the distribution if you like)
            // Testing #5
            Node follower = new();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Stopwatch stopwatch = new Stopwatch();
            //stopwatch.Start();
            //while (cluster.election.electionOngoing == false) { }
            //stopwatch.Stop();

            //Test theoretical time
            Assert.True(follower.electionTimeout >= 150);
            Assert.True(follower.electionTimeout <= 300);

            ////Test timed time
            //Assert.Equal(1, stopwatch.Elapsed.Milliseconds);
            //Assert.True(stopwatch.ElapsedMilliseconds >= 150);
            //Assert.True(stopwatch.ElapsedMilliseconds <= 300);
        }

        [Fact]
        public void NewElectionIncrementsTermTest()
        {
            //6. When a new election begins, the term is incremented by 1.
            //  1. Create a new node, store id in variable.
            //  2. wait 300 ms
            //  3. reread term(?)
            //  4. assert after is greater(by at least 1)
            // Testing #6
            Node follower = new();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(follower);
            cluster.runCluster(nodes);

            Thread.Sleep(300);
            Assert.Equal(1, cluster.election.term);
        }

        [Fact]
        public void FollowerResetsElectionTimerOnAppendEntriesMessageTest()
        {
            //7. When a follower does get an AppendEntries message, it resets the election timer. (i.e.it doesn't start an election even after more than 300ms)
            // Testing #7
            Node leader = new();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            Node follower = new();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            Thread.Sleep(300);
            Assert.Equal(0, cluster.election.term);
        }

        [Fact]
        public void CandidateWithMajorityVoteBecomesLeaderSingleNodeTest()
        {
            //8. Given an election begins, when the candidate gets a majority of votes, it becomes a leader. (think of the easy case; can use two tests for single and multi-node clusters)
            // Testing #8
            Node candidate = new();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(candidate);
            cluster.runCluster(nodes);

            Thread.Sleep(300);
            //Assert.Equal(1, cluster.election.term);
            Assert.Equal("leader", candidate.serverType);
        }

        [Fact]
        public void CandidateWithMajorityVoteBecomesLeaderMultiNodeTest()
        {
            //8. Given an election begins, when the candidate gets a majority of votes, it becomes a leader. (think of the easy case; can use two tests for single and multi-node clusters)
            // Testing #8
            Node candidate = new();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            Node follower = new();
            follower.serverType = "directedFollower";
            follower.directedVote = 0;
            Assert.Equal("directedFollower", follower.serverType);

            Node follower2 = new();
            follower2.serverType = "directedFollower";
            follower2.directedVote = 0;
            Assert.Equal("directedFollower", follower2.serverType);

            Node follower3 = new();
            follower3.serverType = "directedFollower";
            follower3.directedVote = 0;
            Assert.Equal("directedFollower", follower3.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(candidate);
            nodes.Add(follower);
            nodes.Add(follower2);
            nodes.Add(follower3);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(300);
            Assert.Equal("leader", candidate.serverType);
        }

        [Fact]
        public void CandidateWithMajorityVotesBecomesLeaderEvenIfNodesUnresponsiveTest()
        {
            //9. Given a candidate receives a majority of votes while waiting for unresponsive node, it still becomes a leader.
            // Testing #9
            Node candidate = new();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            Node follower = new();
            follower.serverType = "directedFollower";
            follower.directedVote = 0;
            Assert.Equal("directedFollower", follower.serverType);

            Node follower2 = new();
            follower2.serverType = "directedFollower";
            follower2.directedVote = 0;
            Assert.Equal("directedFollower", follower2.serverType);

            Node follower3 = new();
            follower3.serverType = "directedFollower";
            follower3.directedVote = 0;
            Assert.Equal("directedFollower", follower3.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(candidate);
            nodes.Add(follower);
            nodes.Add(follower2);
            nodes.Add(follower3);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(300);
            Assert.Equal("leader", candidate.serverType);
        }

        //[Fact]
        //public void FollowerInEarlierTermRespondsYesToRequestForVoteRPCTest()
        //{
        //    //10. A follower that has not voted and is in an earlier term responds to a RequestForVoteRPC with yes. (the reply will be a separate RPC)
        //    // Testing #10
        //}

        //11. Given a candidate server that just became a candidate, it votes for itself.
        //12. Given a candidate, when it receives an AppendEntries message from a node with a later term, then candidate loses and becomes a follower.
        //13. Given a candidate, when it receives an AppendEntries message from a node with an equal term, then candidate loses and becomes a follower.
        //14. If a node receives a second request for vote for the same term, it should respond no. (again, separate RPC for response)
        //15. If a node receives a second request for vote for a future term, it should vote for that node.
        //16. Given a candidate, when an election timer expires inside of an election, a new election is started.
        //17. When a follower node receives an AppendEntries request, it sends a response.
        //18. Given a candidate receives an AppendEntries from a previous term, then rejects.
        //19. When a candidate wins an election, it immediately sends a heart beat.
        //20. (testing persistence to disk will be a later assignment)

        //[Fact]
        //public void NodeShouldReturnTrueVoteWhenRequested()
        //{
        //    Node node = new(true);
        //    Assert.True(node.Request());
        //}

        //[Fact]
        //public void NodeShouldReturnFalseVoteWhenRequested()
        //{
        //    Node node = new(false);
        //    Assert.False(node.Request());
        //}

        //[Fact]
        //public void WhenOnlyIVoteIWinIfMyVoteIsTrue()
        //{
        //    Node node = new(true);
        //    Assert.True(node.IsElectionWinner());
        //}

        //[Fact]
        //public void WhenOnlyIVoteILoseIfMyVoteIsFalse()
        //{
        //    Node node = new(false);
        //    Assert.False(node.IsElectionWinner());
        //}

        //[Fact]
        //public void FollowerVotesWhenElectionIsStarted()
        //{
        //    //Given the sever is a follower
        //    Node node = new(true);
        //    node.becomeFollower();
        //    Assert.Equal("follower", node.serverType);

        //    //When an election has started
        //    Election election = new();
        //    List<Node> nodes = new List<Node>();
        //    nodes.Add(node);
        //    election.startElection(nodes);

        //    //Then the server casts their vote
        //    Assert.True(node.hasVoted);
        //}
    }
}