using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raft_5._2_Class_Library
{
    public class Node
    {
        bool _vote = false;
        public bool hasVoted = false;
        public string serverType = "follower";

        public Node(bool vote)
        {
            _vote = vote;
        }

        public bool Vote()
        {
            hasVoted = true;
            return _vote;
        }

        public bool IsElectionWinner()
        {
            return _vote;
        }

        public bool Request()
        {
            return _vote;
        }

        public void becomeFollower()
        {
            serverType = "follower";
        }
    }
}
