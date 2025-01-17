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
    public class RPCTests
    {
        [Fact]
        public void LeaderSendsHeartBeatsEvery50msTest()
        {
            //1. When a leader is active it sends a heart beat within 50ms.
            // Testing #1
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);
        }

        [Fact]
        public void NodeStartsAnElectionAfter300msTest()
        {
            //4. When a follower doesn't get a message for 300ms then it starts an election.
            // Testing #4
            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode candidate = new Node();
            candidate.BecomeCandidate();
            candidate.term++;
            Assert.Equal("candidate", candidate.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode follower = new Node();
            follower.BecomeDirectedFollower();
            follower.directedVote = 0;
            Assert.Equal("directedFollower", follower.serverType);

            INode follower2 = new Node();
            follower2.BecomeDirectedFollower();
            follower2.directedVote = 0;
            Assert.Equal("directedFollower", follower2.serverType);

            INode follower3 = new Node();
            follower3.BecomeDirectedFollower();
            follower3.directedVote = 0;
            Assert.Equal("directedFollower", follower3.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode follower = new Node();
            follower.BecomeDirectedFollower();
            follower.directedVote = 0;
            Assert.Equal("directedFollower", follower.serverType);

            INode follower2 = new Node();
            follower2.BecomeDirectedFollower();
            follower2.directedVote = 0;
            Assert.Equal("directedFollower", follower2.serverType);

            INode follower3 = new Node();
            follower3.BecomeDirectedFollower();
            follower3.directedVote = 0;
            follower3.responsive = false;
            Assert.Equal("directedFollower", follower3.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
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
        public void FollowerInEarlierTermRespondsYesToRequestForVoteRPCTest()
        {
            //10. A follower that has not voted and is in an earlier term responds to a RequestForVoteRPC with yes. (the reply will be a separate RPC)
            // Testing #10
            INode candidate = new Node();
            candidate.BecomeCandidate();
            candidate.term++;
            Assert.Equal("candidate", candidate.serverType);

            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(follower);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(300);
            //The candidate becomes leader because it got the followers' vote
            Assert.Equal("leader", candidate.serverType);
        }

        [Fact]
        public void CandidateServerVotesForItselfTest()
        {
            //11.Given a candidate server that just became a candidate, it votes for itself.
            // Testing #11
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            cluster.runCluster(nodes);

            Thread.Sleep(300);
            //Assert.Equal(1, cluster.election.term);
            Assert.Equal("leader", candidate.serverType);
            Assert.Equal(1, candidate.voteCount);
            Assert.True(candidate.hasVoted);
        }

        [Fact]
        public void CandidateBecomesFollowerIfAppendEntriesHasLaterTermTest()
        {
            //12.Given a candidate, when it receives an AppendEntries message from a node with a later term, then candidate loses and becomes a follower.
            // Testing #12
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode candidate2 = new Node();
            candidate2.BecomeCandidate();
            Assert.Equal("candidate", candidate2.serverType);

            INode follower1 = new Node();
            follower1.BecomeDirectedFollower();
            follower1.directedVote = 1;
            Assert.Equal("directedFollower", follower1.serverType);

            INode follower2 = new Node();
            follower2.BecomeDirectedFollower();
            follower2.directedVote = 1;
            Assert.Equal("directedFollower", follower2.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(candidate2);
            nodes.Add(follower1);
            nodes.Add(follower2);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            //Then the candidate becomes a follower again
            Thread.Sleep(300);
            Assert.Equal("follower", candidate.serverType);
        }

        [Fact]
        public void CandidateBecomesFollowerIfAppendEntriesHasEqualTermTest()
        {
            //13.Given a candidate, when it receives an AppendEntries message from a node with an equal term, then candidate loses and becomes a follower.
            // Testing #13
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode candidate2 = new Node();
            candidate2.BecomeCandidate();
            Assert.Equal("candidate", candidate2.serverType);

            INode follower1 = new Node();
            follower1.BecomeDirectedFollower();
            follower1.directedVote = 1;
            Assert.Equal("directedFollower", follower1.serverType);

            INode follower2 = new Node();
            follower2.BecomeDirectedFollower();
            follower2.directedVote = 1;
            Assert.Equal("directedFollower", follower2.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(candidate2);
            nodes.Add(follower1);
            nodes.Add(follower2);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            //Then the candidate becomes a follower again
            Thread.Sleep(300);
            Assert.Equal("follower", candidate.serverType);
        }

        [Fact]
        public void NodeRefusesToVoteTwiceIfAskedToTest()
        {
            //14. If a node receives a second request for vote for the same term, it should respond no. (again, separate RPC for response)
            // Testing #14
            INode candidate = new Node();
            candidate.BecomeCandidate();
            //We have to make sure this one gets precedent, otherwise either could go first
            candidate.goFirst = true;
            Assert.Equal("candidate", candidate.serverType);

            INode candidate2 = new Node();
            candidate2.BecomeCandidate();
            Assert.Equal("candidate", candidate2.serverType);

            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(candidate2);
            nodes.Add(follower);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(300);
            //The first candidate becomes leader because it got to go first, and the second one was in the same term
            Assert.Equal("leader", candidate.serverType);
        }

        [Fact]
        public void NodeChangesVoteIfSenderIsInFutureTermTest()
        {
            //15. If a node receives a second request for vote for a future term, it should vote for that node.
            // Testing #15
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode candidate2 = new Node();
            candidate2.BecomeCandidate();
            //Ensure the term is greater than the followers terms and greater than the other candidate
            candidate2.term = candidate2.term + 3;
            Assert.Equal("candidate", candidate2.serverType);

            INode follower = new Node();
            Assert.Equal("follower", follower.serverType);

            INode follower2 = new Node();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(candidate2);
            nodes.Add(follower);
            nodes.Add(follower2);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(300);
            //The candidate becomes leader because it got the followers' vote
            Assert.Equal("leader", candidate2.serverType);
        }

        [Fact]
        public void CandidateStartsNewElectionIfElectionTimerExpiresTest()
        {
            //16. Given a candidate, when an election timer expires inside of an election, a new election is started.
            // Testing #16
            INode candidate = new Node();
            candidate.BecomeCandidate();
            Assert.Equal("candidate", candidate.serverType);

            INode follower = new Node();
            follower.BecomeDirectedFollower();
            follower.directedVote = 0;
            Assert.Equal("directedFollower", follower.serverType);

            INode follower2 = new Node();
            follower2.BecomeDirectedFollower();
            follower2.directedVote = 1;
            Assert.Equal("directedFollower", follower2.serverType);

            INode follower3 = new Node();
            follower3.BecomeDirectedFollower();
            follower3.directedVote = 1;
            follower3.responsive = false;
            Assert.Equal("directedFollower", follower3.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(candidate);
            nodes.Add(follower);
            nodes.Add(follower2);
            nodes.Add(follower3);
            cluster.runCluster(nodes);
            candidate.StartElection(cluster.election, nodes);

            Thread.Sleep(600);
            //We make sure more than one election has started (because the first one was a tie)
            Assert.True(cluster.election.term > 1);
        }

        [Fact]
        public void FollowerNodeRespondsToAppendEntriesTest()
        {
            //17. When a follower node receives an AppendEntries request, it sends a response.
            // Testing #17
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Then the follower sends a response to the leader
            Thread.Sleep(50);
            Assert.True(leader.receivedResponse);
        }

        [Fact]
        public void CandidateRejectsAppendEntriesFromPreviousTermTest()
        {
            //18. Given a candidate receives an AppendEntries from a previous term, then rejects.
            // Testing #18
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            //So that its in the next term
            follower.term = 1;
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Then the follower appended its entries and rejects the false leader
            Thread.Sleep(50);
            Assert.Equal(-1, follower.leaderId);
        }

        [Fact]
        public void CandidateImmediatelySendsHeartbeatWhenWinningAnElectionTest()
        {
            //19. When a candidate wins an election, it immediately sends a heart beat.
            // Testing #19
            INode leader = new Node();
            leader.BecomeLeader();
            Assert.Equal("leader", leader.serverType);

            INode follower = new Node();
            follower.BecomeFollower();
            Assert.Equal("follower", follower.serverType);

            //When the cluster is running
            Cluster cluster = new();
            List<INode> nodes = new List<INode>();
            nodes.Add(leader);
            nodes.Add(follower);
            cluster.runCluster(nodes);

            //Then the follower should've received a heartbeat after 0ms
            Thread.Sleep(0);
            Assert.True(follower.receivedHeartBeat);
        }
    }
}