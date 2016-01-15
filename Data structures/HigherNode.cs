using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Nodes used on High level Astar Searches
public class HigherNode : IHeapItem<HigherNode>
{
    public HigherNode parent = null;
	public int G = 0;
	public int H = 0;
	public int F
	{
		get {return G + H;}
	}
    int heapIndex;

	public Node lowLevelNode = null;

    // stores connections to other HigherNodes in the Keys, with the distance between them as Value
	public Dictionary<HigherNode, int> connections = new Dictionary<HigherNode, int>();


    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }


    public int CompareTo(HigherNode nodeToCompare)
    {
        int compare = F.CompareTo(nodeToCompare.F);
        if(compare == 0)
            compare = H.CompareTo(nodeToCompare.H);

        return -compare;
    }
}
