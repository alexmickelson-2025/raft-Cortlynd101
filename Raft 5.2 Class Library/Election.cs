using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raft_5._2_Class_Library
{
    public class Election
    {
        bool isElectionRunning = false;

        public void startElection(List<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                node.Vote();
            }
            isElectionRunning = true;
        }
    }
}
