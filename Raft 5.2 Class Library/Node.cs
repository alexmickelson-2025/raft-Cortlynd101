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
        public int Id = 0;
        public bool _vote = false;
        public bool hasVoted = false;
        public bool hasActed = false;
        public string serverType = "follower";
        public bool receivedHeartBeat = false;
        public List<string> entries = [];
        public int leaderId { get; set; } = -1;
        public int electionTimeout = 0;
        public int voteCount = 0;
        public bool responsive = true;
        public int directedVote = 0;
        public int term = 0;
        public bool receivedResponse = false;
        public bool recievedRPC = false;
        public int votingFor = -1;
        public int termVotedFor = 0;
        public bool goFirst = false;

        public Node()
        {
        }

        public void Vote(List<Node> nodes, int id)
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

        public void Act(List<Node> nodes, int id, Election election)
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

        public void SendRPCs(Election election, List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                node.receiveRPC(election, nodes, Id, term);
            }
        }

        private void receiveRPC(Election election, List<Node> nodes, int id, int sentTerm)
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

        private void ElectionTimeout(Election election, List<Node> nodes)
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

        public void StartElection(Election election, List<Node> nodes)
        {
            term++;
            election.electionOngoing = true;
            election.term++;
            election.runElection(nodes);
        }

        public void AppendEntries(List<Node> nodes, int id)
        {
            Thread.Sleep(10);
            foreach (var node in nodes)
            {
                List<string> newEntries = ["1", "2"];
                node.RecieveAppendEntries(entries, id, term, nodes);
            }
        }

        private void RecieveAppendEntries(List<string> newEntries, int id, int receivedTerm, List<Node> nodes)
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

        public void SendHeartBeats(List<Node> nodes)
        {
            Thread.Sleep(40);
            foreach (var node in nodes)
            {
                node.ReceiveHeartBeat();
            }
        }

        public void SendHeartBeatsImmediately(List<Node> nodes)
        {
            foreach (var node in nodes)
            {
                node.ReceiveHeartBeat();
            }
        }

        private void ReceiveHeartBeat()
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
