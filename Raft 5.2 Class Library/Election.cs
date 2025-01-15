using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Raft_5._2_Class_Library
{
    public class Election
    {
        public bool electionOngoing= false;
        public int term = 0;
        public int votesCast = 0;

        public void runElection(List<Node> nodes)
        {
            electionOngoing = true;

            while (electionOngoing)
            {
                Thread.Sleep(10);
                for (int i = 0; i < nodes.Count(); i++)
                {
                    if (nodes[i].responsive == true)
                    {
                        nodes[i].Vote(nodes, i);
                        votesCast++;
                    }
                }

                for (int i = 0; i < nodes.Count(); i++)
                {
                    if (nodes[i].voteCount > votesCast / 2)
                    {
                        nodes[i].BecomeLeader();
                    }
                }
                electionOngoing = false;
            }
        }
    }
}
