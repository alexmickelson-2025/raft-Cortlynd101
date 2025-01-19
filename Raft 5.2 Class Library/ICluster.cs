
namespace Raft_5._2_Class_Library
{
    public interface ICluster
    {
        Election election { get; set; }

        void runCluster(List<INode> nodes);
    }
}