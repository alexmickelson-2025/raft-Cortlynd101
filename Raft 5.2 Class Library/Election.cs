using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Raft_5._2_Class_Library;

public class Election : IElection
{
    public bool electionOngoing { get; set; } = false;
    public int term { get; set; } = 0;
    public int votesCast { get; set; } = 0;

    public void runElection(List<INode> nodes)
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
                if (nodes[i].voteCount > votesCast / 2 || nodes[i].goFirst)
                {
                    nodes[i].BecomeLeader();
                    nodes[i].SendHeartBeatsImmediately(nodes);
                    for (int j = 0; j < nodes.Count(); j++)
                    {
                        if (j != i)
                        {
                            nodes[j].BecomeFollower();
                        }
                    }
                    break;
                }
            }

            electionOngoing = false;
        }
    }
}
