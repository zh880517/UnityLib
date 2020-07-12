using System.Collections.Generic;
using UnityEngine;

public class GraphView : ScriptableObject
{
    public NodeGraph Graph;
    public List<GraphNodeRef> SelectNodes = new List<GraphNodeRef>();
}
