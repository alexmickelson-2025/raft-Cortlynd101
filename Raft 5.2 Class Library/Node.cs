using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Raft_5._2_Class_Library
{
    public class Node
    {
        public bool _vote = false;
        public bool hasVoted = false;
        public bool hasActed = false;
        public string serverType = "follower";
        public bool receivedHeartBeat = false;
        public List<string> entries = [];
        public int leaderId = -1;
        public int electionTimeout = 0;
        public int voteCount = 0;
        public bool responsive = true;
        public int directedVote = 0;

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
            }
            else if (serverType == "directedFollower")
            {
                nodes[directedVote].voteCount++;
            }
            else
            {
                voteCount++;
            }
            hasActed = true;
        }

        public void Act(List<Node> nodes, int id, Election election)
        {
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
                StartElection(election, nodes);
            }
            hasActed = true;
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
            election.electionOngoing = true;
            election.term++;
            election.runElection(nodes);
        }

        private void AppendEntries(List<Node> nodes, int id)
        {
            Thread.Sleep(10);
            foreach (var node in nodes)
            {
                List<string> newEntries = ["1", "2"];
                node.RecieveAppendEntries(entries, id);
            }
        }

        private void RecieveAppendEntries(List<string> newEntries, int id)
        {
            entries = newEntries;
            leaderId = id;
        }

        public void SendHeartBeats(List<Node> nodes)
        {
            Thread.Sleep(40);
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
    }
}
