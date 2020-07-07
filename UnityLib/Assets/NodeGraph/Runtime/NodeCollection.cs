using System.Collections.Generic;

public class NodeCollection<TNode> where TNode : BaseNode
{
    public List<TNode> Nodes = new List<TNode>();
    public List<NodeRelation> Relations = new List<NodeRelation>();
}
