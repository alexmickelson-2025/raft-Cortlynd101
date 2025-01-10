using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;
using NSubstitute.Core;
using Raft_5._2_Class_Library;
using System.Threading;
using System.Xml.Linq;
using System;

namespace Raft_5._2_Test_Scenarios
{
    public class RPCTests
    {
        //each node has a bool (e.g.flips a coin)
        //    the node node under test want to determine if there are a majority yes
        //        do TDD as pairs
        //        scenarios:
        //            single node cluster;
        //            node should return true vote when reque
        //            node should return false vote when requested
        //            just us says yes, then yes(do the TDD)
        //            just us says no, then no(do the TDD)
        //            three node cluster; (two mocked and one real)
        //            all respond: majority yes
        //            all respond: majority no
        //            one doesn't respond but majority clearly yes
        //            one doesn't respond but majority clearly no
        //            two are unresponsive: inconclusive after 300 ms
        //            one doesn't respond and inconclusive after 300 ms
        [Fact]
        public void NodeShouldReturnTrueVoteWhenRequested()
        {
            Node node = new(true);
            Assert.True(node.Request());
        }

        [Fact]
        public void NodeShouldReturnFalseVoteWhenRequested()
        {
            Node node = new(false);
            Assert.False(node.Request());
        }

        [Fact]
        public void WhenOnlyIVoteIWinIfMyVoteIsTrue()
        {
            Node node = new(true);
            Assert.True(node.IsElectionWinner());
        }

        [Fact]
        public void WhenOnlyIVoteILoseIfMyVoteIsFalse()
        {
            Node node = new(false);
            Assert.False(node.IsElectionWinner());
        }

        [Fact]
        public void FollowerVotesWhenElectionIsStarted()
        {
            //Given the sever is a follower
            Node node = new(true);
            node.becomeFollower();
            Assert.Equal("follower", node.serverType);
            
            //When an election has started
            Election election = new();
            List<Node> nodes = new List<Node>();
            nodes.Add(node);
            election.startElection(nodes);

            //Then the server casts their vote
            Assert.True(node.hasVoted);
        }
    }
}