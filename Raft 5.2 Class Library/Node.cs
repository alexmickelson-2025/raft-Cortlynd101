using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raft_5._2_Class_Library
{
    public class Node : INode
    {
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

        public Node()
        {
        }

        public void Vote(List<INode> nodes, int id)
        {
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
            Id = id;
            if (serverType == "leader")
            {
                leaderId = id;
                SendHeartBeats(nodes);
                AppendEntries(nodes, id);
            }
            else if (serverType == "follower")
            {
                ElectionTimeout(election, nodes);
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

        public void ElectionTimeout(Election election, List<INode> nodes)
        {
            Random random = new();
            int randomNum = random.Next(150, 300);
            electionTimeout = randomNum;

            Thread.Sleep(electionTimeout);
            if (!receivedHeartBeat)
            {
                StartElection(election, nodes);
            }
        }

        public void StartElection(Election election, List<INode> nodes)
        {
            term++;
            election.electionOngoing = true;
            election.term++;
            election.runElection(nodes);
        }

        public void AppendEntries(List<INode> nodes, int id)
        {
            Thread.Sleep(10);
            foreach (var node in nodes)
            {
                List<string> newEntries = ["1", "2"];
                node.RecieveAppendEntries(entries, id, term, nodes);
            }
        }

        public void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, List<INode> nodes)
        {
            if (serverType == "leader")
            {
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
                nodes[leaderId].RecieveAppendEntries(newEntries, leaderId, term, nodes);
            }
        }

        public void SendHeartBeats(List<INode> nodes)
        {
            Thread.Sleep(40);
            foreach (var node in nodes)
            {
                node.ReceiveHeartBeat();
            }
        }

        public void SendHeartBeatsImmediately(List<INode> nodes)
        {
            foreach (var node in nodes)
            {
                node.ReceiveHeartBeat();
            }
        }

        public void ReceiveHeartBeat()
        {
            receivedHeartBeat = true;
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
            serverType = "follower";
        }

        public void BecomeLeader()
        {
            serverType = "leader";
        }
        public void BecomeCandidate()
        {
            serverType = "candidate";
        }
        public void BecomeDirectedFollower()
        {
            serverType = "directedFollower";
        }
    }
}
