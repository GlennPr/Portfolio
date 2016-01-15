using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sector
{
	public int gridX = 0;
	public int gridY = 0;

	public int ID = 0;

	public int top = 0;
	public int bottom = 0;
	public int left = 0;
	public int right = 0;

    // list of higher nodes within the sector, orderned by edge. As sector is always a square shape, therefore 4 lists are needed. 
    // example: higherNodesOnEdge[0] == top row,  higherNodesOnEdge[1] = bottom row,  higherNodesOnEdge[2] == left row,  higherNodesOnEdge[3] = right row
	public List<HigherNode>[] higherNodesOnEdge = new List<HigherNode>[4];


	public void Setup()
	{
		for(int i = 0; i < higherNodesOnEdge.Length; i++)
			higherNodesOnEdge[i] = new List<HigherNode>();
	}
}
