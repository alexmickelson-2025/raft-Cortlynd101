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

public class MiscTests
{
    [Fact]
    public void PausedLeaderDoesntSendHeartbeatAfter400msTest()
    {
        //1. When a node is a leader with an election loop, then they get paused, other nodes do not get heartbeats for 400ms
        // Testing Pause #1
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
        Assert.Equal("leader", leader.serverType);

        INode follower = new Node();
        follower.BecomeFollower();
        Assert.Equal("follower", follower.serverType);


        //When the cluster is running
        Cluster cluster = new();
        List<INode> nodes = new List<INode>();
        nodes.Add(leader);
        nodes.Add(follower);
        
        //And the leader is not responsive
        leader.Pause(nodes, leader.Id);
        cluster.runCluster(nodes);

        //Then the follower shouldn't receive a heartbeat after 400ms
        Thread.Sleep(400);
        Assert.False(leader.responsive);
        Assert.False(follower.receivedHeartBeat);
    }

    [Fact]
    public void PausedLeaderDoesSendHeartbeatIfPausedThenUnpausedTest()
    {
        //2. When a node is a leader with an election loop, then they get paused, other nodes do not get heartbeats for 400ms
        //Then they get un-paused and heartbeats resume
        // Testing Pause #2
        INode leader = new Node();
        leader.BecomeLeader();
        leader.forcedOutcome = true;
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

        //And the leader is not responsive
        leader.Pause(nodes, leader.Id);

        //Then the follower shouldn't receive a heartbeat after 50ms
        Thread.Sleep(400);
        leader.UnPause(cluster, nodes, leader.Id);

        Thread.Sleep(100);
        Assert.True(follower.receivedHeartBeat);
    }
    //[Fact]
    //public void FollowerGetsPausedDoesntBecomeCandidateTest()
    //{
    //    //3. When a follower gets paused, it does not time out to become a candidate
    //    // Testing Pause #3
    //    INode follower = new Node();
    //    follower.BecomeFollower();
    //    Assert.Equal("follower", follower.serverType);

    //    //When the cluster is running
    //    Cluster cluster = new();
    //    List<INode> nodes = new List<INode>();
    //    nodes.Add(follower);
    //    cluster.runCluster(nodes);

    //    //And the follower is paused
    //    follower.Pause(nodes, follower.Id);

    //    //Then the follower shouldn't have become a candidate
    //    Thread.Sleep(100);
    //    Assert.Equal("follower", follower.serverType);
    //}
    //[Fact]
    //public void FollowerGetsUnpausedBecomesCandidateTest()
    //{
    //    //4. When a follower gets unpaused, it will eventually become a candidate.
    //    // Testing Pause #4
    //    INode follower = new Node();
    //    follower.BecomeFollower();
    //    Assert.Equal("follower", follower.serverType);

    //    //When the cluster is running
    //    Cluster cluster = new();
    //    List<INode> nodes = new List<INode>();
    //    nodes.Add(follower);
    //    cluster.runCluster(nodes);

    //    //And the leader is not responsive
    //    follower.Pause(nodes, follower.Id);

    //    //Then the follower shouldn't receive a heartbeat after 50ms
    //    follower.UnPause(cluster, nodes, follower.Id);

    //    Thread.Sleep(100);
    //    Assert.Equal("candidate", follower.serverType);
    //}
}